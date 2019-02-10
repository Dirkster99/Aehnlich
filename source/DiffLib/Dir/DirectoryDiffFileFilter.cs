namespace DiffLib.Dir
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.IO;

	#endregion

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

		public string FilterString => this.concatenatedFilters;

		public bool Include => this.include;

		#endregion

		#region Public Methods

		public FileInfo[] Filter(DirectoryInfo directory)
		{
			// Get all the files that match the filters
			List<FileInfo> files = new List<FileInfo>();
			foreach (string filter in this.individualFilters)
			{
				FileInfo[] filterFiles = directory.GetFiles(filter);
				files.AddRange(filterFiles);
			}

			// Sort them
			files.Sort(FileSystemInfoComparer.Comparer);

			// Throw out duplicates
			FileInfo previousFile = null;
			for (int i = 0; i < files.Count; /*Incremented in the loop*/)
			{
				FileInfo currentFile = (FileInfo)files[i];
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
				FileInfo[] allFiles = directory.GetFiles();
				Array.Sort(allFiles, FileSystemInfoComparer.FileComparer);

				List<FileInfo> filesToInclude = new List<FileInfo>();
				int numExcludes = files.Count;
				int numTotal = allFiles.Length;
				int e = 0;
				for (int a = 0; a < numTotal; a++)
				{
					int compareResult = -1;
					FileInfo fileA = allFiles[a];
					if (e < numExcludes)
					{
						FileInfo fileE = (FileInfo)files[e];
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
