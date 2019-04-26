namespace AehnlichViewModelsLib.Events
{
    using System;

    /// <summary>
    /// Indicates the type of change when comunicating caret changes
    /// in a raised event.
    /// </summary>
    public enum CaretChangeType
    {
        /// <summary>
        /// The line and column have been changed.
        /// </summary>
        ColumnAndLine,

        /// <summary>
        /// Only the column has changed.
        /// </summary>
        Column,

        /// <summary>
        /// Only the line has changed.
        /// </summary>
        Line,
    }

    /// <summary>
    /// Implements an event to indicate a change in the position
    /// of a text caret within a text editor.
    /// </summary>
    public class CaretPositionChangedEvent : EventArgs
    {
        /// <summary>
        /// class constructor
        /// </summary>
        /// <param name="line"></param>
        /// <param name="column"></param>
        /// <param name="changeType"></param>
        public CaretPositionChangedEvent(int line, int column, CaretChangeType changeType)
        {
            ChangeType = changeType;
            Column = column;
            Line = line;
        }

        /// <summary>
        /// Gets the type of change in caret position this event is indicating.
        /// </summary>
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
