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
using Domain.Helpers.Interfaces;
using System;

namespace Domain.Base
{
    internal abstract class BaseSheetAccessor<TRequest, TResult> : BaseHandler<TRequest, TResult> where TRequest : IRequest<TResult>
    {
        protected readonly SheetsDbContext _dbContext;
        private readonly IParser _parser;

        protected BaseSheetAccessor(SheetsDbContext dbContext,
            IParser parser,
            ILogger logger) : base(logger)
        {
            _dbContext = dbContext;
            _parser = parser;
        }

        protected async Task<CellDTO> TryUpsertValueAsync(Cell newCell, CancellationToken cancellationToken)
        {
            try
            {
                Node newCellNode = _parser.Parse(newCell.Value);
                ICollection<string> newCellNodeVariables = newCellNode.GetNodeVariables();
                Dictionary<string, Node> cellNodes = new();
                string newCellResult;

                if (newCell.IsExpression)
                {
                    Dictionary<string, Cell> dependedByCells = newCellNodeVariables.Count == 0 ?
                        new() :
                        await _dbContext
                            .GetAllDependencies(newCell.SheetId, newCellNodeVariables)
                            .ToDictionaryAsync(c => c.CellId, cancellationToken);

                    ICollection<string> variables = newCellNode.GetNodeVariables();

                    if (newCellNode is ValueNode && variables.Count == 1) // resursive to string value
                    {
                        while (dependedByCells.ContainsKey(variables.First()) && _parser.Parse(dependedByCells[variables.First()].Value) is ValueNode)
                        {   
                            variables = new List<string>() {dependedByCells[variables.First()].Value};
                        }

                        
                        newCellResult = variables.First();
                    }
                    else
                    {
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

                        newCellResult = newCellNode.Evaluate();
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
                }
                else
                {
                    newCellResult = newCell.Value;
                }

                if (!await _dbContext.Cells.AnyAsync(c => c.Equals(newCell), cancellationToken))
                {   
                    await _dbContext.Cells.AddAsync(newCell, cancellationToken);
                    await _dbContext.SaveChangesAsync(cancellationToken);
                }
                else
                {
                    Dictionary<string, Cell> dependedCells = await _dbContext
                        .GetAllDependedBy(newCell.SheetId, newCell.CellId)
                        .ToDictionaryAsync(c => c.CellId, cancellationToken);

                    cellNodes.Add(newCell.CellId, newCellNode);

                    if (newCell.IsExpression)
                    {
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
                    }
                    else
                    {
                        foreach(KeyValuePair<string, Cell> dependedCell in dependedCells)
                        {
                            Node dependedCellNode = _parser.Parse(dependedCell.Value.Value);
                            ICollection<string> variables = dependedCellNode.GetNodeVariables();

                            if (dependedCellNode is ValueNode && variables.Count == 1) // resursive to string value
                            {
                                while (dependedCells.ContainsKey(variables.First()) && _parser.Parse(dependedCells[variables.First()].Value) is ValueNode)
                                {   
                                    variables = new List<string>() {dependedCells[variables.First()].Value};
                                }
                            }
                            else
                            {
                                throw new InvalidOperationException("Adding string to number");
                            }
                        }
                    }

                    _dbContext.Cells.Update(newCell);
                    _dbContext.SaveChanges();
                }                

                return new() 
                {
                    Name = newCell.CellId,
                    Value = $"{(newCell.IsExpression ? "=" : "")}{newCell.Value}",
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
                    .GetAllDependencies(cell.SheetId, new List<string>() {cell.CellId})
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