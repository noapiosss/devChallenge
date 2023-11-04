using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Contracts.Event;
using Contracts.Http;
using Domain.Event;

namespace Api.Services.Interfaces
{
    public interface ICellNotificationHandler
    {
        public void Subscribe(string sheetId, string cellId, string result, string webhook);
    }
}