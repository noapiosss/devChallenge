using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Contracts.Event
{
    public class CellWebhook
    {
        public string PrevResult { get; set; }
        public string WebHook { get; set; }
    }
}