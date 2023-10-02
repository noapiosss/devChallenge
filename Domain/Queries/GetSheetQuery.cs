using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Contracts.DTO;
using Domain.Base;
using Domain.Database;
using Domain.Helpers.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Domain.Commands
{
    public class GetSheetQuery : IRequest<GetSheetQueryResult>
    {
        public string SheetId { get; set; }
    }

    public class GetSheetQueryResult
    {
        public bool IsFound { get; init; }
        public List<CellDTO> CellDTOs { get; init; }
    }

    internal class GetSheetQueryHandler : BaseSheetAccessor<GetSheetQuery, GetSheetQueryResult>
    {
        public GetSheetQueryHandler(SheetsDbContext dbContext,
            IParser parser,
            ILogger<GetSheetQueryHandler> logger) : base(dbContext, parser, logger)
        {

        }

        protected override async Task<GetSheetQueryResult> HandleInternal(GetSheetQuery request, CancellationToken cancellationToken)
        {
            request.SheetId = request.SheetId.ToLower();

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