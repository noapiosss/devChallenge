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
                
                Dictionary<string, Cell> cells = await _dbContext.Cells
                    .Where(c => newCellNodeVariables.Contains(c.CellId))
                    .SelectMany(c => c.DependByCells)
                    .Select(c => c.DependedByCell)
                    .Concat(_dbContext.Cells.Where(c => newCellNodeVariables.Contains(c.CellId)))
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
                            Node newNode = _parser.Parse(cells[variable].Value);
                            cellNodes.Add(variable, newNode);
                            newCellNode = newCellNode.ReplaceVariable(variable, newNode);                        
                        }

                        variables = newCellNode.GetNodeVariables();
                    }
                }

                newCell.DependByCells = cells
                    .Where(c => newCellNodeVariables.Contains(c.Key))
                    .Select(c => c.Value)
                    .Select(c => new CellDependency()
                        {
                            DependedByCell = c,
                            DependedCell = newCell
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
                    Dictionary<string, Cell> dependedCells = await _dbContext.Cells
                        .Where(c => c.Equals(newCell))
                        .Include(c => c.DependedCells)
                        .Include(c => c.DependByCells)
                        .ToDictionaryAsync(c => c.CellId, cancellationToken);
                    
                    dependedCells.Remove(newCell.CellId);

                    foreach(KeyValuePair<string, Cell> dependedCell in dependedCells)
                    {
                        Node dependedCellNode = _parser.Parse(dependedCell.Value.Value);

                        ICollection<string> dependedCellVariable = dependedCellNode.GetNodeVariables();
                        while (dependedCellVariable.Count != 0)
                        {
                            foreach(string variable in variables)
                            {
                                if (cellNodes.ContainsKey(variable))
                                {
                                    dependedCellNode = dependedCellNode.ReplaceVariable(variable, cellNodes[variable]);
                                }
                                else
                                {
                                    Node newNode = _parser.Parse(cells[variable].Value);
                                    cellNodes.Add(variable, newNode);
                                    dependedCellNode = dependedCellNode.ReplaceVariable(variable, newNode);                        
                                }                    
                            }
                        }
                    }

                    newCell.DependedCells = await _dbContext.CellDependencies
                        .Where(c => c.DependedByCell.Equals(newCell))
                        .ToListAsync(cancellationToken);

                    _dbContext.Cells.Update(newCell);
                    _dbContext.SaveChanges();
                }                

                return new() 
                {
                    Name = newCell.CellId,
                    Value = newCell.Value,
                    Result = newCellResult,
                    IsValid = true
                };
            }            
            catch
            {
                return new() 
                {
                    Name = newCell.CellId,
                    Value = newCell.Value,
                    Result = "ERROR",
                    IsValid = false
                };
            }
        }

        protected async Task<CellDTO> GetCellDTOAsync(Cell cell, CancellationToken cancellationToken)
        {
            Dictionary<string, Cell> cells = await _dbContext.Cells
                .Where(c => c.Equals(cell))
                .Include(c => c.DependByCells)
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
                        Node newNode = _parser.Parse(cells[variable].Value);
                        cellNodes.Add(variable, newNode);
                        cellNode = cellNode.ReplaceVariable(variable, newNode);                        
                    }                    
                }
            }

            return new() 
            {
                Name = cell.CellId,
                Value = cell.Value,
                Result = cellNode.Evaluate(),
                IsValid = true
            };
        }

        protected async Task<List<CellDTO>> GetSheetAsync(string sheetId, CancellationToken cancellationToken)
        {
            Dictionary<string, Cell> cells = await _dbContext.Cells.ToDictionaryAsync(c => c.CellId, cancellationToken);

            Dictionary<string, Node> cellNodes = new();

            List<CellDTO> cellDTOs = new();

            foreach(KeyValuePair<string, Cell> cell in cells)
            {
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
                }

                cellDTOs.Add(new()
                {
                    Name = cell.Value.CellId,
                    Value = cell.Value.Value,
                    Result = cellNode.Evaluate(),
                    IsValid = true
                });
            }

            return cellDTOs;
        }
    }
}