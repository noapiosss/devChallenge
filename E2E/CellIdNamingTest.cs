using System;
using System.Net.Http;
using System.Net.Http.Json;
using Contracts.Http;
using E2E.Data;
using Shouldly;

namespace E2E
{
    public class CellIdNamingTest
    {
        private readonly HttpClient _client;
        private readonly Random _random;

        public CellIdNamingTest()
        {
            _client = new()
            {
                BaseAddress = new Uri("http://localhost:8080")
            };

            _random = new();
        }

        [Fact]
        public async void CellShouldNotStartsWithDigit()
        {
            // Arrange
            string sheetId = $"sheet{_random.Next(1000000, 9999999)}";
            string cellId = $"{_random.Next(10)}cell";

            // Act
            UpsertCellRequest request = new() { Value = "expression"};
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
            UpsertCellRequest request = new() { Value = "expression"};
            HttpResponseMessage response = await _client.PostAsJsonAsync($"/api/v1/{sheetId}/{cellId}", request);
            ErrorResponse result = await response.Content.ReadFromJsonAsync<ErrorResponse>();

            // Assert
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
            result.Code.ShouldBe(ErrorCode.InvalidCellId);
            result.Message.ShouldBe($"CellId cannot contains '{sign}' sign");
        }
    }
}