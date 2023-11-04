using System;

namespace Domain.Event
{
    //public delegate void CellChangedHandler(object obj, CellChangedEventArgs e);

    public class CellChangedEventArgs : EventArgs
    {
        public string SheetId { get; init; }
        public string CellId { get; init; }
        public string Value { get; init; }
        public string Result { get; set; }
    }

    public class CellChangedEvent
    {
        public event EventHandler<CellChangedEventArgs> CellWasChanged;

        public void RiseEvent(CellChangedEventArgs args)
        {
            CellWasChanged?.Invoke(this, args);
        }
    }
}