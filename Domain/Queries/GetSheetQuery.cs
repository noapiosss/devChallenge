using System.Collections.Generic;
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
    public class GetSheetQuery : IRequest<GetSheetQueryResult>
    {
        public string SheetId { get; init; }
    }

    public class GetSheetQueryResult
    {
        public bool IsFound { get; init; }
        public List<CellDTO> CellDTOs { get; init; }
    }

    internal class GetSheetQueryHandler : BaseSheetAccessor<GetSheetQuery, GetSheetQueryResult>
    {
        public GetSheetQueryHandler(SheetsDbContext dbContext,
            ILogger<GetSheetQueryHandler> logger) : base(dbContext, logger)
        {

        }

        protected override async Task<GetSheetQueryResult> HandleInternal(GetSheetQuery request, CancellationToken cancellationToken)
        {
            if (!await _dbContext.Cells.AnyAsync(c => c.SheetId == request.SheetId, cancellationToken))
            {
                return new()
                {
                    IsFound = false,
                    CellDTOs = null
                };
            }

            return new()
            {
                IsFound = true,
                CellDTOs = await GetSheetAsync(request.SheetId, cancellationToken)
            };
        }
    }
}