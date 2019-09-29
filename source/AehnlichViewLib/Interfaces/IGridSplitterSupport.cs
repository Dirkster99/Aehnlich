namespace AehnlichViewLib.Interfaces
{
    using AehnlichViewLib.Events;
    using System;
    using System.Windows;

    /// <summary>
    /// Defines properties and events of a class that supports a vertical split view
    /// using a GridSplitter that can be extended in the client application of a control
    /// that implements this interface.
    /// </summary>
    public interface IGridSplitterSupport
    {
        /// <summary>
        /// Implements an event that is invoked when the column view with columns A and B
        /// (PART_GridA and PART_GridB separated by a GridSplitter) is being
        /// resized in the way that the visible proportional width has been changed.
        /// </summary>
        event EventHandler<ColumnWidthChangedEvent> ColumnWidthChanged;

        #region Column A B GridSplitter Synchronization
        /// <summary>
        /// Gets the width of column A in a view with columns A and B being separated by a GridSplitter.
        /// </summary>
        GridLength ColumnWidthA
        {
            get; set;
        }

        /// <summary>
        /// Gets the width of column B in a view with columns A and B being separated by a GridSplitter.
        /// </summary>
        GridLength ColumnWidthB
        {
            get; set;
        }
        #endregion Column A B GridSplitter Synchronization
    }
}