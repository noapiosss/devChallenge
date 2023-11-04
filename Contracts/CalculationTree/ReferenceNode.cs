using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Numerics;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Contracts.CalculationTree
{
    public class ReferenceNode : Node
    {
        private string _url;

        public ReferenceNode(string url)
        {
            _url = url;
        }

        public override async Task<string> Evaluate()
        {
            using HttpClient client = new()
            {
                BaseAddress = new Uri(_url)
            };

            HttpResponseMessage response = await client.GetAsync("");
            JObject responseObject = JObject.Parse(await response.Content.ReadAsStringAsync());
            return responseObject.SelectToken("result").ToString();
        }

        public override ICollection<string> GetNodeVariables()
        {
            return new List<string>();
        }

        public override Node ReplaceVariable(string variableName, Node node)
        {
            throw new InvalidOperationException("cannot raplce url");
        }
    }
}