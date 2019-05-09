namespace AehnlichViewLib.Interfaces
{
    using AehnlichViewLib.Controls.AvalonEditEx;
    using ICSharpCode.AvalonEdit.Document;
    using System;
    using System.Collections;
    using System.ComponentModel;

    /// <summary>
    /// 
    /// text editor control.
    /// </summary>
    public interface IDiffView : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets/sets the <see cref="TextDocument"/> viewmodel of the attached AvalonEdit
        /// text editor control.
        /// </summary>
        TextDocument Document   { get; set; }

        /// <summary>
        /// Gets a source of items that can be used to populate marker elements
        /// on the overview bar. This should ideally be an ObservableCollection{T} or
        /// at least an <see cref="IList{T}"/> where T is a <see cref="DiffContext"/>
        /// for a particular line in the merged text views.
        /// </summary>
//        IEnumerable ItemsSource { get; }
        
        /// <summary>
        /// Gets/sets whether the currently shown text in the textedior has been changed
        /// without saving or not.
        /// </summary>
        bool IsDirty            { get; set; }

        /// <summary>
        /// Gets whether the diff view control is enabled or not.
        /// </summary>
        bool IsEnabled          { get; }
        
        /// <summary>
        /// Gets whether line numbers should be shown in the diff view control or not.
        /// </summary>
        bool ShowLineNumbers    { get; }
        
        /// <summary>
        /// Gets whether the text displayed in the diff should be editable or not.
        /// </summary>
        bool IsReadOnly         { get; }                             

        #region Caret Position
        /// <summary>
        /// Gets/sets the column of a display position.
        /// </summary>
        int Column { get; set; }

        /// <summary>
        /// Gets/sets the line of a display position.
        /// </summary>
        int Line { get; set; }
        #endregion Caret Position
        
        /// <summary>
        /// Gets/sets the textbox controller that is used to drive the view
        /// from within the viewmodel (with event based commands like goto line x,y).
        /// </summary>
        TextBoxController TxtControl { get; }
        
        /// <summary>
        /// Gets/set the time stamp of the last time when the attached view
        /// has been activated (GotFocus).
        /// </summary>
        DateTime ViewActivation { get; set; }

        /// <summary>
        /// Gets/sets the linediff data provider which implements an <see cref="ILineDiffProvider"/>
        /// interface to compute textual line diffs on demand (when lines are scrolled into view).
        /// </summary>
        ///ILineDiffProvider LineDiffDataProvider { get; }
    }
}
