using System.Text.Json;
using System.Threading.Tasks;
using Contracts.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Valiadation
{
    public class RouteParameterValidation
    {
        private readonly RequestDelegate _next;
    private readonly char[] invalidCellIdSigns = new char[] {' ', '+','-','/','*','=','(',')'};

    public RouteParameterValidation(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        
        // cellId
        string cellId = context.Request.RouteValues["cellId"] as string;
        if (!IsValidCellId(cellId, out string message))
        {
            context.Response.StatusCode = 400;
            context.Response.ContentType = "application/json";

            ErrorResponse errorResponse = new()
            {
                Code = ErrorCode.InvalidCellId,
                Message = message
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
            return;
        }

        await _next(context);
    }

    private bool IsValidCellId(string cellId, out string message)
    {
        foreach (char sign in invalidCellIdSigns)
        {
            if (cellId.Contains(sign))
            {
                message = $"CellId cannot contains '{sign}'";
                return false;
            }
        }

        message = "";
        return true;
    }
}
}