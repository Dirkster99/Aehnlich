namespace AehnlichDirViewModelLib.ViewModels
{
    using AehnlichDirViewModelLib.Enums;
    using AehnlichDirViewModelLib.ViewModels.Base;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows.Input;

    public class AppViewModel : Base.ViewModelBase
    {
        #region fields
        private string _RightDirPath;
        private string _LeftDirPath;

        private ICommand _CompareDirectoriesCommand;
        private ICommand _DiffViewModeSelectCommand;

        private ListItemViewModel _DiffViewModeSelected;
        private readonly List<ListItemViewModel> _DiffViewModes;
        private readonly DirDiffDocViewModel _DirDiffDoc;
        #endregion fields

        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        public AppViewModel()
        {
            _DirDiffDoc = new DirDiffDocViewModel();
            _DiffViewModes = ResetViewModeDefaults();
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

        /// <summary>
        /// Gets a command that refreshs (reloads) the comparison of 2 textfiles.
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

                        CompareFilesCommand_Executed(leftDir, rightDir);
                        NotifyPropertyChanged(() => DirDiffDoc);
                    },(p) =>
                    {
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
            get { return _DiffViewModes;  }
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
        #endregion properties

        #region methods
        public void Initialize(string leftDirPath, string rightDirPath)
        {
            LeftDirPath = leftDirPath;
            RightDirPath = rightDirPath;
        }

        private void CompareFilesCommand_Executed(string leftDir, string rightDir)
        {
            var args = new Models.ShowDirDiffArgs(leftDir, rightDir);

            _DirDiffDoc.ShowDifferences(args);
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
