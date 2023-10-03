using System;
using System.Net.Http;
using System.Net.Http.Json;
using Contracts.Http;
using E2E.Data;
using E2E.Data.Contracts;
using E2E.Tests.Base;
using Shouldly;
using Xunit.Abstractions;

namespace E2E.Tests
{
    public class MathTest : BaseTest
    {
        public MathTest(ITestOutputHelper output) : base(output)
        {
            
        }

        [Theory]
        [ClassData(typeof(ValidMathExpressions))]
        public async void ShouldEvaluate(params CellDataWithStatusCode[] cells)
        {
            // Arrange
            string sheetId = $"sheet{_random.Next(1000000, 9999999)}";
            double delta = 0.01;

            foreach (CellDataWithStatusCode cell in cells)
            {
                // Act
                UpsertCellRequest request = new() { Value = cell.Value };
                HttpResponseMessage response = await _client.PostAsJsonAsync($"/api/v1/{sheetId}/{cell.CellId}", request);
                GetCellResponse result = await response.Content.ReadFromJsonAsync<GetCellResponse>();

                // Assert
                LogCell(cell);
                LogCell(cell.CellId, result.Value, result.Result, response.StatusCode);

                response.StatusCode.ShouldBe(cell.StatusCode);
                result.Value.ShouldBe(cell.Value);
                Math.Abs(double.Parse(result.Result) - double.Parse(cell.Result)).ShouldBeLessThan(delta);
            }

        }

        [Theory]
        [ClassData(typeof(NotValidMathExpressions))]
        public async void ShouldNotEvaluate(params CellDataWithStatusCode[] cells)
        {
            // Arrange
            string sheetId = $"sheet{_random.Next(1000000, 9999999)}";

            foreach (CellDataWithStatusCode cell in cells)
            {
                // Act
                UpsertCellRequest request = new() { Value = cell.Value };
                HttpResponseMessage response = await _client.PostAsJsonAsync($"/api/v1/{sheetId}/{cell.CellId}", request);
                GetCellResponse result = await response.Content.ReadFromJsonAsync<GetCellResponse>();

                // Assert
                response.StatusCode.ShouldBe(cell.StatusCode);
                result.Value.ShouldBe(cell.Value);
                result.Result.ShouldBe(cell.Result);
            }
        }
    }
}