using System.Collections.Generic;
using Microsoft.EntityFrameworkCore; 
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Contracts.CalculationTree;
using Contracts.Database;
using Contracts.DTO;
using Domain.Database;
using MediatR;
using Microsoft.Extensions.Logging;
using Domain.Helpers;
using System.Linq.Expressions;
using System;

namespace Domain.Base
{
    internal abstract class BaseSheetAccessor<TRequest, TResult> : BaseHandler<TRequest, TResult> where TRequest : IRequest<TResult>
    {
        protected readonly SheetsDbContext _dbContext;
        private readonly Parser _parser;

        protected BaseSheetAccessor(SheetsDbContext dbContext,
            ILogger logger) : base(logger)
        {
            _dbContext = dbContext;
            _parser = new();
        }

        protected async Task<CellDTO> TryUpsertValueAsync(Cell newCell, CancellationToken cancellationToken)
        {
            try
            {
                Node newCellNode = _parser.Parse(newCell.Value);
                ICollection<string> newCellNodeVariables = newCellNode.GetNodeVariables();

                Dictionary<string, Cell> dependedByCells = newCellNodeVariables.Count == 0 ?
                    new() :
                    await _dbContext
                        .GetAllDependency(newCell.SheetId, newCellNodeVariables)
                        .Select(cd => cd.DependedByCell)
                        .Concat(_dbContext.Cells.Where(c => c.SheetId == newCell.SheetId && newCellNodeVariables.Contains(c.CellId)))
                        .Distinct()
                        .ToDictionaryAsync(c => c.CellId, cancellationToken);

                Dictionary<string, Node> cellNodes = new();
                ICollection<string> variables = newCellNode.GetNodeVariables();
                while (variables.Count != 0)
                {
                    foreach(string variable in variables)
                    {
                        if (cellNodes.ContainsKey(variable))
                        {
                            newCellNode = newCellNode.ReplaceVariable(variable, cellNodes[variable]);
                        }
                        else
                        {
                            Node newNode = _parser.Parse(dependedByCells[variable].Value);
                            cellNodes.Add(variable, newNode);
                            newCellNode = newCellNode.ReplaceVariable(variable, newNode);                        
                        }

                        variables = newCellNode.GetNodeVariables();
                    }
                }

                newCell.DependByCells = dependedByCells
                    .Where(c => newCellNodeVariables.Contains(c.Key))
                    .Select(c => c.Value)
                    .Select(c => new CellDependency()
                        {
                            SheetId = newCell.SheetId, 
                            DependedByCellId = c.CellId,
                            DependedCellId = newCell.CellId
                        })
                    .ToList();

                string newCellResult = newCellNode.Evaluate();

                if (!await _dbContext.Cells.AnyAsync(c => c.Equals(newCell), cancellationToken))
                {   
                    await _dbContext.Cells.AddAsync(newCell, cancellationToken);
                    await _dbContext.SaveChangesAsync(cancellationToken);
                }
                else
                {
                    Dictionary<string, Cell> dependedCells = await _dbContext
                        .GetAllDependedBy(newCell.SheetId, newCell.CellId)
                        .Select(cd => cd.DependedCell)
                        .ToDictionaryAsync(c => c.CellId, cancellationToken);

                    cellNodes.Add(newCell.CellId, newCellNode);

                    foreach(KeyValuePair<string, Cell> dependedCell in dependedCells)
                    {
                        Node dependedCellNode = _parser.Parse(dependedCell.Value.Value);

                        ICollection<string> dependedCellVariables = dependedCellNode.GetNodeVariables();
                        while (dependedCellVariables.Count != 0)
                        {
                            foreach(string variable in dependedCellVariables)
                            {
                                if (cellNodes.ContainsKey(variable))
                                {
                                    dependedCellNode = dependedCellNode.ReplaceVariable(variable, cellNodes[variable]);
                                }
                                else
                                {
                                    Node newNode = _parser.Parse(dependedCells[variable].Value);
                                    cellNodes.Add(variable, newNode);
                                    dependedCellNode = dependedCellNode.ReplaceVariable(variable, newNode);                        
                                }                    
                            }

                            dependedCellVariables = dependedCellNode.GetNodeVariables();
                        }

                        _ = dependedCellNode.Evaluate();
                    }

                    _dbContext.Cells.Update(newCell);
                    _dbContext.SaveChanges();
                }                

                return new() 
                {
                    Name = newCell.CellId,
                    Value = $"={newCell.Value}",
                    Result = newCellResult,
                    IsValid = true
                };
            }            
            catch
            {
                return new() 
                {
                    Name = newCell.CellId,
                    Value = $"{(newCell.IsExpression ? "=" : "")}{newCell.Value}",
                    Result = "ERROR",
                    IsValid = false
                };
            }
        }

        protected async Task<CellDTO> GetCellDTOAsync(Cell cell, CancellationToken cancellationToken)
        {
            Dictionary<string, Cell> dependedByCells = await _dbContext
                    .GetAllDependency(cell.SheetId, new List<string>() {cell.CellId})
                    .Select(cd => cd.DependedByCell)
                    .ToDictionaryAsync(c => c.CellId, cancellationToken);

            Dictionary<string, Node> cellNodes = new();

            Node cellNode = _parser.Parse(cell.Value);

            ICollection<string> variables = cellNode.GetNodeVariables();
            while (variables.Count != 0)
            {
                foreach(string variable in variables)
                {
                    if (cellNodes.ContainsKey(variable))
                    {
                        cellNode = cellNode.ReplaceVariable(variable, cellNodes[variable]);
                    }
                    else
                    {
                        Node newNode = _parser.Parse(dependedByCells[variable].Value);
                        cellNodes.Add(variable, newNode);
                        cellNode = cellNode.ReplaceVariable(variable, newNode);                        
                    }
                }

                variables = cellNode.GetNodeVariables();
            }

            return new() 
            {
                Name = cell.CellId,
                Value = $"={cell.Value}",
                Result = cellNode.Evaluate(),
                IsValid = true
            };
        }

        protected async Task<List<CellDTO>> GetSheetAsync(string sheetId, CancellationToken cancellationToken)
        {
            Dictionary<string, Cell> cells = await _dbContext.Cells
                .Where(c => c.SheetId == sheetId)
                .ToDictionaryAsync(c => c.CellId, cancellationToken);

            Dictionary<string, Node> cellNodes = new();

            List<CellDTO> cellDTOs = new();

            foreach(KeyValuePair<string, Cell> cell in cells)
            {
                if (!cell.Value.IsExpression)
                {
                    cellDTOs.Add(new()
                    {
                        Name = cell.Value.CellId,
                        Value = cell.Value.Value,
                        Result = cell.Value.Value,
                        IsValid = true
                    });
                    continue;
                }

                Node cellNode = _parser.Parse(cell.Value.Value);

                ICollection<string> variables = cellNode.GetNodeVariables();
                while (variables.Count != 0)
                {
                    foreach(string variable in variables)
                    {
                        if (cellNodes.ContainsKey(variable))
                        {
                            cellNode = cellNode.ReplaceVariable(variable, cellNodes[variable]);
                        }
                        else
                        {
                            Node newNode = _parser.Parse(cells[variable].Value);
                            cellNodes.Add(variable, newNode);
                            cellNode = cellNode.ReplaceVariable(variable, newNode);                        
                        }                    
                    }

                    variables = cellNode.GetNodeVariables();
                }

                cellDTOs.Add(new()
                {
                    Name = cell.Value.CellId,
                    Value = $"={cell.Value.Value}",
                    Result = cellNode.Evaluate(),
                    IsValid = true
                });
            }

            return cellDTOs;
        }
    }
}