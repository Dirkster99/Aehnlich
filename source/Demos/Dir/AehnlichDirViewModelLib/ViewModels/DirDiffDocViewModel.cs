namespace AehnlichDirViewModelLib.ViewModels
{
    using AehnlichDirViewModelLib.Models;
    using AehnlichDirViewModelLib.ViewModels.Base;
    using AehnlichLib.Dir;
    using AehnlichLib.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Windows.Input;

    public class DirDiffDocViewModel : Base.ViewModelBase
    {
        #region fields
        private ShowDirDiffArgs _CompareOptions;
        private string _LblFilter;
        private string _PathB, _PathA;
        private DateTime _ViewActivation_A, _ViewActivation_B;
        private object _SelectedItem_A, _SelectedItem_B;

        private bool _IsDiffDataAvailable;
        private IDirectoryDiffRoot _Results;

        private ICommand _BrowseItemCommand;
        private ICommand _BrowseUpCommand;
        private ICommand _CopyPathToClipboardCommand;
        private ICommand _OpenContainingFolderCommand;
        private ICommand _OpenInWindowsCommand;
        private ICommand _OpenFileFromActiveViewCommand;

        private readonly ObservableRangeCollection<DirEntryViewModel> _DirEntries;
        private readonly Stack<DirEntryViewModel> _DirPathStack;
        #endregion fields

        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        public DirDiffDocViewModel()
        {
            _DirEntries = new ObservableRangeCollection<DirEntryViewModel>();
            _DirPathStack = new Stack<DirEntryViewModel>();

            _ViewActivation_A = DateTime.MinValue;
            _ViewActivation_B = DateTime.MinValue;
        }
        #endregion ctors

        #region properties
        /// <summary>
        /// Gets whether data binding should currently result in Diff data being available or not.
        /// This can be a good indicator for command enable/disablement when these make sense
        /// with the required data only.
        /// </summary>
        public bool IsDiffDataAvailable
        {
            get { return _IsDiffDataAvailable; }
            private set
            {
                if (_IsDiffDataAvailable != value)
                {
                    _IsDiffDataAvailable = value;
                    NotifyPropertyChanged(() => IsDiffDataAvailable);
                }
            }
        }

        public IReadOnlyList<DirEntryViewModel> DirEntries
        {
            get
            {
                return _DirEntries;
            }
        }

        public string PathA
        {
            get { return _PathA; }
            private set
            {
                if (_PathA != value)
                {
                    _PathA = value;
                    NotifyPropertyChanged(() => PathA);
                }
            }
        }

        public string PathB
        {
            get { return _PathB; }
            private set
            {
                if (_PathB != value)
                {
                    _PathB = value;
                    NotifyPropertyChanged(() => PathB);
                }
            }
        }

        /// <summary>
        /// Gets/set the time stamp of the last time when the attached view
        /// has been activated (GotFocus).
        /// </summary>
        public DateTime ViewActivation_A
        {
            get
            {
                return _ViewActivation_A;
            }

            set
            {
                if (_ViewActivation_A != value)
                {
                    _ViewActivation_A = value;
                    NotifyPropertyChanged(() => ViewActivation_A);
                }
            }
        }

        /// <summary>
        /// Gets/set the time stamp of the last time when the attached view
        /// has been activated (GotFocus).
        /// </summary>
        public DateTime ViewActivation_B
        {
            get
            {
                return _ViewActivation_B;
            }

            set
            {
                if (_ViewActivation_B != value)
                {
                    _ViewActivation_B = value;
                    NotifyPropertyChanged(() => ViewActivation_B);
                }
            }
        }

        public object SelectedItem_A
        {
            get { return _SelectedItem_A; }
            set
            {
                if (_SelectedItem_A != value)
                {
                    _SelectedItem_A = value;
                    NotifyPropertyChanged(() => SelectedItem_A);
                }
            }
        }

        public object SelectedItem_B
        {
            get { return _SelectedItem_B; }
            set
            {
                if (_SelectedItem_B != value)
                {
                    _SelectedItem_B = value;
                    NotifyPropertyChanged(() => SelectedItem_B);
                }
            }
        }

        /// <summary>
        /// Gets a string that describes the currently applied filter criteria for display in UI.
        /// </summary>
        public string LblFilter
        {
            get { return _LblFilter; }
            private set
            {
                if (_LblFilter != value)
                {
                    _LblFilter = value;
                    NotifyPropertyChanged(() => LblFilter);
                }
            }
        }

        /// <summary>
        /// Gets a command to browse the current directory diff view by one level down
		/// (if there is a current view and a remaining level down is available).
        /// </summary>
        public ICommand BrowseItemCommand
        {
            get
            {
                if (_BrowseItemCommand == null)
                {
                    _BrowseItemCommand = new RelayCommand<object>((p) =>
                    {
                        var param = p as DirEntryViewModel;
                        if (param == null)
                            return;

                        if (param.IsFile == true)  // Todo Open a text file diff view for this
                            return;

                        if (param.Subentries.Count == 0) // No more subentries to browse to
                            return;

                        var dirs = SetDirectoryEntries(param.Subentries, _Results.RootPathA, _Results.RootPathB);
                        _DirEntries.ReplaceRange(dirs);

                        _DirPathStack.Push(param);
                        PathA = GetSubPath(_CompareOptions.LeftDir, _DirPathStack, true);
                        PathB = GetSubPath(_CompareOptions.RightDir, _DirPathStack, false);
                    });
                }

                return _BrowseItemCommand;
            }
        }

        /// <summary>
        /// Gets a command to browse the current directory diff view by one level
		/// up in the directory hierarchy
		/// (if there is a current view and a remaining level up is available).
        /// </summary>
        public ICommand BrowseUpCommand
        {
            get
            {
                if (_BrowseUpCommand == null)
                {
                    _BrowseUpCommand = new RelayCommand<object>((p) =>
                    {
                        if (_DirPathStack.Count == 0)
                            return;

                        _DirPathStack.Pop();

                        DirectoryDiffEntryCollection entries = null;

                        if (_DirPathStack.Count > 0)
                        {
                            var param = _DirPathStack.Peek();
                            if (param == null)
                                return;

                            if (param.IsFile == true)  // Todo Open a text file diff view for this
                                return;

                            if (param.Subentries.Count == 0) // No more Sub-Entries to browse to
                                return;

                            entries = param.Subentries;
                        }
                        else
                        {
                            entries = _Results.RootEntry.Subentries;
                        }

                        var dirs = SetDirectoryEntries(entries, _Results.RootPathA, _Results.RootPathB);
                        _DirEntries.ReplaceRange(dirs);
                        PathA = GetSubPath(_CompareOptions.LeftDir, _DirPathStack, true);
                        PathB = GetSubPath(_CompareOptions.RightDir, _DirPathStack, false);

                    }, (p) =>
                    {
                        if (_DirPathStack.Count > 0)
                            return true;

                        return false;
                    }

                    );
                }

                return _BrowseUpCommand;
            }
        }

        public ICommand CopyPathToClipboardCommand
        {
            get
            {
                if (_CopyPathToClipboardCommand == null)
                {
                    _CopyPathToClipboardCommand = new RelayCommand<object>((p) =>
                    {
                        var param = (p as string);

                        FileSystemCommands.CopyString(param);


                    }, (p) =>
                    {
                        return (p is string);
                    }

                    );
                }

                return _CopyPathToClipboardCommand;
            }
        }

        public ICommand OpenContainingFolderCommand
        {
            get
            {
                if (_OpenContainingFolderCommand == null)
                {
                    _OpenContainingFolderCommand = new RelayCommand<object>((p) =>
                    {
                        var param = (p as string);

                        FileSystemCommands.OpenContainingFolder(param);

                    }, (p) =>
                    {
                        return (p is string);
                    }

                    );
                }

                return _OpenContainingFolderCommand;
            }
        }

        public ICommand OpenInWindowsCommand
        {
            get
            {
                if (_OpenInWindowsCommand == null)
                {
                    _OpenInWindowsCommand = new RelayCommand<object>((p) =>
                    {
                        var param = (p as string);

                        FileSystemCommands.OpenInWindows(param);

                    }, (p) =>
                    {
                        return (p is string);
                    }

                    );
                }

                return _OpenInWindowsCommand;
            }
        }

        public ICommand OpenFileFromActiveViewCommand
        {
            get
            {
                if (_OpenFileFromActiveViewCommand == null)
                {
                    _OpenFileFromActiveViewCommand = new RelayCommand<object>((p) =>
                    {
                        bool? fromA;
                        var param = GetSelectedItem(out fromA);
                        if (param == null)
                            return;

                        string sPath;
                        if ((bool)fromA)
                            sPath = param.ItemPathA;
                        else
                            sPath = param.ItemPathB;

                        FileSystemCommands.OpenInWindows(sPath);

                    }, (p) =>
                    {
                        bool? fromA;
                        var param = GetSelectedItem(out fromA);
                        if (param == null)
                            return false;

                        return true;
                    });
                }

                return _OpenFileFromActiveViewCommand;
            }
        }
        #endregion properties

        #region methods
        internal void ShowDifferences(ShowDirDiffArgs args)
        {
            try
            {
                var diff = new DirectoryDiff(args.ShowOnlyInA,
                                             args.ShowOnlyInB,
                                             args.ShowDifferent,
                                             args.ShowSame,
                                             args.Recursive,
                                             args.IgnoreDirectoryComparison,
                                             args.FileFilter);

                IDirectoryDiffRoot results = diff.Execute(args.LeftDir, args.RightDir);

                SetData(results);

                _CompareOptions = args; // Record comparison options for later

                IsDiffDataAvailable = true;
            }
            catch
            {
                IsDiffDataAvailable = false;
            }
        }

        private void SetData(IDirectoryDiffRoot results)
        {
            string currentPathA = results.RootPathA;
            string currentPathB = results.RootPathB;

            var dirs = SetDirectoryEntries(results.RootEntry.Subentries, currentPathA, currentPathB);
            //var dirs = SetDirectoryEntries(results.DifferentFiles, currentPathA, currentPathB);

            _DirPathStack.Clear();

            _Results = results;
            _DirEntries.ReplaceRange(dirs);
            PathA = string.Empty;
            PathB = string.Empty;

            // Set a filter description
            if (results.Filter == null)
            {
                this.LblFilter = "All Files";
            }
            else
            {
                DirectoryDiffFileFilter filter = results.Filter;
                this.LblFilter = string.Format("{0}: {1}", filter.Include ? "Includes" : "Excludes", filter.FilterString);
            }
        }

        private List<DirEntryViewModel> SetDirectoryEntries(DirectoryDiffEntryCollection entries,
                                                            string currentPathA,
                                                            string currentPathB)
        {
            List<DirEntryViewModel> dirs = new List<DirEntryViewModel>();

            if (entries == null)
                return dirs;

            foreach (var item in entries)
            {
                dirs.Add(new DirEntryViewModel(item, currentPathA, currentPathB));
            }

            return dirs;
        }

        /// <summary>
        /// Gets the relative sub-path portion of the current <see cref="_DirPathStack"/> against
        /// the given <param name="basePath"/>.
        /// </summary>
        /// <param name="basePath"></param>
        /// <param name="dirPathStack"></param>
        /// <param name="forA"></param>
        /// <returns></returns>
        private string GetSubPath(string basePath, Stack<DirEntryViewModel> dirPathStack, bool forA)
        {
            string subPath = string.Empty;
            if (dirPathStack.Count == 0)
                return subPath;

            if (forA)
                subPath = dirPathStack.Peek().ItemPathA;
            else
                subPath = dirPathStack.Peek().ItemPathB;

            subPath = subPath.Substring(basePath.Length);

            return subPath;
        }

        /// <summary>
        /// Gets the selected item (of the 2 side by side views) that was activated last
        /// (had the focus the last time).
        /// </summary>
        /// <returns></returns>
        internal DirEntryViewModel GetSelectedItem(out bool? fromA)
        {
            fromA = null;

            if (ViewActivation_A < ViewActivation_B)
            {
                fromA = false;
                return SelectedItem_B as DirEntryViewModel;
            }

            fromA = true;
            return SelectedItem_A as DirEntryViewModel;
        }

        #endregion methods
    }
}
