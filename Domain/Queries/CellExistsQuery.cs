using System.Collections.Generic;
using System.Linq;
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
    public class CellExistsQuery : IRequest<CellExistsQueryResult>
    {
        public string SheetId { get; set; }
        public string CellId { get; set; }
    }

    public class CellExistsQueryResult
    {
        public bool CellExists { get; init; }
    }

    internal class CellExistsQueryHandler : BaseSheetAccessor<CellExistsQuery, CellExistsQueryResult>
    {
        public CellExistsQueryHandler(SheetsDbContext dbContext,
            IParser parser,
            ILogger<CellExistsQueryHandler> logger) : base(dbContext, parser, logger)
        {

        }

        protected override async Task<CellExistsQueryResult> HandleInternal(CellExistsQuery request, CancellationToken cancellationToken)
        {
            request.SheetId = request.SheetId.ToLower();
            request.CellId = request.CellId.ToLower();

            return new()
            {
                CellExists = await _dbContext.Cells.AnyAsync(c => c.SheetId == request.SheetId && c.CellId == request.CellId)
            };
        }
    }
}