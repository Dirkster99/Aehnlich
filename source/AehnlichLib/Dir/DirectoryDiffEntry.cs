namespace AehnlichLib.Dir
{
    public sealed class DirectoryDiffEntry
	{
		#region Private Data Members

		private bool _different;
		private readonly bool _inA;
		private readonly bool _inB;
		private readonly bool _isFile;
		private string _error;
        private DirectoryDiffEntryCollection subentries;

        #endregion

        #region Constructors
        internal DirectoryDiffEntry(string basePath, string name, bool isFile, bool inA, bool inB)
            : this()
        {
            // Mark node as different if this entry either refers to A only or B only
            this._different = (inA == true && inB == false || inA == false && inB == true);

            this.BasePath = basePath;
            this.Name = name;
            this._isFile = isFile;
            this._inA = inA;
            this._inB = inB;
        }

        internal DirectoryDiffEntry(string basePath, string name, bool isFile, bool inA, bool inB, bool different)
            : this(name, isFile, inA, inB, different)
        {
            this.BasePath = basePath;
        }

        internal DirectoryDiffEntry(string name, bool isFile, bool inA, bool inB, bool different)
            : this()
		{
			this.Name = name;
			this._isFile = isFile;
			this._inA = inA;
			this._inB = inB;
			this._different = different;
		}

        private DirectoryDiffEntry()
        {
            BasePath = string.Empty;
        }
		#endregion

		#region Public Properties

		public bool Different
		{
			get { return this._different; }

			internal set { this._different = value; }
		}

		public string Error
		{
			get { return this._error; }

			internal set { this._error = value; }
		}

		public bool InA => this._inA;

		public bool InB => this._inB;

		public bool IsFile => this._isFile;

        public string BasePath { get; }

        public string Name { get; }

		public object TagA { get; set; }

		public object TagB { get; set; }

		public DirectoryDiffEntryCollection Subentries
		{
			get
			{
				if (this.subentries == null && !this._isFile)
				{
					this.subentries = new DirectoryDiffEntryCollection();
				}

				return this.subentries;
			}
		}

        #endregion properties

        #region methods
        public void AddSubEntry(DirectoryDiffEntry entry)
        {
            if (this.subentries == null)
                this.subentries = new DirectoryDiffEntryCollection();

            this.subentries.Add(entry);
        }

        public int CountSubDirectories()
        {
            return (Subentries == null ? 0 : Subentries.Count);
        }

        public bool SetDiffBasedOnChildren(bool ignoreDirectoryComparison)
        {
            // Is this a directory and do we want to ignore directory diffs?
            if (IsFile == false && ignoreDirectoryComparison == true)
            {
                Different = false;
                return Different;
            }

            if (Different == false && ignoreDirectoryComparison == false && Subentries != null)
            {
                for (int i = 0; i < Subentries.Count; i++)
                {
                    if (Subentries[i].Different == true)
                    {
                        Different = true;
                        break;
                    }
                }
            }

            return Different;
        }
        #endregion methods
    }
}
