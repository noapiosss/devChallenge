using System;
using System.Threading;
using System.Threading.Tasks;
using Contracts.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Api.Controllers
{
    public class BaseController : ControllerBase
    {
        protected readonly ILogger Logger;

        public BaseController(ILogger logger)
        {
            Logger = logger;
        }

        protected async Task<IActionResult> SafeExecute(Func<Task<IActionResult>> action, CancellationToken cancellationToken)
        {
            try
            {
                return await action();
            }
            catch (InvalidCastException ioe) when (ioe.InnerException is NpgsqlException)
            {
                ErrorResponse response = new()
                {
                    Code = ErrorCode.DbFailureError,
                    Message = "DB failure"
                };

                return ToActionResult(response);
            }
            catch (Exception)
            {
                ErrorResponse response = new()
                {
                    Code = ErrorCode.InternalServerError,
                    Message = "Unhabdled error"
                };

                return ToActionResult(response);
            }
        }

        protected IActionResult ToActionResult(ErrorResponse errorResponse)
        {
            return StatusCode((int)errorResponse.Code / 100, errorResponse);
        }
    }
}