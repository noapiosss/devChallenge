using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Api.Services.Interfaces;
using Contracts.Event;
using Contracts.Http;
using Domain.Event;

namespace Api.Services
{
    public class CellNotificationHandler : ICellNotificationHandler
    {
        private readonly Dictionary<CellSubscription, List<string>> _subscribers;
        private readonly CellChangedEvent _cellChangedEvent;

        public CellNotificationHandler(CellChangedEvent cellChangedEvent)
        {
            _subscribers = new();
            _cellChangedEvent = cellChangedEvent;
            _cellChangedEvent.CellWasChanged += CellChanged_Rised;
        }

        public void Subscribe(string sheetId, string cellId, string result, string webhook)
        {
            CellSubscription subscription = new()
            {
                SheetId = sheetId,
                CellId = cellId,
                PrevResult = result
            };

            if (_subscribers.ContainsKey(subscription))
            {
                _subscribers[subscription].Add(webhook);
            }
            else
            {
                _subscribers.Add(subscription, new() { webhook });
            }
        }

        private void CellChanged_Rised(object sender, CellChangedEventArgs args)
        {
            CellSubscription changedCell = new()
            {
                SheetId = args.SheetId,
                CellId = args.CellId
            };

            if (_subscribers.ContainsKey(changedCell) && _subscribers.Keys.First(k => k.Equals(changedCell)).PrevResult != args.Result)
            {
                _subscribers.Keys.First(k => k.Equals(changedCell)).PrevResult = args.Result;                

                GetCellResponse notification = new()
                {
                    Value = args.Value,
                    Result = args.Result
                };

                foreach (string webhook in _subscribers[changedCell])
                {
                    using HttpClient client = new()
                    {
                        BaseAddress = new Uri(webhook)
                    };
                    client.DefaultRequestHeaders.Add("Accept", "application/json");

                    string body = Newtonsoft.Json.JsonConvert.SerializeObject(new
                    {
                        value = args.Value,
                        result = args.Result
                    });
                    StringContent content = new StringContent(body, Encoding.UTF8, "application/json");

                    client.PostAsync("", content);
                }
            }
        }
    }
}