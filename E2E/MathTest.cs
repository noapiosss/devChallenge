using System;
using System.Net.Http;
using System.Net.Http.Json;
using Contracts.Http;
using E2E.Data;
using Shouldly;

namespace E2E
{
    public class MathTest
    {
        private readonly HttpClient _client;
        private readonly Random _random;

        public MathTest()
        {
            _client = new()
            {
                BaseAddress = new Uri("http://localhost:8080")
            };

            _random = new();
        }

        [Theory]
        [ClassData(typeof(ValidMathExpressions))]
        public async void ShouldEvaluate(string expression, string expressionResult)
        {
            // Arrange
            string sheetId = $"sheet{_random.Next(1000000, 9999999)}";
            string cellId = $"cell{_random.Next(1000000, 9999999)}";
            double delta = 0.01;

            // Act
            UpsertCellRequest request = new() { Value = expression};
            HttpResponseMessage response = await _client.PostAsJsonAsync($"/api/v1/{sheetId}/{cellId}", request);
            GetCellResponse result = await response.Content.ReadFromJsonAsync<GetCellResponse>();

            // Assert
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Created);
            Math.Abs(double.Parse(result.Result) - double.Parse(expressionResult)).ShouldBeLessThan(delta);

        }

        [Theory]
        [ClassData(typeof(NotValidMathExpressions))]
        public async void ShouldNotEvaluate(string expression)
        {
            // Arrange
            string sheetId = $"sheet{_random.Next(1000000, 9999999)}";
            string cellId = $"cell{_random.Next(1000000, 9999999)}";

            // Act
            UpsertCellRequest request = new() { Value = expression};
            HttpResponseMessage response = await _client.PostAsJsonAsync($"/api/v1/{sheetId}/{cellId}", request);
            GetCellResponse result = await response.Content.ReadFromJsonAsync<GetCellResponse>();

            // Assert
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.UnprocessableEntity);
            result.Result.ShouldBe("ERROR");
        }
    }
}