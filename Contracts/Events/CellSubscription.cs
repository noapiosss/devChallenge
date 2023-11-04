using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Contracts.Event
{
    public class CellSubscription
    {
        public string SheetId { get; set; }
        public string CellId { get; set;}
        public string PrevResult { get; set; }

        public override int GetHashCode()
        {
            return CellId.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is CellSubscription other)
            {
                return SheetId == other.SheetId && CellId == other.CellId;
            }
            return false;
        }
    }
}