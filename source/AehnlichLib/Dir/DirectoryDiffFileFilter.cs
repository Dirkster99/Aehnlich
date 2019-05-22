namespace AehnlichLib.Dir
{
    using AehnlichLib.Interfaces.Dir;
    using System;
    using System.Collections.Generic;

    public sealed class DirectoryDiffFileFilter
	{
		#region Private Data Members

		private readonly string concatenatedFilters;
		private readonly string[] individualFilters;
		private readonly bool include;

		#endregion

		#region Constructors

		public DirectoryDiffFileFilter(string filter, bool include)
		{
			this.concatenatedFilters = filter;
			this.include = include;
			this.individualFilters = filter.Split(';');
			for (int i = 0; i < this.individualFilters.Length; i++)
			{
				this.individualFilters[i] = this.individualFilters[i].Trim();
			}
		}

		#endregion

		#region Public Properties

		public string FilterString { get { return this.concatenatedFilters; } }

		public bool Include { get { return this.include; } }

		#endregion

		#region Public Methods
        internal IFileInfo[] Filter(IDirectoryInfo directory)
        {
            // Get all the files that match the filters
            List<IFileInfo> files = new List<IFileInfo>();
            foreach (string filter in this.individualFilters)
            {
                IFileInfo[] filterFiles = directory.GetFiles(filter);
                files.AddRange(filterFiles);
            }

            // Sort them
            files.Sort(FileSystemInfoComparer.Comparer);

            // Throw out duplicates
            IFileInfo previousFile = null;
            for (int i = 0; i < files.Count; /*Incremented in the loop*/)
            {
                IFileInfo currentFile = files[i];
                if (previousFile != null && FileSystemInfoComparer.Comparer.Compare(currentFile, previousFile) == 0)
                {
                    files.RemoveAt(i);

                    // Don't increment i;
                }
                else
                {
                    previousFile = currentFile;
                    i++;
                }
            }

            // Exclude these files if necessary
            if (this.include)
            {
                return files.ToArray();
            }
            else
            {
                IFileInfo[] allFiles = directory.GetFiles();
                Array.Sort(allFiles, FileSystemInfoComparer.FileComparer);

                List<IFileInfo> filesToInclude = new List<IFileInfo>();
                int numExcludes = files.Count;
                int numTotal = allFiles.Length;
                int e = 0;
                for (int a = 0; a < numTotal; a++)
                {
                    int compareResult = -1;
                    IFileInfo fileA = allFiles[a];
                    if (e < numExcludes)
                    {
                        IFileInfo fileE = files[e];
                        compareResult = FileSystemInfoComparer.Comparer.Compare(fileA, fileE);
                    }

                    if (compareResult == 0)
                    {
                        // Don't put this match in the results.
                        e++;
                    }
                    else
                    {
                        filesToInclude.Add(fileA);
                    }
                }

                return filesToInclude.ToArray();
            }
        }

        #endregion
    }
}
