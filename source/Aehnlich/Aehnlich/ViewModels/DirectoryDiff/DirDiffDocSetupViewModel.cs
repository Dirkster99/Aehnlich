namespace Aehnlich.ViewModels.Documents
{
    using AehnlichDirViewModelLib.Models;
    using System.Windows.Input;

    internal class DirDiffDocSetupViewModel : Base.ViewModelBase
    {
        #region fields
        private string _LeftDirectoryPath;
        private string _RightDirectoryPath;
        private bool _IsRecursive;
        #endregion fields

        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="createViewPageCommand">
        /// Gets a command that closes this viewmodel and opens the next viewmodel
        /// to view the actual directory comparison results.
        /// </param>
        public DirDiffDocSetupViewModel(ICommand createViewPageCommand,
                                        string leftDirPath, string rightDirPath)
            : this()
        {
            LeftDirectoryPath = leftDirPath;
            RightDirectoryPath = rightDirPath;
            CreateNewDirectoryCompareCommand = createViewPageCommand;
        }

        /// <summary>
        /// HIDDEN Class constructor
        /// </summary>
        protected DirDiffDocSetupViewModel()
        {
            _IsRecursive = true;
        }
        #endregion ctors

        #region properties
        /// <summary>
        /// Gets a command that closes this viewmodel and opens the next viewmodel
        /// to view the actual directory comparison results.
        /// </summary>
        public ICommand CreateNewDirectoryCompareCommand { get; }

        public string LeftDirectoryPath
        {
            get { return _LeftDirectoryPath; }
            set
            {
                if (_LeftDirectoryPath != value)
                {
                    _LeftDirectoryPath = value;
                    NotifyPropertyChanged(() => LeftDirectoryPath);
                }
            }
        }

        public string RightDirectoryPath
        {
            get { return _RightDirectoryPath; }
            set
            {
                if (_RightDirectoryPath != value)
                {
                    _RightDirectoryPath = value;
                    NotifyPropertyChanged(() => RightDirectoryPath);
                }
            }
        }

        public bool IsRecursive
        {
            get { return _IsRecursive; }
            set
            {
                if (_IsRecursive != value)
                {
                    _IsRecursive = value;
                    NotifyPropertyChanged(() => IsRecursive);
                }
            }
        }
        #endregion properties

        #region methods
        internal ShowDirDiffArgs GetDirDiffSetup()
        {
            var setup = new ShowDirDiffArgs(LeftDirectoryPath, RightDirectoryPath);

            setup.Recursive = this.IsRecursive;

            return setup;
        }
        #endregion methods
    }
}
