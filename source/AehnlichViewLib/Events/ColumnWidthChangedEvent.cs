namespace AehnlichViewLib.Events
{
    using System;
    using System.Windows;

    public class ColumnWidthChangedEvent : EventArgs
    {
        public ColumnWidthChangedEvent(GridLength columnWidthA, GridLength columnWidthB)
        {
            ColumnWidthA = columnWidthA;
            ColumnWidthB = columnWidthB;
        }

        public GridLength ColumnWidthA { get; }

        public GridLength ColumnWidthB { get; }
    }
}
