namespace DiffLib.Dir
{
    #region Using Directives

    using System.IO;

    #endregion

    public sealed class DirectoryDiffResults
	{
		#region Private Data Members

		private readonly DirectoryInfo directoryA;
		private readonly DirectoryInfo directoryB;
		private readonly bool recursive;
		private readonly DirectoryDiffEntryCollection entries;
		private readonly DirectoryDiffFileFilter filter;

		#endregion

		#region Constructors

		internal DirectoryDiffResults(
			DirectoryInfo directoryA,
			DirectoryInfo directoryB,
			DirectoryDiffEntryCollection entries,
			bool recursive,
			DirectoryDiffFileFilter filter)
		{
			this.directoryA = directoryA;
			this.directoryB = directoryB;
			this.entries = entries;
			this.recursive = recursive;
			this.filter = filter;
		}

		#endregion

		#region Public Properties

		public DirectoryInfo DirectoryA => this.directoryA;

		public DirectoryInfo DirectoryB => this.directoryB;

		public DirectoryDiffEntryCollection Entries => this.entries;

		public DirectoryDiffFileFilter Filter => this.filter;

		public bool Recursive => this.recursive;

		#endregion
	}
}
