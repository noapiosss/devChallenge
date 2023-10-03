using System.Net;

namespace E2E.Data.Contracts
{
    public record CellData(string CellId, string Value, string Result);
    public record CellDataWithStatusCode(string CellId, string Value, string Result, HttpStatusCode StatusCode);
}