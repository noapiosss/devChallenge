using System.Net;
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
    public class LinkedCellsTest : BaseTest
    {
        public LinkedCellsTest(ITestOutputHelper output) : base(output)
        {

        }

        [Theory]
        [ClassData(typeof(BasicLInkedCells))]
        public async void BasicLinkedCellsShouldBeCreated(params CellData[] cells)
        {
            // Arrange            
            string sheetId = $"sheet{_random.Next(1000000, 9999999)}";

            
            foreach(CellData cell in cells)
            {
                // Act
                UpsertCellRequest request = new() { Value = cell.Value };
                HttpResponseMessage response = await _client.PostAsJsonAsync($"/api/v1/{sheetId}/{cell.CellId}", request);
                GetCellResponse result = await response.Content.ReadFromJsonAsync<GetCellResponse>();

                // Assert
                response.StatusCode.ShouldBe(HttpStatusCode.Created);
                result.Value.ShouldBe(cell.Value);
                result.Result.ShouldBe(cell.Result);
            }

        }

        [Theory]
        [ClassData(typeof(BasicLinkedCellsBreakers))]
        public async void CellShouldNotBeUpdated(params CellDataWithStatusCode[] cells)
        {
            // Arrange
            string sheetId = $"sheet{_random.Next(1000000, 9999999)}";

            foreach(CellDataWithStatusCode cell in cells)
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

        [Theory]
        [ClassData(typeof(EmptyStringLinking))]
        public async void EmptyCellShouldBeEqualZeroInMath(params CellData[] cells)
        {
            // Arrange
            string sheetId = $"sheet{_random.Next(1000000, 9999999)}";

            // Act
            foreach(CellData cell in cells)
            {
                // Act
                UpsertCellRequest request = new() { Value = cell.Value };
                HttpResponseMessage response = await _client.PostAsJsonAsync($"/api/v1/{sheetId}/{cell.CellId}", request);
                GetCellResponse result = await response.Content.ReadFromJsonAsync<GetCellResponse>();

                // Assert
                response.StatusCode.ShouldBe(HttpStatusCode.Created);
                result.Value.ShouldBe(cell.Value);
                result.Result.ShouldBe(cell.Result);
            }
        }
    }
}