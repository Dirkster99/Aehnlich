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
        private bool _ShowOnlyInA;
        private bool _ShowOnlyInB;
        private bool _ShowIfDifferent;
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
            _ShowOnlyInA = true;
            _ShowOnlyInB = true;
            _ShowIfDifferent = true;
        }
        #endregion ctors

        #region properties
        /// <summary>
        /// Gets a command that closes this viewmodel and opens the next viewmodel
        /// to view the actual directory comparison results.
        /// </summary>
        public ICommand CreateNewDirectoryCompareCommand { get; }

        /// <summary>
        /// Gets/sets the left directory path to be matched/diff'ed
        /// against the <see cref="RightDirectoryPath"/>.
        /// </summary>
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

        /// <summary>
        /// Gets/sets the right directory path to be matched/diff'ed
        /// against the <see cref="LeftDirectoryPath"/>.
        /// </summary>
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

        /// <summary>
        /// Determines whether only the content in a given directory is compared
        /// or whether content in sub-directories is also taken into account.
        /// </summary>
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

        public bool ShowOnlyInA
        {
            get { return _ShowOnlyInA; }
            set
            {
                if (_ShowOnlyInA != value)
                {
                    _ShowOnlyInA = value;
                    NotifyPropertyChanged(() => ShowOnlyInA);
                }
            }
        }

        public bool ShowOnlyInB
        {
            get { return _ShowOnlyInB; }
            set
            {
                if (_ShowOnlyInB != value)
                {
                    _ShowOnlyInB = value;
                    NotifyPropertyChanged(() => ShowOnlyInB);
                }
            }
        }

        public bool ShowIfDifferent
        {
            get { return _ShowIfDifferent; }
            set
            {
                if (_ShowIfDifferent != value)
                {
                    _ShowIfDifferent = value;
                    NotifyPropertyChanged(() => ShowIfDifferent);
                }
            }
        }
        #endregion properties

        #region methods
        internal ShowDirDiffArgs GetDirDiffSetup()
        {
            var setup = new ShowDirDiffArgs(LeftDirectoryPath, RightDirectoryPath);

            setup.Recursive = this.IsRecursive;
            setup.ShowOnlyInA = ShowOnlyInA;
            setup.ShowOnlyInB = ShowOnlyInB;
            setup.ShowDifferent = ShowIfDifferent;

            return setup;
        }
        #endregion methods
    }
}
