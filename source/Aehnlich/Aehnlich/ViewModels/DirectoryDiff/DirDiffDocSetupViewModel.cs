namespace Aehnlich.ViewModels.Documents
{
    using AehnlichDirViewModelLib.Interfaces;
    using AehnlichLib.Dir;
    using AehnlichLib.Interfaces.Dir;
    using System.Collections.Generic;
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
        private bool _ShowIfSameFile;
        private bool _ShowIfSameDirectory;
        private string _CustomFiltersSelectedItem;
        private bool _IncludeFilter;
        private bool _ExcludeFilter;
        private string _NewFilterItem;
        private readonly IFileDiffModeViewModel _FileDiffMode;
        private readonly ObservableRangeCollection<string> _CustomFilters;
        private readonly IDataSource _DirDataSource;
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
                                        string leftDirPath, string rightDirPath,
                                        IDataSource dirDataSource)
            : this()
        {
            _DirDataSource = dirDataSource;
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
            _ShowIfSameFile = true;
            _ShowIfSameDirectory = true;
            _CustomFilters = new ObservableRangeCollection<string>();

            _IncludeFilter = true;
            _ExcludeFilter = false;

            _CustomFilters.Add("*.cs");
            _CustomFilters.Add("*.cs;*.xaml");
            _CustomFilters.Add("*.cpp;*.h;*.idl;*.rc;*.c;*.inl");
            _CustomFilters.Add("*.vb");
            _CustomFilters.Add("*.xml");
            _CustomFilters.Add("*.htm;*.html");
            _CustomFilters.Add("*.txt");
            _CustomFilters.Add("*.sql");
            _CustomFilters.Add("*.obj;*.pdb;*.exe;*.dll;*.cache;*.tlog;*.trx;*.FileListAbsolute.txt");

            _FileDiffMode = AehnlichDirViewModelLib.ViewModels.Factory.ConstructFileDiffModes();
        }
        #endregion ctors

        #region properties
        #region Filter Properties
        public IEnumerable<string> CustomFilters
        {
            get
            {
                return _CustomFilters;
            }
        }

        public string FilterText
        {
            get
            {
                if (string.Compare(CustomFiltersSelectedItem, NewFilterItem,true) == 0)
                    return CustomFiltersSelectedItem;

                return NewFilterItem;
            }
        }

        public string CustomFiltersSelectedItem
        {
            get { return _CustomFiltersSelectedItem; }
            set
            {
                if (_CustomFiltersSelectedItem != value)
                {
                    _CustomFiltersSelectedItem = value;
                    NotifyPropertyChanged(() => CustomFiltersSelectedItem);
                    NotifyPropertyChanged(() => FilterText);
                }
            }
        }

        public string NewFilterItem
        {
            get { return _NewFilterItem; }
            set
            {
                if (_NewFilterItem != value)
                {
                    _NewFilterItem = value;
                    NotifyPropertyChanged(() => NewFilterItem);
                    NotifyPropertyChanged(() => FilterText);
                }
            }
        }

        public bool IncludeFilter
        {
            get { return _IncludeFilter; }
            set
            {
                if (_IncludeFilter != value)
                {
                    _IncludeFilter = value;
                    NotifyPropertyChanged(() => IncludeFilter);
                }
            }
        }

        public bool ExcludeFilter
        {
            get { return _ExcludeFilter; }
            set
            {
                if (_ExcludeFilter != value)
                {
                    _ExcludeFilter = value;
                    NotifyPropertyChanged(() => ExcludeFilter);
                }
            }
        }
        #endregion Filter Properties

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

        /// <summary>
        /// Invoke method to normalize left and right paths
        /// to ensure proper processing in later steps.
        /// </summary>
        internal void NormalizePaths()
        {
            RightDirectoryPath = _DirDataSource.NormalizePath(RightDirectoryPath);
            LeftDirectoryPath = _DirDataSource.NormalizePath(LeftDirectoryPath);
        }

        public bool ShowIfSameFile
        {
            get { return _ShowIfSameFile; }
            set
            {
                if (_ShowIfSameFile != value)
                {
                    _ShowIfSameFile = value;
                    NotifyPropertyChanged(() => ShowIfSameFile);
                }
            }
        }

        public bool ShowIfSameDirectory
        {
            get { return _ShowIfSameDirectory; }
            set
            {
                if (_ShowIfSameDirectory != value)
                {
                    _ShowIfSameDirectory = value;
                    NotifyPropertyChanged(() => ShowIfSameDirectory);
                }
            }
        }

        /// <summary>
        /// Gets a viewmodel that defines a comparison strategy for files
        /// (using lastupdate, size in bytes, and/or byte-by-byte comparison)
        /// </summary>
        public IFileDiffModeViewModel FileDiffMode
        {
            get
            {
                return _FileDiffMode;
            }
        }
        #endregion properties

        #region methods
        internal DirDiffArgs GetDirDiffSetup()
        {
            var setup = new DirDiffArgs(LeftDirectoryPath, RightDirectoryPath);

            setup.CompareDirFileMode = this.FileDiffMode.DiffFileModeSelected.ModeKey;

            setup.Recursive = this.IsRecursive;
            setup.ShowOnlyInA = ShowOnlyInA;
            setup.ShowOnlyInB = ShowOnlyInB;
            setup.ShowDifferent = ShowIfDifferent;
            setup.ShowSame = ShowIfSameFile;
            setup.IgnoreDirectoryComparison = !ShowIfSameDirectory;

            // Set file filter aka '*.cs' if any selected
            if (string.IsNullOrEmpty(FilterText) == false)
                setup.FileFilter = new DirectoryDiffFileFilter(FilterText, IncludeFilter);

            return setup;
        }
        #endregion methods
    }
}
