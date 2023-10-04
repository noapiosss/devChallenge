using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using E2E.Data.Contracts;
using Newtonsoft.Json;
using Xunit.Abstractions;

namespace E2E.Tests.Base
{
    public abstract class BaseTest
    {
        protected readonly HttpClient _client;
        protected readonly Random _random;
        protected readonly ITestOutputHelper _output;

        public BaseTest(ITestOutputHelper output)
        {
            _client = new()
            {
                BaseAddress = new Uri(Environment.GetEnvironmentVariable("API_URL") ?? "http://localhost:8080")
            };

            _random = new();
            _output = output;
        }

        protected void Log(string message)
        {
            _output.WriteLine(message);
        }

        protected void LogCell(CellData cell)
        {
            _output.WriteLine(JsonConvert.SerializeObject(cell));
        }

        protected void LogCell(CellDataWithStatusCode cell)
        {
            _output.WriteLine(JsonConvert.SerializeObject(cell));
        }

        protected void LogCell(string cellId, string value, string result)
        {
            _output.WriteLine(JsonConvert.SerializeObject(new CellData(cellId, value, result)));
        }    

        protected void LogCell(string cellId, string value, string result, HttpStatusCode code)
        {
            _output.WriteLine(JsonConvert.SerializeObject(new CellDataWithStatusCode(cellId, value, result, code)));
        }
    }
}