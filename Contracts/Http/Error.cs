namespace Contracts.Http
{
    public enum ErrorCode
    {
        BadRequest = 40000,
        InvalidCellId = 40001,
        SheetNotFound = 40401,
        CellNotFound = 40402,
        InvalidFormula = 42200,        
        InternalServerError = 50000,
        DbFailureError = 50001
    }

    public class ErrorResponse
    {
        public ErrorCode Code { get; init; }
        public string Message { get; init; }
    }
}