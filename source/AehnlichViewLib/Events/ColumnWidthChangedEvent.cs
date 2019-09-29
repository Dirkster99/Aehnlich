namespace AehnlichViewLib.Events
{
    using System;
    using System.Windows;

    public class ColumnWidthChangedEvent : EventArgs
    {
        public ColumnWidthChangedEvent(GridLength columnWidthA, GridLength columnWidthB)
        {
            ColumnWidthA = new GridLength(columnWidthA.Value, columnWidthA.GridUnitType);
            ColumnWidthB = new GridLength(columnWidthB.Value, columnWidthB.GridUnitType);
        }

        public GridLength ColumnWidthA { get; }

        public GridLength ColumnWidthB { get; }
    }
}
