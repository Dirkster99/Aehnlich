namespace DiffLib.Dir
{
    #region Using Directives

    #endregion

    public sealed class DirectoryDiffEntry
	{
		#region Private Data Members

		private bool different;
		private bool inA;
		private bool inB;
		private bool isFile;
		private string error;
		private string name;
		private DirectoryDiffEntryCollection subentries;

		#endregion

		#region Constructors

		internal DirectoryDiffEntry(string name, bool isFile, bool inA, bool inB, bool different)
		{
			this.name = name;
			this.isFile = isFile;
			this.inA = inA;
			this.inB = inB;
			this.different = different;
		}

		#endregion

		#region Public Properties

		public bool Different
		{
			get { return this.different; }

			internal set { this.different = value; }
		}

		public string Error
		{
			get { return this.error; }

			internal set { this.error = value; }
		}

		public bool InA => this.inA;

		public bool InB => this.inB;

		public bool IsFile => this.isFile;

		public string Name => this.name;

		public object TagA { get; set; }

		public object TagB { get; set; }

		public DirectoryDiffEntryCollection Subentries
		{
			get
			{
				if (this.subentries == null && !this.isFile)
				{
					this.subentries = new DirectoryDiffEntryCollection();
				}

				return this.subentries;
			}
		}

		#endregion
	}
}
