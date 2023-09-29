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
            ILogger<CreateRecursiveTableCommandHandler> logger) : base(dbContext, logger)
        {

        }

        protected override async Task<CreateRecursiveTableCommandResult> HandleInternal(CreateRecursiveTableCommand request, CancellationToken cancellationToken)
        {
            _dbContext.Cells.Add(new(request.SheetId, "var0", "0"));

            List<Cell> cells = new(request.Size);
            for (int i = 1; i <= request.Size; ++i)
            {
                cells.Add(new(request.SheetId, $"var{i}", $"=var{i-1}+1"));
            }
            _dbContext.AddRange(cells);
            
            // for (int i = 0; i <= 16000; ++i)
            // {
            //      _dbContext.Cells.Add(new(request.SheetId, $"var{i}", "{i}"));
            // }

            await _dbContext.SaveChangesAsync(cancellationToken);

            return new()
            {
                IsDone = true
            };
        }
    }
}