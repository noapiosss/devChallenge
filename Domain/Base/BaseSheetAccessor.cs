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

namespace Domain.Base
{
    internal abstract class BaseSheetAccessor<TRequest, TResult> : BaseHandler<TRequest, TResult> where TRequest : IRequest<TResult>
    {
        protected readonly SheetsDbContext _dbContext;

        protected BaseSheetAccessor(SheetsDbContext dbContext,
            ILogger logger) : base(logger)
        {
            _dbContext = dbContext;
        }

        protected async Task<CellDTO> TryUpsertValueAsync(Cell newCell, CancellationToken cancellationToken)
        {
            AsyncParser asyncParser = new(_dbContext, newCell);
            ParseResult parseResult = await asyncParser.ParseAsync(newCell.CellId, cancellationToken);

            if(parseResult.IsValid)
            {               
                if (!await _dbContext.Cells.AnyAsync(c => c.Equals(newCell), cancellationToken))
                {
                    await _dbContext.Cells.AddAsync(newCell, cancellationToken);
                    await _dbContext.SaveChangesAsync(cancellationToken);
                }
                else
                {    
                    Cell oldCell = await _dbContext.Cells.FirstAsync(c => c.Equals(newCell), cancellationToken);
                    oldCell.Value = newCell.Value;
                    oldCell.IsExpression = newCell.IsExpression;
                    _dbContext.SaveChanges();
                }
            }
            
            return new() 
            {
                Name = newCell.CellId,
                Value = newCell.Value,
                Result = parseResult.Result,
                IsValid = parseResult.IsValid
            };
        }

        protected async Task<CellDTO> GetCellDTOAsync(Cell cell, CancellationToken cancellationToken)
        {
            AsyncParser asyncParser = new(_dbContext, cell);
            ParseResult parseResult = await asyncParser.ParseAsync(cell.CellId, cancellationToken);

            return new() 
            {
                Name = cell.CellId,
                Value = cell.Value,
                Result = parseResult.Result,
                IsValid = parseResult.IsValid
            };
        }

        protected async Task<List<CellDTO>> GetSheetAsync(string sheetId, CancellationToken cancellationToken)
        {
            List<CellDTO> cells = new();
            Parser parser = new(cells: await _dbContext.Cells.Where(c => c.SheetId == sheetId).ToDictionaryAsync(c => c.CellId, cancellationToken));

            foreach(Cell cell in parser.Cells.Values)
            {
                string result = parser.Parse(cell.CellId).Evaluate();

                cells.Add(new()
                {
                    Name = cell.CellId,
                    Value = cell.Value,
                    Result = result,
                    IsValid = true
                });
            }

            return cells;
        }
    }
}