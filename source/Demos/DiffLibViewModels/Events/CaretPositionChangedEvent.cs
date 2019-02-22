namespace DiffLibViewModels.Events
{
    using System;

    public enum CaretChangeType
    {
        ColumnAndLine,
        Column,
        Line,
    }

    public class CaretPositionChangedEvent : EventArgs
    {
        /// <summary>
        /// class constructor
        /// </summary>
        /// <param name="line"></param>
        /// <param name="column"></param>
        public CaretPositionChangedEvent(int line, int column, CaretChangeType changeType)
        {
            ChangeType = changeType;
            Column = column;
            Line = line;
        }

        public CaretChangeType ChangeType { get; }

        /// <summary>
        /// Gets the X coordinate at which the carret was seen last.
        /// </summary>
        public int Column { get; }

        /// <summary>
        /// Gets the Y coordinate at which the carret was seen last.
        /// </summary>
        public int Line { get; }
    }
}
