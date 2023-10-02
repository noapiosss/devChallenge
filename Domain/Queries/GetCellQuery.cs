using System.Threading;
using System.Threading.Tasks;
using Contracts.Database;
using Contracts.DTO;
using Domain.Base;
using Domain.Database;
using Domain.Helpers.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Domain.Commands
{
    public class GetCellQuery : IRequest<GetCellQueryResult>
    {
        public string SheetId { get; init; }
        public string CellId { get; init; }
    }

    public class GetCellQueryResult
    {
        public bool IsFound { get; init; }
        public CellDTO CellDTO { get; init; }
    }

    internal class GetCellQueryHandler : BaseSheetAccessor<GetCellQuery, GetCellQueryResult>
    {
        public GetCellQueryHandler(SheetsDbContext dbContext,
            IParser parser,
            ILogger<GetCellQueryHandler> logger) : base(dbContext, parser, logger)
        {

        }

        protected override async Task<GetCellQueryResult> HandleInternal(GetCellQuery request, CancellationToken cancellationToken)
        {
            Cell cell = await _dbContext.Cells
                .FirstOrDefaultAsync(c => c.SheetId == request.SheetId.ToLower() && c.CellId == request.CellId.ToLower(), cancellationToken);

            if (cell is null)
            {
                return new()
                {
                    IsFound = false,
                    CellDTO = new()
                };
            }

            if (!cell.IsExpression)
            {
                return new()
                {
                    IsFound = true,
                    CellDTO = new()
                    {
                        Name = cell.CellId,
                        Value = cell.Value,
                        Result = cell.Value,
                        IsValid = true
                    }
                };
            }

            return new()
            {
                IsFound = true,
                CellDTO = await GetCellDTOAsync(cell, cancellationToken)
            };
        }
    }
}