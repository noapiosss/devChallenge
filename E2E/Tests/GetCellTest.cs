using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Contracts.Http;
using E2E.Data;
using E2E.Data.Contracts;
using E2E.Tests.Base;
using Shouldly;
using Xunit.Abstractions;

namespace E2E.Tests
{
    public class GetCellTest : BaseTest
    {
        public GetCellTest(ITestOutputHelper output) : base(output)
        {

        }

        [Theory]
        [ClassData(typeof(OnlyValidCells))]
        public async void OnlyValidCells(params List<CellDataWithStatusCode>[] cells)
        {
            // Arrange
            string sheetId = $"sheet{_random.Next(1000000, 9999999)}";

            await UpsertCellsAsync(sheetId, cells[0]);
            await GetCellsAsync(sheetId, cells[1]);
        }

        [Theory]
        [ClassData(typeof(ValidAndInvalidCellsCells))]
        public async void ValidAndInvalidCellsCells(params List<CellDataWithStatusCode>[] cells)
        {
            // Arrange
            string sheetId = $"sheet{_random.Next(1000000, 9999999)}";

            await UpsertCellsAsync(sheetId, cells[0]);
            await GetCellsAsync(sheetId, cells[1]);
        }

        [Theory]
        [ClassData(typeof(ValidCellsWithRecuresion))]
        public async void ValidCellsWithRecuresion(params List<CellDataWithStatusCode>[] cells)
        {
            // Arrange
            string sheetId = $"sheet{_random.Next(1000000, 9999999)}";

            await UpsertCellsAsync(sheetId, cells[0]);
            await GetCellsAsync(sheetId, cells[1]);
        }

        [Theory]
        [ClassData(typeof(ValidCellsWithRecuresionWithUpdateAtBegining))]
        public async void ValidCellsWithRecuresionWithUpdateAtBegining(params List<CellDataWithStatusCode>[] cells)
        {
            // Arrange
            string sheetId = $"sheet{_random.Next(1000000, 9999999)}";

            await UpsertCellsAsync(sheetId, cells[0]);
            await GetCellsAsync(sheetId, cells[1]);
        }

        private async Task UpsertCellsAsync(string sheetId, List<CellDataWithStatusCode> cells)
        {
            for (int i = 0; i < cells.Count; ++i)
            {
                // Act
                UpsertCellRequest request = new() { Value = cells[i].Value };
                HttpResponseMessage response = await _client.PostAsJsonAsync($"/api/v1/{sheetId}/{cells[i].CellId}", request);
                GetCellResponse result = await response.Content.ReadFromJsonAsync<GetCellResponse>();

                // Assert
                response.StatusCode.ShouldBe(cells[i].StatusCode);
                result.Value.ShouldBe(cells[i].Value);
                result.Result.ShouldBe(cells[i].Result);
            }
        }        

        private async Task GetCellsAsync(string sheetId, List<CellDataWithStatusCode> cells)
        {            
            for (int i = 0; i < cells.Count; ++i)
            {
                // Act
                HttpResponseMessage response = await _client.GetAsync($"/api/v1/{sheetId}/{cells[i].CellId}");
                GetCellResponse result = await response.Content.ReadFromJsonAsync<GetCellResponse>();

                // Assert
                response.StatusCode.ShouldBe(cells[i].StatusCode);
                if (response.StatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    result.Value.ShouldBe(cells[i].Value);
                    result.Result.ShouldBe(cells[i].Result);
                }
            }
        }
    }
}