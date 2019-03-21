namespace AehnlichLib.Dir
{
    using System;
	using AehnlichLib.Interfaces;

    internal sealed class DirectoryDiffRoot : IDirectoryDiffRoot
    {
        #region fields
        private readonly DirectoryDiffEntry _rootEntry;
		private readonly DirectoryDiffEntryCollection _DifferentFiles;
        private readonly bool _Recursive;
        private readonly DirectoryDiffFileFilter _Filter;
        #endregion fields

        #region Constructors
        /// <summary>
        /// Class constructor
        /// </summary>
        public DirectoryDiffRoot(string rootPathA, string rootPathB,
								 bool recursive,
								 DirectoryDiffFileFilter filter)
			: this ()
        {
			this.RootPathA = rootPathA;
			this.RootPathB = rootPathB;
			
			_Recursive = recursive;
			_Filter = filter;
        }
		
        /// <summary>
        /// Class constructor
        /// </summary>
        public DirectoryDiffRoot()
        {
			_rootEntry = new DirectoryDiffEntry();
			_DifferentFiles = new DirectoryDiffEntryCollection();
        }
        #endregion Constructors
		
		public string RootPathA { get; }

		public string RootPathB { get; }

		public bool Recursive { get { return _Recursive; } }

		public DirectoryDiffFileFilter Filter { get { return _Filter; } }
		
        /// <summary>
        /// Gets a hierarchical collection of directories (and their files)
		/// that are different in the directories below <see cref="RootPathA"> and <see cref="RootPathB">.
        /// </summary>
        public IDirectoryDiffEntry RootEntry
        {
            get { return _rootEntry; }
        }

        /// <summary>
        /// Gets a collection of files that are different in the directories below
		/// <see cref="RootPathA"> and <see cref="RootPathB">.
        /// </summary>
        public DirectoryDiffEntryCollection DifferentFiles
        {
            get
            {
                return _DifferentFiles;
            }
        }
		
		#region methods
		internal void AddDiffFile(IDirectoryDiffEntry diffEntry)
		{
			_DifferentFiles.Add(diffEntry);
		}
		#endregion methods
    }
}
