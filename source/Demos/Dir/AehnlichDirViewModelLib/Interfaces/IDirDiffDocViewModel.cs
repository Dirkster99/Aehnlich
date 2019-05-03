namespace AehnlichDirViewModelLib.Interfaces
{
    using AehnlichDirViewModelLib.Events;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows.Input;

    /// <summary>
    /// defines the properties for a for a side-by-side viewmodel that drives a
    /// view of left and right directory content lists.
    /// </summary>
    public interface IDirDiffDocViewModel : INotifyPropertyChanged
    {

        /// <summary>
        /// Requests a listner to handle the event when the user wants to open a detailed
        /// file diff view to compare the contents of 2 (text or binary) files.
        /// </summary>
        event EventHandler<OpenFileDiffEventArgs> CompareFilesRequest;

        #region properties
        /// <summary>
        /// Gets whether data binding should currently result in Diff data being available or not.
        /// This can be a good indicator for command enable/disablement when these make sense
        /// with the required data only.
        /// </summary>
        bool IsDiffDataAvailable { get; }

        /// <summary>
        /// Gets a list of viewmodel entries that lists the contents of the left ViewA
        /// in a synchronized way (with imaginary lines) to the right ViewB.
        /// </summary>
        IReadOnlyList<IDirEntryViewModel> DirEntries { get; }

        /// <summary>
        /// Gets the file system path for the contents displayed in left ViewA.
        /// </summary>
        string PathA { get; }

        /// <summary>
        /// Gets the file system path for the contents displayed in right ViewA.
        /// </summary>
        string PathB { get; }

        /// <summary>
        /// Gets/set the time stamp of the last time when the attached view
        /// has been activated (GotFocus).
        /// </summary>
        DateTime ViewActivation_A { get; }

        /// <summary>
        /// Gets/set the time stamp of the last time when the attached view
        /// has been activated (GotFocus).
        /// </summary>
        DateTime ViewActivation_B { get; set; }

        /// <summary>
        /// Gets/sets the single selected item in the left ViewA.
        /// </summary>
        object SelectedItem_A { get; set; }

        /// <summary>
        /// Gets/sets the single selected item in the right ViewB.
        /// </summary>
        object SelectedItem_B { get; set; }

        /// <summary>
        /// Gets a string that describes the currently applied filter criteria for display in UI.
        /// </summary>
        string LblFilter { get; }

        /// <summary>
        /// Get set of currently selected items (when multiple items are selected).
        /// </summary>
        ObservableCollection<IDirEntryViewModel> SelectedItemsA { get; }

        /// <summary>
        /// Get set of currently selected items (when multiple items are selected).
        /// </summary>
        ObservableCollection<IDirEntryViewModel> SelectedItemsB { get; }

        /// <summary>
        /// Gets the number of files that have been deleted in B
        /// when comparing a set of files between A and B.
        /// </summary>
        int CountFilesDeleted { get; }

        /// <summary>
        /// Gets the number of files that have been added in B
        /// when comparing a set of files between A and B.
        /// </summary>
        int CountFilesAdded { get; }

        /// <summary>
        /// Gets the number of files that have been changed in B or A
        /// when comparing a set of files between A and B.
        /// </summary>
        int CountFilesChanged { get; }

        #region Commands
        /// <summary>
        /// Gets a command to browse the current directory diff view by one level down
        /// (if there is a current view and a remaining level down is available).
        /// </summary>
        ICommand BrowseItemCommand { get; }

        /// <summary>
        /// Gets a command to browse the current directory diff view by one level
        /// up in the directory hierarchy
        /// (if there is a current view and a remaining level up is available).
        /// </summary>
        ICommand BrowseUpCommand { get; }

        /// <summary>
        /// Gets a command that copies the path from all selected item(s)
        /// in the left view A into the Windows clipboard.
        /// </summary>
        ICommand CopyPathAToClipboardCommand { get; }

        /// <summary>
        /// Gets a command that copies the path from all selected item(s)
        /// in the right view B into the Windows clipboard.
        /// </summary>
        ICommand CopyPathBToClipboardCommand { get; }

        /// <summary>
        /// Gets a command to open the folder in which the
        /// currently (single) selected item (if any) is stored.
        /// </summary>
        ICommand OpenContainingFolderCommand { get; }

        /// <summary>
        /// Gets a command to execute the associate Windows program for the
        /// currently (single) selected item (if any).
        /// </summary>
        ICommand OpenInWindowsCommand { get; }

        /// <summary>
        /// Gets a command to execute the associate Windows program for the
        /// currently (single) selected item (if any) from the last avtive view.
        /// 
        /// This command is intended for binding via UI elements such as Menu, Toolbar
        /// etc. which are not directly part of the DiffDir control/viewmodel structure.
        /// </summary>
        ICommand OpenFileFromActiveViewCommand { get; }
        #endregion Commands
        #endregion properties
    }
}
