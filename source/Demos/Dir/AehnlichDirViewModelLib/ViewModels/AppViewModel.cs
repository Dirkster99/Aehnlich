namespace AehnlichDirViewModelLib.ViewModels
{
    using AehnlichDirViewModelLib.Enums;
    using AehnlichDirViewModelLib.ViewModels.Base;
    using AehnlichLib.Dir;
    using AehnlichLib.Enums;
    using AehnlichLib.Interfaces;
    using AehnlichLib.Progress;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Input;

    public class AppViewModel : Base.ViewModelBase //, IDisposable
    {
        #region fields
        private string _RightDirPath;
        private string _LeftDirPath;

        private CancellationTokenSource _cancelTokenSource;

        private ICommand _CompareDirectoriesCommand;
        private ICommand _CancelCompareCommand;
        private ICommand _DiffViewModeSelectCommand;

        private ListItemViewModel _DiffViewModeSelected;
        private DiffFileModeItemViewModel _DiffFileModeSelected;
        private readonly DiffProgressViewModel _DiffProgress;
        private readonly List<ListItemViewModel> _DiffViewModes;
        private readonly DirDiffDocViewModel _DirDiffDoc;
        private readonly List<DiffFileModeItemViewModel> _DiffFileModes;
        #endregion fields

        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        public AppViewModel()
        {
            _cancelTokenSource = new CancellationTokenSource();

            _DirDiffDoc = new DirDiffDocViewModel();
            _DiffViewModes = ResetViewModeDefaults();
            _DiffProgress = new DiffProgressViewModel();

            _DiffFileModes = new List<DiffFileModeItemViewModel>();
            _DiffFileModeSelected = new DiffFileModeItemViewModel("All Bytes",
                "Compare each file by their length, last update, and byte by byte sequence",
                DiffDirFileMode.ByteLength_LastUpdate_AllBytes);

            _DiffFileModes.Add(
                new DiffFileModeItemViewModel("Last Change",
                "Compare last time of change of each file",
                DiffDirFileMode.LastUpdate));

            _DiffFileModes.Add(
                new DiffFileModeItemViewModel("Byte Length",
                "Compare the byte length of each file",
                DiffDirFileMode.ByteLength));

            _DiffFileModes.Add(new DiffFileModeItemViewModel("Byte Length + Last Change",
                "Compare the byte length and last time of change of each file",
                DiffDirFileMode.ByteLength_LastUpdate));

            _DiffFileModes.Add(_DiffFileModeSelected);
        }
        #endregion ctors

        #region properties
        public DirDiffDocViewModel DirDiffDoc
        {
            get
            {
                return _DirDiffDoc;
            }
        }

        #region Diff File Mode Selection
        public List<DiffFileModeItemViewModel> DiffFileModes
        {
            get
            {
                return _DiffFileModes;
            }
        }

        public DiffFileModeItemViewModel DiffFileModeSelected
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

                        if (leftDir == null || rightDir == null)
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

        public IReadOnlyList<ListItemViewModel> DiffViewModes
        {
            get { return _DiffViewModes; }
        }

        public ListItemViewModel DiffViewModeSelected
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
                            DirDiffDoc.SetViewMode(DiffViewModeEnum.DirectoriesAndFiles);
                        }
                        else
                        {
                            if (param.Key == (int)DiffViewModeEnum.FilesOnly)
                            {
                                DirDiffDoc.SetViewMode(DiffViewModeEnum.FilesOnly);
                            }
                        }
                    }, ((p) => { return DirDiffDoc.IsDiffDataAvailable; }));
                }

                return _DiffViewModeSelectCommand;
            }
        }

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
            if (_cancelTokenSource != null)
            {
                if (_cancelTokenSource.IsCancellationRequested == true)
                    return;
            }
            else
            {
                _cancelTokenSource = new CancellationTokenSource();
            }

            var args = new Models.ShowDirDiffArgs(leftDir, rightDir);

            var diff = new DirectoryDiff(args.ShowOnlyInA, args.ShowOnlyInB,
                                         args.ShowDifferent, args.ShowSame,
                                         args.Recursive,
                                         args.IgnoreDirectoryComparison,
                                         args.FileFilter,
                                         dirFileMode);

            try
            {
                var token = _cancelTokenSource.Token;
                _DiffProgress.ResetProgressValues(token);

                Task.Factory.StartNew<IDiffProgress>(
                        (p) => diff.Execute(args.LeftDir, args.RightDir, _DiffProgress)
                      , TaskCreationOptions.LongRunning, token)
                .ContinueWith((r) =>
                {
                    bool onError = false;

                    if (r.Result == null)
                        onError = true;
                    else
                    {
                        if (r.Result.ResultData == null)
                            onError = true;
                    }

                    if (onError == false)
                    {
                        var diffResults = r.Result.ResultData as IDirectoryDiffRoot;
                        _DirDiffDoc.ShowDifferences(args, diffResults);
                    }
                    else
                    {
                        if (_cancelTokenSource != null)
                        {
                            _cancelTokenSource.Dispose();
                            _cancelTokenSource = null;
                        }

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
                if (string.IsNullOrEmpty(leftDir) == true || string.IsNullOrEmpty(rightDir) == true)
                    return false;

                if (leftDir.Length < 2 || rightDir.Length < 2)
                    return false;

                var leftDirInfo = new DirectoryInfo(leftDir);
                var rightDirInfo = new DirectoryInfo(rightDir);

                if (leftDirInfo.Exists == false || rightDirInfo.Exists == false)
                    return false;
            }
            catch
            {
                return false;
            }

            return true;
        }
        #endregion Compare Files Command

        private List<ListItemViewModel> ResetViewModeDefaults()
        {
            var lst = new List<ListItemViewModel>();

            lst.Add(new ListItemViewModel("Directories and Files", (int)DiffViewModeEnum.DirectoriesAndFiles));
            lst.Add(new ListItemViewModel("Files only", (int)DiffViewModeEnum.FilesOnly));

            DiffViewModeSelected = lst[0]; // Select default view mode

            return lst;
        }
        #endregion methods
    }
}
