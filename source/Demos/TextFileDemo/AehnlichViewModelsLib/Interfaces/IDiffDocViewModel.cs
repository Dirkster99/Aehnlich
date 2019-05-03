namespace AehnlichViewModelsLib.Interfaces
{
    using AehnlichLib.Enums;
    using AehnlichLib.Interfaces;
    using AehnlichViewModelsLib.ViewModels;
    using ICSharpCode.AvalonEdit;
    using System;
    using System.ComponentModel;
    using System.Windows.Input;

    /// <summary>
    /// Implements a viewmodel that services both views viewA and viewB (left and right).
    ///
    /// Defines the properties and methods of a viewmodel document that displays diff
    /// information using a synchronized
    /// - <see cref="ViewA"/> (left view) and
    /// - <see cref="ViewB"/> (left view)
    /// 
    /// with additional highlighting information.
    /// </summary>
    public interface IDiffDocViewModel : IDisposable, INotifyPropertyChanged
    {
        #region properties
        /// <summary>
        /// Gets the text editor display options that control the left and right text diff view.
        /// Both diff views are bound to one options object to ensure consistent displays.
        /// </summary>
        TextEditorOptions DiffViewOptions { get; }

        /// <summary>
        /// Gets the viemodel that represents the left side of the diff view.
        /// </summary>
        IDiffSideViewModel ViewA { get; }

        /// <summary>
        /// Gets the viemodel that represents the right side of the diff view.
        /// </summary>
        IDiffSideViewModel ViewB { get; }

        /// <summary>
        /// Gets whether both viewmodels ViewA or ViewB hold more than
        /// no line to compare (enabling comparison functions makes no sense if this is false).
        /// </summary>
        bool IsDiffDataAvailable { get; }

        #region Synchronized Caret Position
        /// <summary>
        /// Gets/sets the caret positions column from the last time when the
        /// caret position in the left view has been synchronzied with the right view (or vice versa).
        /// </summary>
        int SynchronizedColumn { get; set; }

        /// <summary>
        /// Gets/sets the caret positions line from the last time when the
        /// caret position in the left view has been synchronzied with the right view (or vice versa).
        /// </summary>
        int SynchronizedLine { get; set; }
        #endregion Synchronized Caret Position

        /// <summary>
        /// Gets the similarity value (0% - 100%) between 2 as formated text things
        /// to be shown as tooltip in toolbar.
        /// </summary>
        string Similarity_Text { get; }

        #region Left and Right File Name Labels
        /// <summary>
        /// Gets whether left and right file name labels over each ViewA and ViewB
        /// are visible or not.
        /// </summary>
        bool edtLeft_Right_Visible { get; }

        /// <summary>
        /// Gets the left text label (file name) displayed over the left diff view (ViewA).
        /// </summary>
        string edtLeft_Text { get; }

        /// <summary>
        /// Gets the right text label (file name) displayed over the right diff view (ViewA).
        /// </summary>
        string edtRight_Text { get; }
        #endregion Left and Right File Name Labels

        /// <summary>
        /// Gets the <see cref="IDiffSideViewModel"/> that drives the diff view
        /// that contains always exactly 2 lines.
        /// 
        /// This diff view shows the currently selected line from the left view A
        /// and the right view B on top of each other.
        /// </summary>
        IDiffSideViewModel ViewLineDiff { get; }

        /// <summary>
        /// Gets/sets the height of the bottom panel view that shows diff
        /// of the currently selected line with a 2 line view.
        /// </summary>
        int LineDiffHeight { get; }

        #region Goto Diff Commands
        /// <summary>
        /// Gets a command that positions the diff viewer at the first detected difference.
        /// </summary>
        ICommand GoToFirstDifferenceCommand { get; }

        /// <summary>
        /// Gets a command that positions the diff viewer at the next detected difference.
        /// </summary>
        ICommand GoToNextDifferenceCommand { get; }

        /// <summary>
        /// Gets a command that positions the diff viewer at a previously detected difference.
        /// </summary>
        ICommand GoToPrevDifferenceCommand { get; }

        /// <summary>
        /// Gets a command that positions the diff viewer at the last detected difference.
        /// </summary>
        ICommand GoToLastDifferenceCommand { get; }
        #endregion Goto Diff Commands

        /// <summary>
        /// Gets the total number of labeled lines that are visible in the left view.
        /// This count DOES NOT include imaginary lines that may have
        /// been inserted to bring both texts into a synchronized
        /// view.
        /// 
        /// This count applies therfore, to the maximum number of LABELED
        /// lines in the left side view and is equal to the number of lines
        /// number of lines in the original text.
        /// 
        /// This number is used to instruct the Goto Line dialog to jump to
        /// a particular line and left and right side the view synchronization
        /// ensures scrolling to the correct line even if that line label is different
        /// in the right text view.
        /// </summary>
        uint NumberOfLines { get; }

        /// <summary>
        /// Gets the total number of lines available in left and right text view.
        /// 
        /// This number includes Imaginary lines and is always applicable to the
        /// left and right view since both view displays are synchronized via Meyers Diff.
        /// </summary>
        uint MaxNumberOfLines { get; }

        /// <summary>
        /// Gets the type of the items being compared
        /// <see cref="DiffType.File"/>, <see cref="DiffType.Text"/> or <see cref="DiffType.Directory"/>
        /// </summary>
        DiffType DiffType { get; }

        /// <summary>
        /// Gets a string that contains the file name and path of the left and right view
        /// (each in a seperate line) formated in one string.
        /// 
        /// This information can be used as tool tip or other descriptive text in the UI.
        /// </summary>
        string ToolTipText { get; }

        /// <summary>
        /// Gets the current status of the viewmodel formated as string for
        /// display in tooltip or statusbar or such.
        /// </summary>
        string StatusText { get; }

        /// <summary>
        /// Gets the number of inserts visible in the current view.
        /// </summary>
        int CountInserts { get; }

        /// <summary>
        /// Gets the number of deletes visible in the current view.
        /// </summary>
        int CountDeletes { get; }

        /// <summary>
        /// Gets the number of changes visible in the current view.
        /// </summary>
        int CountChanges { get; }
        #endregion properties

        #region methods
        /// <summary>
        /// Gets the view (of the 2 side by side views) that was activated last
        /// (had the focus the last time)
        /// </summary>
        /// <returns></returns>
        IDiffSideViewModel GetActiveView(out IDiffSideViewModel nonActiveView);

        /// <summary>
        /// Navigates both views A (right) and B (left) to line number n.
        /// </summary>
        /// <param name="thisLine"></param>
        void GotoTextLine(uint thisLine);

        /// <summary>
        /// Moves both views to the requested line position.
        /// </summary>
        /// <param name="gotoPos"></param>
        /// <param name="viewA"></param>
        /// <param name="viewB"></param>
        /// <param name="positionCursor"></param>
        void ScrollToLine(IDiffViewPosition gotoPos,
                          IDiffSideViewModel viewA,
                          IDiffSideViewModel viewB,
                          bool positionCursor);
        #endregion methods
    }
}
