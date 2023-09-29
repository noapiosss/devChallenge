using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Contracts.CalculationTree;
using Contracts.Database;
using Domain.Database;

namespace Domain.Helpers
{
    internal class AsyncParser
    {
        protected readonly SheetsDbContext _dbContext;
        private readonly string _sheetId;
        private Dictionary<string, Cell> _cells;
        private readonly Dictionary<string, int> _indexes;
        private readonly HashSet<string> _currentlyEvaluating;

        public AsyncParser(SheetsDbContext dbContext, Cell cell)
        {
            _dbContext = dbContext;
            _sheetId = cell.SheetId;
            _cells = new()
            {
                { cell.CellId, new(cell.SheetId, cell.CellId, (cell.IsExpression ? "=" : "") + cell.Value) }
            };
            _indexes = new();
            _currentlyEvaluating = new();
        }

        public async Task<ParseResult> ParseAsync(string cellId, CancellationToken cancellationToken)
        {            
            if (!_cells.ContainsKey(cellId))
            {
                Cell cell = await _dbContext.Cells.FirstOrDefaultAsync(c => c.SheetId == _sheetId && c.CellId == cellId, cancellationToken);
                if (cell is null)
                {
                    throw new NullReferenceException("There is no such variable.");
                }

                _cells.Add(cellId, cell);
            }

            if (!_cells[cellId].IsExpression)
            {
                return new()
                {
                    IsValid = true,
                    Result = _cells[cellId].Value 
                };
            }

            try
            {
                if (!_currentlyEvaluating.Add(cellId))
                {
                    throw new InvalidCastException("Self linking."); 
                }

                if (_indexes.ContainsKey(cellId))
                {
                    _indexes[cellId] = 0;
                }
                else
                {
                    _indexes.Add(cellId, 0);
                }
                
                if (_cells[cellId].Value.StartsWith('-'))
                {
                    _cells[cellId].Value = $"0{_cells[cellId].Value}";
                }

                _cells[cellId].Value = _cells[cellId].Value.PrepareExpression();
                ParseResult parseResult = new()
                {
                    IsValid = true,
                    Result = (await ParseExpressionAsync(cellId, cancellationToken)).Evaluate()
                };

                _currentlyEvaluating.Remove(cellId);

                return parseResult;
            }
            catch
            {
                return new()
                {
                    IsValid = false,
                    Result = "ERROR" 
                };
            }
        }
        
        private async Task<Node> ParseExpressionAsync(string cellId, CancellationToken cancellationToken)
        {
            Node left = await ParseTermAsync(cellId, cancellationToken);
            
            while (_indexes[cellId] < _cells[cellId].Value.Length &&
                (_cells[cellId].Value[_indexes[cellId]] == '+' || _cells[cellId].Value[_indexes[cellId]] == '-'))
            {
                char operation = _cells[cellId].Value[_indexes[cellId]++];
                Node right = await ParseTermAsync(cellId, cancellationToken);
                left = new OperationNode(left, right, operation);
            }

            return new ValueNode(left.Evaluate());
        }

        private async Task<Node> ParseTermAsync(string cellId, CancellationToken cancellationToken)
        {
            Node left = await ParseFactorAsync(cellId, cancellationToken);

            while (_indexes[cellId] < _cells[cellId].Value.Length &&
                (_cells[cellId].Value[_indexes[cellId]] == '*' || _cells[cellId].Value[_indexes[cellId]] == '/'))
            {
                char operation = _cells[cellId].Value[_indexes[cellId]++];
                Node right = await ParseFactorAsync(cellId, cancellationToken);
                left = new OperationNode(left, right, operation);
            }

            return new ValueNode(left.Evaluate());
        }

        private async Task<Node> ParseFactorAsync(string cellId, CancellationToken cancellationToken)
        {
            if (_indexes[cellId] < _cells[cellId].Value.Length && _cells[cellId].Value[_indexes[cellId]] == '(')
            {
                _indexes[cellId]++;
                Node node = await ParseExpressionAsync(cellId, cancellationToken);
                
                if (_indexes[cellId] >= _cells[cellId].Value.Length || _cells[cellId].Value[_indexes[cellId]] != ')')
                {
                    throw new InvalidOperationException("Mismatched parentheses.");
                }

                _indexes[cellId]++;
                return node;
            }
            else if (char.IsDigit(_cells[cellId].Value[_indexes[cellId]]) || char.IsLetter(_cells[cellId].Value[_indexes[cellId]]))
            {
                int start_index = _indexes[cellId];
                while (_indexes[cellId] < _cells[cellId].Value.Length &&
                    _cells[cellId].Value[_indexes[cellId]] != '+' &&
                    _cells[cellId].Value[_indexes[cellId]] != '-' &&
                    _cells[cellId].Value[_indexes[cellId]] != '*' &&
                    _cells[cellId].Value[_indexes[cellId]] != '/' &&
                    _cells[cellId].Value[_indexes[cellId]] != ')')
                {
                    _indexes[cellId]++;
                }

                string numberStr = _cells[cellId].Value[start_index.._indexes[cellId]];

                foreach (char c in numberStr)
                {
                    if (!char.IsDigit(c))
                    {
                        return new ValueNode((await ParseAsync(numberStr, cancellationToken)).Result);
                    }
                }

                return new ValueNode(numberStr);
            }
            else
            {
                throw new InvalidOperationException("Invalid character.");
            }
        }
    }
}