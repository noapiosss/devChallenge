using System;
using System.Globalization;
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
    public class ReferencesTest : BaseTest
    {
        public ReferencesTest(ITestOutputHelper output) : base(output)
        {
            
        }

        [Theory]
        [ClassData(typeof(ValidReferencesFunctions))]
        public async void ShouldEvaluateReferencesFunctions(params SheedCellDataWithStatusCode[] cells)
        {
            // Arrange

            foreach (SheedCellDataWithStatusCode cell in cells)
            {
                // Act
                UpsertCellRequest request = new() { Value = cell.Value };
                HttpResponseMessage response = await _client.PostAsJsonAsync($"/api/v1/{cell.SheetId}/{cell.CellId}", request);
                GetCellResponse result = await response.Content.ReadFromJsonAsync<GetCellResponse>();

                // Assert
                response.StatusCode.ShouldBe(cell.StatusCode);
                result.Value.ShouldBe(cell.Value);
                result.Result.ShouldBe(cell.Result);
            }
        }

        [Theory]
        [ClassData(typeof(InvalidReferencesFunctions))]
        public async void ShouldNotEvaluateInvalidReferences(params SheedCellDataWithStatusCode[] cells)
        {
            // Arrange

            foreach (SheedCellDataWithStatusCode cell in cells)
            {
                // Act
                UpsertCellRequest request = new() { Value = cell.Value };
                HttpResponseMessage response = await _client.PostAsJsonAsync($"/api/v1/{cell.SheetId}/{cell.CellId}", request);
                GetCellResponse result = await response.Content.ReadFromJsonAsync<GetCellResponse>();

                // Assert
                response.StatusCode.ShouldBe(cell.StatusCode);
                result.Value.ShouldBe(cell.Value);
                result.Result.ShouldBe(cell.Result);
            }
        }
    }
}