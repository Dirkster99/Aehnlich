namespace DiffLib.Dir
{
    #region Using Directives

    using System;
    using System.IO;

    #endregion

    public sealed class DirectoryDiff
	{
		#region Private Data Members

		private readonly bool ignoreDirectoryComparison;
		private readonly bool recursive;
		private readonly bool showDifferent;
		private readonly bool showOnlyInA;
		private readonly bool showOnlyInB;
		private readonly bool showSame;
		private readonly DirectoryDiffFileFilter filter;

		#endregion

		#region Constructors

		public DirectoryDiff(
			bool showOnlyInA,
			bool showOnlyInB,
			bool showDifferent,
			bool showSame,
			bool recursive,
			bool ignoreDirectoryComparison,
			DirectoryDiffFileFilter filter)
		{
			this.showOnlyInA = showOnlyInA;
			this.showOnlyInB = showOnlyInB;
			this.showDifferent = showDifferent;
			this.showSame = showSame;
			this.recursive = recursive;
			this.ignoreDirectoryComparison = ignoreDirectoryComparison;
			this.filter = filter;
		}

		#endregion

		#region Public Methods

		public DirectoryDiffResults Execute(string directoryA, string directoryB) => this.Execute(new DirectoryInfo(directoryA), new DirectoryInfo(directoryB));

		public DirectoryDiffResults Execute(DirectoryInfo directoryA, DirectoryInfo directoryB)
		{
			// Create a faux base entry to pass to Execute
			DirectoryDiffEntry entry = new DirectoryDiffEntry(string.Empty, false, true, true, false);

			// If the base paths are the same, we don't need to check for file differences.
			bool checkIfFilesAreDifferent = string.Compare(directoryA.FullName, directoryB.FullName, true) != 0;

			this.Execute(directoryA, directoryB, entry, checkIfFilesAreDifferent);

			DirectoryDiffResults results = new DirectoryDiffResults(directoryA, directoryB, entry.Subentries, this.recursive, this.filter);
			return results;
		}

		#endregion

		#region Private Methods

		private void DiffFileSystemInfos(FileSystemInfo[] infosA, FileSystemInfo[] infosB, DirectoryDiffEntry entry, bool isFile, bool checkIfFilesAreDifferent)
		{
			int indexA = 0;
			int indexB = 0;
			int countA = infosA.Length;
			int countB = infosB.Length;
			while (indexA < countA && indexB < countB)
			{
				FileSystemInfo infoA = infosA[indexA];
				FileSystemInfo infoB = infosB[indexB];

				int compareResult = string.Compare(infoA.Name, infoB.Name, true);
				if (compareResult == 0)
				{
					// The item is in both directories
					if (this.showDifferent || this.showSame)
					{
						bool different = false;
						DirectoryDiffEntry newEntry = new DirectoryDiffEntry(infoA.Name, isFile, true, true, false);

						if (isFile)
						{
							if (checkIfFilesAreDifferent)
							{
								try
								{
									different = DiffUtility.AreFilesDifferent((FileInfo)infoA, (FileInfo)infoB);
								}
								catch (IOException ex)
								{
									newEntry.Error = ex.Message;
								}
								catch (UnauthorizedAccessException ex)
								{
									newEntry.Error = ex.Message;
								}

								newEntry.Different = different;
							}

							if ((different && this.showDifferent) || (!different && this.showSame))
							{
								entry.Subentries.Add(newEntry);
							}
						}
						else
						{
							if (this.recursive)
							{
								this.Execute((DirectoryInfo)infoA, (DirectoryInfo)infoB, newEntry, checkIfFilesAreDifferent);
							}

							if (this.ignoreDirectoryComparison)
							{
								newEntry.Different = false;
							}
							else
							{
								different = newEntry.Different;
							}

							if (this.ignoreDirectoryComparison || (different && this.showDifferent) || (!different && this.showSame))
							{
								entry.Subentries.Add(newEntry);
							}
						}

						if (different)
						{
							entry.Different = true;
						}
					}

					indexA++;
					indexB++;
				}
				else if (compareResult < 0)
				{
					// The item is only in A
					if (this.showOnlyInA)
					{
						entry.Subentries.Add(new DirectoryDiffEntry(infoA.Name, isFile, true, false, false));
						entry.Different = true;
					}

					indexA++;
				}
				else
				{
					// iCompareResult > 0
					// The item is only in B
					if (this.showOnlyInB)
					{
						entry.Subentries.Add(new DirectoryDiffEntry(infoB.Name, isFile, false, true, false));
						entry.Different = true;
					}

					indexB++;
				}
			}

			// Add any remaining entries
			if (indexA < countA && this.showOnlyInA)
			{
				for (int i = indexA; i < countA; i++)
				{
					entry.Subentries.Add(new DirectoryDiffEntry(infosA[i].Name, isFile, true, false, false));
					entry.Different = true;
				}
			}
			else if (indexB < countB && this.showOnlyInB)
			{
				for (int i = indexB; i < countB; i++)
				{
					entry.Subentries.Add(new DirectoryDiffEntry(infosB[i].Name, isFile, false, true, false));
					entry.Different = true;
				}
			}
		}

		private void Execute(DirectoryInfo directoryA,
                             DirectoryInfo directoryB,
                             DirectoryDiffEntry entry,
                             bool checkIfFilesAreDifferent)
		{
			// Get the arrays of files
			FileInfo[] filesA, filesB;
			if (this.filter == null)
			{
				filesA = directoryA.GetFiles();
				filesB = directoryB.GetFiles();

				// Sort them
				Array.Sort(filesA, FileSystemInfoComparer.FileComparer);
				Array.Sort(filesB, FileSystemInfoComparer.FileComparer);
			}
			else
			{
				filesA = this.filter.Filter(directoryA);
				filesB = this.filter.Filter(directoryB);
			}

			// Diff them
			this.DiffFileSystemInfos(filesA, filesB, entry, true, checkIfFilesAreDifferent);

			// Get the arrays of subdirectories
			DirectoryInfo[] directoriesA = directoryA.GetDirectories();
			DirectoryInfo[] directoriesB = directoryB.GetDirectories();

			// Sort them
			Array.Sort(directoriesA, FileSystemInfoComparer.DirectoryComparer);
			Array.Sort(directoriesB, FileSystemInfoComparer.DirectoryComparer);

			// Diff them
			this.DiffFileSystemInfos(directoriesA, directoriesB, entry, false, checkIfFilesAreDifferent);
		}

		#endregion
	}
}
