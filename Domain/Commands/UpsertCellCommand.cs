using System.Threading;
using System.Threading.Tasks;
using Contracts.Database;
using Contracts.DTO;
using Domain.Base;
using Domain.Database;
using Domain.Helpers.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Domain.Commands
{
    public class UpsertCellCommand : IRequest<UpsertCellCommandResult>
    {
        public Cell Cell { get; init; }
    }

    public class UpsertCellCommandResult
    {
        public CellDTO CellDTO { get; init; }
    }

    internal class UpsertCellCommandHandler : BaseSheetAccessor<UpsertCellCommand, UpsertCellCommandResult>
    {
        public UpsertCellCommandHandler(SheetsDbContext dbContext,
            IParser parser,
            ILogger<UpsertCellCommandHandler> logger) : base(dbContext, parser, logger)
        {

        }

        protected override async Task<UpsertCellCommandResult> HandleInternal(UpsertCellCommand request, CancellationToken cancellationToken)
        {
            request.Cell.SheetId = request.Cell.SheetId.ToLower();
            request.Cell.CellId = request.Cell.CellId.ToLower();

            return new()
            {
                CellDTO = await TryUpsertValueAsync(request.Cell, cancellationToken)
            };
        }
    }
}