using System.Net.Http;
using System.Net.Http.Json;
using Contracts.Http;
using E2E.Data;
using E2E.Tests.Base;
using Shouldly;
using Xunit.Abstractions;

namespace E2E.Tests
{
    public class CellIdNamingTest : BaseTest
    {
        public CellIdNamingTest(ITestOutputHelper output) : base(output)
        {

        }

        [Fact]
        public async void CellShouldNotStartsWithDigit()
        {
            // Arrange
            string sheetId = $"sheet{_random.Next(1000000, 9999999)}";
            string cellId = $"{_random.Next(10)}cell";

            // Act
            UpsertCellRequest request = new() { Value = "expression" };
            HttpResponseMessage response = await _client.PostAsJsonAsync($"/api/v1/{sheetId}/{cellId}", request);
            ErrorResponse result = await response.Content.ReadFromJsonAsync<ErrorResponse>();

            // Assert
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
            result.Code.ShouldBe(ErrorCode.InvalidCellId);
            result.Message.ShouldBe("CellId cannot starts with a number");
        }

        [Theory]
        [ClassData(typeof(NotValidCellIdSigns))]
        public async void CellShouldNotContainsSign(char sign)
        {
            // Arrange
            string sheetId = $"sheet{_random.Next(1000000, 9999999)}";
            string cellId = $"cell{sign}llec";

            // Act
            UpsertCellRequest request = new() { Value = "expression" };
            HttpResponseMessage response = await _client.PostAsJsonAsync($"/api/v1/{sheetId}/{cellId}", request);
            ErrorResponse result = await response.Content.ReadFromJsonAsync<ErrorResponse>();

            // Assert
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
            result.Code.ShouldBe(ErrorCode.InvalidCellId);
            result.Message.ShouldBe($"CellId cannot contains '{sign}' sign");
        }
    }
}