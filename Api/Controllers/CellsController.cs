using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using Contracts.Database;
using Contracts.DTO;
using Contracts.Http;
using Domain.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Api.Controllers
{
    public class FiltersController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly char[] _invalidCellIdSigns = new char[] {' ', '+','-','/','*','=','(',')', ',', '.'};

        public FiltersController(IMediator mediator,
            ILogger<FiltersController> logger) : base(logger)
        {
            _mediator = mediator;
        }

        [HttpPost("/api/v1/{sheetId}/{cellId}")]
        public Task<IActionResult> UpsertCell([FromRoute] string sheetId, string cellId, [FromBody] UpsertCellRequest request, CancellationToken cancellationToken)
        {
            return SafeExecute(async () =>
            {
                if (!IsValidCellId(cellId, out string message))
                {
                    return ToActionResult(new()
                    {
                        Code = ErrorCode.InvalidCellId,
                        Message = message
                    });
                }

                UpsertCellCommand command = new()
                {
                    Cell = new(sheetId, cellId, request.Value)
                };
                UpsertCellCommandResult result = await _mediator.Send(command, cancellationToken);
                GetCellResponse response = new()
                {
                    Value = result.CellDTO.Value,
                    Result = result.CellDTO.Result
                };
                
                if (!result.CellDTO.IsValid)
                {
                    return UnprocessableEntity(response);
                }

                return Created("", response);

            }, cancellationToken);
        }

        [HttpGet("/api/v1/{sheetId}")]
        public Task<IActionResult> GetSheet([FromRoute] string sheetId, CancellationToken cancellationToken)
        {
            return SafeExecute(async () =>
            {
                GetSheetQuery query = new()
                {
                    SheetId = sheetId
                };
                GetSheetQueryResult result = await _mediator.Send(query, cancellationToken);

                if (!result.IsFound)
                {
                    return ToActionResult(new()
                    {
                        Code = ErrorCode.SheetNotFound,
                        Message = "Sheet is not found"
                    });
                }

                Dictionary<string, GetCellResponse> sheet = new(result.CellDTOs.Count);
                foreach (CellDTO cellDTO in result.CellDTOs)
                {
                    sheet[cellDTO.Name] = new()
                    {
                        Value = cellDTO.Value,
                        Result = cellDTO.Result
                    };
                }

                return Ok(sheet);

            }, cancellationToken);
        }

        [HttpGet("/api/v1/{sheetId}/{cellId}")]
        public Task<IActionResult> GetCell([FromRoute] string sheetId, string cellId, CancellationToken cancellationToken)
        {
            return SafeExecute(async () =>
            {
                if (!IsValidCellId(cellId, out string message))
                {
                    return ToActionResult(new()
                    {
                        Code = ErrorCode.InvalidCellId,
                        Message = message
                    });
                }

                GetCellQuery query = new()
                {
                    SheetId = sheetId,
                    CellId = cellId
                };
                GetCellQueryResult result = await _mediator.Send(query, cancellationToken);
                
                if (!result.IsFound)
                {
                    return ToActionResult(new()
                    {
                        Code = ErrorCode.CellNotFound,
                        Message = "Cell is not found"
                    });
                }

                GetCellResponse response = new()
                {
                    Value = result.CellDTO.Value,
                    Result = result.CellDTO.Result
                };

                return Ok(response);

            }, cancellationToken);
        }

        private bool IsValidCellId(string cellId, out string message)
        {
            if (char.IsDigit(cellId[0]))
            {
                message = $"CellId cannot starts with a number";
                return false;
            }

            foreach (char sign in _invalidCellIdSigns)
            {
                if (cellId.Contains(sign))
                {
                    message = $"CellId cannot contains '{sign}' sign";
                    return false;
                }
            }

            message = "";
            return true;
        }
    }
}