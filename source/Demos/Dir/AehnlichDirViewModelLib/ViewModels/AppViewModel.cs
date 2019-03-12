namespace AehnlichDirViewModelLib.ViewModels
{
    using AehnlichDirViewModelLib.ViewModels.Base;
    using System.Windows.Input;

    public class AppViewModel : Base.ViewModelBase
    {
        #region fields
        private string _RightDirPath;
        private string _LeftDirPath;
        private ICommand _CompareDirectoriesCommand;

        private readonly DirDiffDocViewModel _DirDiffDoc;
        #endregion fields

        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        public AppViewModel()
        {
            _DirDiffDoc = new DirDiffDocViewModel();
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
        #endregion methods
    }
}
