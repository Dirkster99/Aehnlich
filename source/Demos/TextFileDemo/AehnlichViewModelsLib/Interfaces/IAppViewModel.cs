namespace AehnlichViewModelsLib.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.Windows.Input;
    using AehnlichViewLib.Enums;
    using AehnlichViewModelsLib.Enums;
    using AehnlichViewModelsLib.Interfaces;
    using AehnlichViewModelsLib.ViewModels.Suggest;

    /// <summary>
    /// Defines the interface for the viewmodel that implements the main application viewmodel.
    /// </summary>
    public interface IAppViewModel : IDisposable, INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the document viewmodel that manages left and right viewmodel
        /// which drive the synchronized diff text view.
        /// </summary>
        IDiffDocViewModel DiffCtrl { get; }

        /// <summary>
        /// Gets the path of file A in the comparison.
        /// </summary>
        ISuggestSourceViewModel FilePathA { get; }

        /// <summary>
        /// Gets the path of file B in the comparison.
        /// </summary>
        ISuggestSourceViewModel FilePathB { get; }

        /// <summary>
        /// Gets a focus element indicator to indicate a ui element to focus
        /// (this is used to focus the lift diff view by default when loading new files)
        /// </summary>
        Focus FocusControl { get; }

        /// <summary>
        /// Gets view model that drives the Goto Line inline dialog view.
        /// </summary>
        IGotoLineControllerViewModel GotoLineController { get; }

        /// <summary>
        /// Gets/sets the current inline dialog mode.
        /// </summary>
        InlineDialogMode InlineDialog { get; set; }

        /// <summary>
        /// Gets the number of text lines currently visible in the text view
        /// of the left view A and the right view B.
        /// </summary>
        int NumberOfTextLinesInViewPort { get; }

        /// <summary>
        /// Gets/sets the current overview value shown in a seperate overview diff control.
        /// This control is similar to a scrollbar - it can be used to scroll to a certain
        /// postion - but its thumb and color background also indicates the current cursor
        /// line within a birds eye view.
        /// </summary>
        double OverViewValue { get; set; }

        #region Commands
        /// <summary>
        /// Gets a command that refreshs (reloads) the comparison of 2 textfiles.
        /// </summary>
        ICommand CompareFilesCommand { get; }

        /// <summary>
        /// Gets a command that copies the currently selected text into the Windows Clipboard.
        /// </summary>
        ICommand CopyTextSelectionFromActiveViewCommand { get; }

        /// <summary>
        /// Implements a find command via AvalonEdits build in search panel which can be
        /// activated if the right or left control has focus.
        /// </summary>
        ICommand FindTextCommand { get; }

        /// <summary>
        /// Gets a command to scroll the view to a certain line within the available lines.
        /// </summary>
        ICommand GotoLineCommand { get; }

        /// <summary>
        /// Gets a command that should be invoked:
        /// - when either of the synchronized left or right text diff view changes its display line or 
        /// - when the size (width and/or height) of the text diff view changes
        /// 
        /// to sync with the Overview control and update parts of the application.
        /// </summary>
        ICommand ViewPortChangedCommand { get; }

        /// <summary>
        /// Gets a command that opens the currently active file in Windows.
        /// </summary>
        ICommand OpenFileFromActiveViewCommand { get; }
        #endregion Commands
    }
}