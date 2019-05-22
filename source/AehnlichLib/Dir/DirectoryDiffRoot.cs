namespace AehnlichLib.Dir
{
    using AehnlichLib.Enums;
    using AehnlichLib.Interfaces;
    using AehnlichLib.Interfaces.Dir;

    /// <summary>
    /// Implements an object root of an object data graph that models directories
    /// A (left side) and B (right side), their sub-directories, and their files.
    /// 
    /// The data in this structure captures directory and file differences in either
    /// (sub-)directory in terms of directories and/or files that have been:
    /// deleted, added, changed, or appear to be equal in both.
    /// </summary>
    internal sealed class DirectoryDiffRoot : IDirectoryDiffRoot
    {
        #region fields
        private readonly DirectoryDiffEntry _rootEntry;
        private readonly DirectoryDiffEntryCollection _DifferentFiles;
        private readonly bool _Recursive;
        private readonly DirectoryDiffFileFilter _Filter;
        private readonly DiffDirFileMode _DiffMode;
        #endregion fields

        #region Constructors
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="recursive"></param>
        /// <param name="rootPathA"></param>
        /// <param name="rootPathB"></param>
        /// <param name="diffMode">
        /// Determines the modus operandi per <see cref="DiffDirFileMode"/> that is used to
        /// compare two files and pronounce them as different or equal.
        /// </param>
        public DirectoryDiffRoot(string rootPathA, string rootPathB,
                                 bool recursive,
                                 DirectoryDiffFileFilter filter,
                                 DiffDirFileMode diffMode,
                                 IDataSource dataSource)
            : this()
        {
            this.RootPathA = rootPathA;
            this.RootPathB = rootPathB;

            _Recursive = recursive;
            _Filter = filter;
            _DiffMode = diffMode;
            this.Source = dataSource;
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        private DirectoryDiffRoot()
        {
            _rootEntry = new DirectoryDiffEntry();
            _DifferentFiles = new DirectoryDiffEntryCollection();

            CountFilesAdded = 0;
            CountFilesDeleted = 0;
            CountFilesChanged = 0;
        }
        #endregion Constructors

        /// <summary>
        /// Gets an object that provides routines and objects for working with
        /// data objects that refer to directories and files.
        /// </summary>
        public IDataSource Source { get; }

        /// <summary>
        /// Gets the root directory level path of the diff directory A (left side).
        /// </summary>
        public string RootPathA { get; }

        /// <summary>
        /// Gets the root directory level path of directory B (right side).
        /// </summary>
		public string RootPathB { get; }

        /// <summary>
        /// Gets whether this root represents a diff between two directories
        /// with more than one level (false) or multiple levels of sub-directories (true).
        /// </summary>
		public bool Recursive { get { return _Recursive; } }

        public DirectoryDiffFileFilter Filter { get { return _Filter; } }

        /// <summary>
        /// Gets a hierarchical collection of directories (and their files)
        /// that are different in the directories below <see cref="RootPathA"/> and <see cref="RootPathB"/>.
        /// </summary>
        public IDirectoryDiffEntry RootEntry
        {
            get { return _rootEntry; }
        }

        /// <summary>
        /// Gets a collection of files that are different in the directories below
		/// <see cref="RootPathA"/> and <see cref="RootPathB"/>.
        /// </summary>
        public DirectoryDiffEntryCollection DifferentFiles
        {
            get
            {
                return _DifferentFiles;
            }
        }

        public DiffDirFileMode DiffMode
        {
            get
            {
                return _DiffMode;
            }
        }

        /// <summary>
        /// Gets the number of files deleted in directory A (left side)
        /// when comparing directory A (left side) with directory B (right side).
        /// </summary>
        public int CountFilesDeleted { get; private set; }

        /// <summary>
        /// Gets the number of files added in directory B (left side)
        /// when comparing directory A (left side) with directory B (right side).
        /// </summary>
        public int CountFilesAdded { get; private set; }

        /// <summary>
        /// Gets the number of changes files in directory A and B
        /// when comparing directory A (left side) with directory B (right side).
        /// </summary>
        public int CountFilesChanged { get; private set; }

        #region methods
        /// <summary>
        /// Adds another item into the collection of files
        /// that are different between 2 sets of files.
        /// </summary>
        /// <param name="diffEntry"></param>
        internal void AddDiffFile(IDirectoryDiffEntry diffEntry)
        {
            _DifferentFiles.Add(diffEntry);

            switch (diffEntry.EditContext)
            {
                case Enums.EditType.Delete:
                    CountFilesDeleted += 1;
                    break;
                case Enums.EditType.Insert:
                    CountFilesAdded += 1;
                    break;
                case Enums.EditType.Change:
                    CountFilesChanged += 1;
                    break;

                case Enums.EditType.None:
                default:
                    break;
            }
        }
        #endregion methods
    }
}
