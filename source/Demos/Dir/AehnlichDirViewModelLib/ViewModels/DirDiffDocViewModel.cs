namespace AehnlichDirViewModelLib.ViewModels
{
    using AehnlichDirViewModelLib.Models;
    using AehnlichDirViewModelLib.ViewModels.Base;
    using AehnlichLib.Dir;
    using System.Collections.Generic;
    using System.Windows.Input;

    public class DirDiffDocViewModel : Base.ViewModelBase
    {
        #region fields
        private string _LblFilter;
        private string _PathB, _PathA;
        private ICommand _BrowseItemCommand;
        private ICommand _BrowseUpCommand;

        private DirectoryDiffResults _Results;
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
        }
        #endregion ctors

        #region properties
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

                        var dirs = SetDirectoryEntries(param.Subentries, param.ItemPathA, param.ItemPathB);
                        _DirEntries.ReplaceRange(dirs);
                        PathA = param.ItemPathA;
                        PathB = param.ItemPathB;

                        _DirPathStack.Push(param);
                    });
                }

                return _BrowseItemCommand;
            }
        }

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
                        string itemPathA = string.Empty, itemPathB = string.Empty;
                        if (_DirPathStack.Count > 0)
                        {
                            var param = _DirPathStack.Peek();
                            if (param == null)
                                return;

                            if (param.IsFile == true)  // Todo Open a text file diff view for this
                                return;

                            if (param.Subentries.Count == 0) // No more subentries to browse to
                                return;

                            itemPathA = param.ItemPathA;
                            itemPathB = param.ItemPathB;
                            entries = param.Subentries;
                        }
                        else
                        {
                            itemPathA = _Results.DirectoryA.FullName; // Go back to root entries display
                            itemPathB = _Results.DirectoryB.FullName;
                            entries = _Results.Entries;
                        }

                        var dirs = SetDirectoryEntries(entries, itemPathA, itemPathB);
                        _DirEntries.ReplaceRange(dirs);
                        PathA = itemPathA;
                        PathB = itemPathB;

                    },(p) =>
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
        #endregion properties

        #region methods
        internal void ShowDifferences(ShowDirDiffArgs args)
        {
            DirectoryDiff diff = new DirectoryDiff(
                args.ShowOnlyInA,
                args.ShowOnlyInB,
                args.ShowDifferent,
                args.ShowSame,
                args.Recursive,
                args.IgnoreDirectoryComparison,
                args.FileFilter);

            DirectoryDiffResults results = diff.Execute(args.LeftDir, args.RightDir);

            SetData(results);
        }

        private void SetData(DirectoryDiffResults results)
        {
            string currentPathA = results.DirectoryA.FullName;
            string currentPathB = results.DirectoryB.FullName;

            var dirs = SetDirectoryEntries(results.Entries, currentPathA, currentPathB);

            _Results = results;
            _DirEntries.ReplaceRange(dirs);
            PathA = currentPathA;
            PathB = currentPathB;

////            this.TreeA.SetData(results, true);
////            this.TreeB.SetData(results, false);

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

////            this.UpdateButtons();
////
////            if (this.TreeA.Nodes.Count > 0)
////            {
////                this.TreeA.SelectedNode = this.TreeA.Nodes[0];
////            }
        }

        private List<DirEntryViewModel> SetDirectoryEntries(DirectoryDiffEntryCollection entries,
                                                            string currentPathA,
                                                            string currentPathB)
        {
            List<DirEntryViewModel> dirs = new List<DirEntryViewModel>();
            foreach (var item in entries)
            {
                dirs.Add(new DirEntryViewModel(item, currentPathA, currentPathB));
            }

            return dirs;
        }
        #endregion methods
    }
}
