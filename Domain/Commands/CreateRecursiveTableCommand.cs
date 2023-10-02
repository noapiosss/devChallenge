using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Contracts.Database;
using Domain.Base;
using Domain.Database;
using Domain.Helpers.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Domain.Commands
{
    public class CreateRecursiveTableCommand : IRequest<CreateRecursiveTableCommandResult>
    {
        public string SheetId { get; init; }
        public int Size { get; init; }
    }

    public class CreateRecursiveTableCommandResult
    {
        public bool IsDone { get; init; }
    }

    internal class CreateRecursiveTableCommandHandler : BaseSheetAccessor<CreateRecursiveTableCommand, CreateRecursiveTableCommandResult>
    {
        public CreateRecursiveTableCommandHandler(SheetsDbContext dbContext,
            IParser parser,
            ILogger<CreateRecursiveTableCommandHandler> logger) : base(dbContext, parser, logger)
        {

        }

        protected override async Task<CreateRecursiveTableCommandResult> HandleInternal(CreateRecursiveTableCommand request, CancellationToken cancellationToken)
        {
            _dbContext.Cells.Add(new(request.SheetId, "var0", "0"));

            List<Cell> cells = new(request.Size);
            List<CellDependency> cellDependencies = new(request.Size);
            for (int i = 1; i <= request.Size; ++i)
            {
                cells.Add(new(request.SheetId, $"var{i}", $"=var{i-1}+1"));
                cellDependencies.Add(new()
                {
                    SheetId = request.SheetId,
                    DependedCellId = $"var{i}",
                    DependedByCellId = $"var{i-1}"
                });
            }
            _dbContext.Cells.AddRange(cells);
            _dbContext.CellDependencies.AddRange(cellDependencies);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return new()
            {
                IsDone = true
            };
        }
    }
}