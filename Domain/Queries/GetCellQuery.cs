using System.Threading;
using System.Threading.Tasks;
using Contracts.Database;
using Contracts.DTO;
using Domain.Base;
using Domain.Database;
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
            ILogger<GetCellQueryHandler> logger) : base(dbContext, logger)
        {

        }

        protected override async Task<GetCellQueryResult> HandleInternal(GetCellQuery request, CancellationToken cancellationToken)
        {
            Cell cell = await _dbContext.Cells.FirstOrDefaultAsync(c => c.SheetId == request.SheetId && c.CellId == request.CellId, cancellationToken);

            if (cell is null)
            {
                return new()
                {
                    IsFound = false,
                    CellDTO = new()
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