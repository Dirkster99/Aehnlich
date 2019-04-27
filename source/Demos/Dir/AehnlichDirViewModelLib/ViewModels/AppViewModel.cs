namespace AehnlichDirViewModelLib.ViewModels
{
    using AehnlichDirViewModelLib.Enums;
    using AehnlichDirViewModelLib.Interfaces;
    using AehnlichDirViewModelLib.ViewModels.Base;
    using AehnlichLib.Dir;
    using AehnlichLib.Enums;
    using AehnlichLib.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Input;

    internal class AppViewModel : Base.ViewModelBase, IAppViewModel
    {
        #region fields
        private string _RightDirPath;
        private string _LeftDirPath;

        private ICommand _CompareDirectoriesCommand;
        private ICommand _CancelCompareCommand;
        private ICommand _DiffViewModeSelectCommand;

        private readonly List<IListItemViewModel> _DiffViewModes;
        private IListItemViewModel _DiffViewModeSelected;

        private readonly List<IDiffFileModeItemViewModel> _DiffFileModes;
        private IDiffFileModeItemViewModel _DiffFileModeSelected;

        private bool _Disposed;
        private CancellationTokenSource _cancelTokenSource;
        private readonly DiffProgressViewModel _DiffProgress;
        private readonly DirDiffDocViewModel _DirDiffDoc;
        #endregion fields

        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        public AppViewModel()
        {
            _cancelTokenSource = new CancellationTokenSource();
            _DiffProgress = new DiffProgressViewModel();

            _DirDiffDoc = new DirDiffDocViewModel();
            _DiffViewModes = ResetViewModeDefaults();

            _DiffFileModes = new List<IDiffFileModeItemViewModel>();
            _DiffFileModeSelected = CreateCompareFileModes(_DiffFileModes);
        }
        #endregion ctors

        #region properties
        /// <summary>
        /// Gets the viewmodel for the document that contains the diff information
        /// on a left directory (A) and a right directory (B) and its contents.
        /// </summary>
        public IDirDiffDocViewModel DirDiffDoc
        {
            get
            {
                return _DirDiffDoc;
            }
        }

        #region Diff File Mode Selection
        /// <summary>
        /// Gets a list of modies that can be used to compare one directory
        /// and its contents, to the other directory.
        /// </summary>
        public List<IDiffFileModeItemViewModel> DiffFileModes
        {
            get
            {
                return _DiffFileModes;
            }
        }

        /// <summary>
        /// Gets/sets the mode that is currently used to compare one directory
        /// and its contents with the other directory.
        /// </summary>
        public IDiffFileModeItemViewModel DiffFileModeSelected
        {
            get
            {
                return _DiffFileModeSelected;
            }

            set
            {
                if (_DiffFileModeSelected != value)
                {
                    _DiffFileModeSelected = value;
                    NotifyPropertyChanged(() => DiffFileModeSelected);
                }
            }
        }
        #endregion

        #region CompareCommand
        /// <summary>
        /// Gets a command that refreshs (reloads) the comparison of
        /// two directories (sub-directories) and their files.
        /// </summary>
        public ICommand CompareDirectoriesCommand
        {
            get
            {
                if (_CompareDirectoriesCommand == null)
                {
                    _CompareDirectoriesCommand = new RelayCommand<object>((p) =>
                    {
                        string leftDir;
                        string rightDir;

                        if ((p is object[]) == true)
                        {
                            var param = p as object[];

                            if (param.Length != 2)
                                return;

                            leftDir = param[0] as string;
                            rightDir = param[1] as string;
                        }
                        else
                            return;

                        leftDir = PathUtil.GetPathIfDirExists(leftDir);
                        rightDir = PathUtil.GetPathIfDirExists(rightDir);

                        if (string.IsNullOrEmpty(leftDir) == true ||
                            string.IsNullOrEmpty(rightDir) == true)
                            return;

                        CompareFilesCommand_Executed(leftDir, rightDir, _DiffFileModeSelected.ModeKey);
                        NotifyPropertyChanged(() => DirDiffDoc);
                    },
                    (p) =>
                    {
                        if (DiffProgress.IsProgressbarVisible == true)
                            return false;

                        string leftDir;
                        string rightDir;

                        if ((p is object[]) == true)
                        {
                            var param = p as object[];

                            if (param.Length != 2)
                                return false;

                            leftDir = param[0] as string;
                            rightDir = param[1] as string;
                        }
                        else
                            return false;

                        return CompareFilesCommand_CanExecut(leftDir, rightDir);
                    });
                }

                return _CompareDirectoriesCommand;
            }
        }

        /// <summary>
        /// Gets a command that can be used to cancel the directory comparison
        /// currently being processed (if any).
        /// </summary>
        public ICommand CancelCompareCommand
        {
            get
            {
                if (_CancelCompareCommand == null)
                {
                    _CancelCompareCommand = new RelayCommand<object>((p) =>
                    {
                        if (_cancelTokenSource != null)
                        {
                            if (_cancelTokenSource.IsCancellationRequested == false)
                                _cancelTokenSource.Cancel();
                        }
                    },
                    (p) =>
                    {
                        if (DiffProgress.IsProgressbarVisible == true)
                        {
                            if (_cancelTokenSource != null)
                            {
                                if (_cancelTokenSource.IsCancellationRequested == false)
                                    return true;
                            }
                        }

                        return false;
                    });
                }

                return _CancelCompareCommand;
            }
        }

        #endregion CompareCommand

        /// <summary>
        /// Gets the left directory path.
        /// </summary>
        public string LeftDirPath
        {
            get
            {
                return _LeftDirPath;
            }

            set
            {
                if (_LeftDirPath != value)
                {
                    _LeftDirPath = value;
                    NotifyPropertyChanged(() => LeftDirPath);
                }
            }
        }

        /// <summary>
        /// Gets the right directory path.
        /// </summary>
        public string RightDirPath
        {
            get
            {
                return _RightDirPath;
            }

            set
            {
                if (_RightDirPath != value)
                {
                    _RightDirPath = value;
                    NotifyPropertyChanged(() => RightDirPath);
                }
            }
        }

        #region File DiffMode
        /// <summary>
        /// Gets a list of view modes by which the results of the
        /// directory and file comparison can be viewed
        /// (eg.: directories and files or files only).
        /// </summary>
        public IReadOnlyList<IListItemViewModel> DiffViewModes
        {
            get { return _DiffViewModes; }
        }

        /// <summary>
        /// Gets the currently selected view mode for the display of diff results.
        /// </summary>
        public IListItemViewModel DiffViewModeSelected
        {
            get { return _DiffViewModeSelected; }

            protected set
            {
                if (_DiffViewModeSelected != value)
                {
                    _DiffViewModeSelected = value;
                    NotifyPropertyChanged(() => DiffViewModeSelected);
                }
            }
        }

        /// <summary>
        /// Gets a command that can be used to change the
        /// currently selected view mode for displaying diff results.
        /// </summary>
        public ICommand DiffViewModeSelectCommand
        {
            get
            {
                if (_DiffViewModeSelectCommand == null)
                {
                    _DiffViewModeSelectCommand = new RelayCommand<object>((p) =>
                    {
                        var param = p as ListItemViewModel;

                        if (param == null)
                            return;

                        if (param.Key == (int)DiffViewModeEnum.DirectoriesAndFiles)
                        {
                            _DirDiffDoc.SetViewMode(DiffViewModeEnum.DirectoriesAndFiles);
                        }
                        else
                        {
                            if (param.Key == (int)DiffViewModeEnum.FilesOnly)
                            {
                                _DirDiffDoc.SetViewMode(DiffViewModeEnum.FilesOnly);
                            }
                        }
                    }, ((p) => { return DirDiffDoc.IsDiffDataAvailable; }));
                }

                return _DiffViewModeSelectCommand;
            }
        }
        #endregion File DiffMode

        /// <summary>
        /// Gets a viewmodel that manages progress display in terms of min, value, max or
        /// indeterminate progress display.
        /// </summary>
        public IDiffProgress DiffProgress
        {
            get
            {
                return _DiffProgress;
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Initializes the left and right dir from the last application session (if any)
        /// </summary>
        /// <param name="leftDirPath"></param>
        /// <param name="rightDirPath"></param>
        public void Initialize(string leftDirPath, string rightDirPath)
        {
            LeftDirPath = leftDirPath;
            RightDirPath = rightDirPath;
        }

        #region Compare Files Command
        private void CompareFilesCommand_Executed(string leftDir,
                                                  string rightDir,
                                                  DiffDirFileMode dirFileMode)
        {
            if (_cancelTokenSource.IsCancellationRequested == true)
                return;

            var args = new Models.ShowDirDiffArgs(leftDir, rightDir);

            var diff = new DirectoryDiff(args.ShowOnlyInA, args.ShowOnlyInB,
                                         args.ShowDifferent, args.ShowSame,
                                         args.Recursive,
                                         args.IgnoreDirectoryComparison,
                                         args.FileFilter,
                                         dirFileMode, args.LastUpDateFilePrecision);

            try
            {
                _DiffProgress.ResetProgressValues(_cancelTokenSource.Token);

                Task.Factory.StartNew<IDiffProgress>(
                        (p) => diff.Execute(args.LeftDir, args.RightDir, _DiffProgress)
                      , TaskCreationOptions.LongRunning, _cancelTokenSource.Token)
                .ContinueWith((r) =>
                {
                    bool onError = false;
                    bool taskCancelled = false;

                    if (_cancelTokenSource != null)
                    {
                        // Re-create cancellation token if this task was cancelled
                        // to support cancelable tasks in the future
                        if (_cancelTokenSource.IsCancellationRequested)
                        {
                            taskCancelled = true;
                            _cancelTokenSource.Dispose();
                            _cancelTokenSource = new CancellationTokenSource();
                        }
                    }

                    if (taskCancelled == false)
                    {
                        if (r.Result == null)
                            onError = true;
                        else
                        {
                            if (r.Result.ResultData == null)
                                onError = true;
                        }
                    }

                    if (onError == false && taskCancelled == false)
                    {
                        var diffResults = r.Result.ResultData as IDirectoryDiffRoot;
                        _DirDiffDoc.ShowDifferences(args, diffResults);
                    }
                    else
                    {
                        // Display Error
                    }
                });
            }
            catch
            {
            }
        }

        private bool CompareFilesCommand_CanExecut(string leftDir, string rightDir)
        {
            try
            {
                leftDir = PathUtil.GetPathIfDirExists(leftDir);
                rightDir = PathUtil.GetPathIfDirExists(rightDir);

                if (string.IsNullOrEmpty(leftDir) == true || string.IsNullOrEmpty(rightDir) == true)
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion Compare Files Command

        #region IDisposeable
        /// <summary>
        /// Standard dispose method of the <seealso cref="IDisposable" /> interface.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Source: http://www.codeproject.com/Articles/15360/Implementing-IDisposable-and-the-Dispose-Pattern-P
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (_Disposed == false)
            {
                if (disposing == true)
                {
                    if (_cancelTokenSource != null)
                    {
                        _cancelTokenSource.Dispose();
                        _cancelTokenSource = null;
                    }
                }

                // There are no unmanaged resources to release, but
                // if we add them, they need to be released here.
            }

            _Disposed = true;

            //// If it is available, make the call to the
            //// base class's Dispose(Boolean) method
            ////base.Dispose(disposing);
        }
        #endregion IDisposeable

        private List<IListItemViewModel> ResetViewModeDefaults()
        {
            var lst = new List<IListItemViewModel>();

            lst.Add(new ListItemViewModel("Directories and Files", (int)DiffViewModeEnum.DirectoriesAndFiles));
            lst.Add(new ListItemViewModel("Files only", (int)DiffViewModeEnum.FilesOnly));

            DiffViewModeSelected = lst[0]; // Select default view mode

            return lst;
        }

        private static IDiffFileModeItemViewModel CreateCompareFileModes(
            IList<IDiffFileModeItemViewModel> diffFileModes)
        {
            DiffFileModeItemViewModel defaultItem = null;

            diffFileModes.Add(
                new DiffFileModeItemViewModel("File Length",
                "Compare the byte length of each file",
                DiffDirFileMode.ByteLength));

            diffFileModes.Add(
                new DiffFileModeItemViewModel("Last Change",
                "Compare last modification time of change of each file",
                DiffDirFileMode.LastUpdate));

            defaultItem = new DiffFileModeItemViewModel("File Length + Last Change",
                "Compare the byte length and last modification time of each file",
                DiffDirFileMode.ByteLength_LastUpdate);

            diffFileModes.Add(defaultItem);

            diffFileModes.Add(new DiffFileModeItemViewModel("All Bytes",
                "Compare each file by their length, last modification time, and byte-by-byte sequence",
                DiffDirFileMode.ByteLength_LastUpdate_AllBytes));

            return defaultItem;
        }
        #endregion methods
    }
}
