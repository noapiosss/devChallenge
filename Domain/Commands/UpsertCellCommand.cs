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
            ILogger<UpsertCellCommandHandler> logger) : base(dbContext, logger)
        {

        }

        protected override async Task<UpsertCellCommandResult> HandleInternal(UpsertCellCommand request, CancellationToken cancellationToken)
        {
            return new()
            {
                CellDTO = await TryUpsertValueAsync(request.Cell, cancellationToken)
            };
        }
    }
}