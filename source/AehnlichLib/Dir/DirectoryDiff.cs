namespace AehnlichLib.Dir
{
    using System;
    using System.IO;

    /// <summary>
    /// Compares sub-directories and files in 2 given folders and returns the result of the comparison
    /// via Execute(...) method.
    /// </summary>
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
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="showOnlyInA">
        /// Determines whether comparison results should only be relevant for the left view A.
        /// 
        /// Setting this to false will ignore differences that are relevant for B only. That is,
        /// directories that are missing in view B will not be flagged as difference.
        /// </param>
        /// <param name="showOnlyInB">
        /// Determines whether comparison results should only be relevant for the right view B.
        /// 
        /// Setting this to false will ignore differences that are relevant for A only. That is,
        /// directories that are missing in view A will not be flagged as difference.
        /// </param>
        /// <param name="showDifferent">
        /// Determines whether the result set also contains similar items, such as folders with
        /// similar or the same content, or whether the result contains only items that are present
        /// in one view but not the other view.
        /// 
        /// Setting this to false to generates only insert/delete item differences in the result.
        /// Set it to true to generate insert/delete and update (similar) item differences in
        /// the result set.
        /// </param>
        /// <param name="showSame">
        /// Determines whether the result set also contains equal items or not.
        /// 
        /// Set this to false to generate a result set that contains only
        /// insert/delete/update item differences but no equal items
        /// (equal items are not part of the result set even if they exists).
        /// Setting this to true also includes equal items in the generated result set.
        /// </param>
        /// <param name="recursive">Determines whether or not only files and folders in a given directory
        /// are compared without comparing contents of sub-directories.</param>
        /// <param name="ignoreDirectoryComparison">Determines whether sub-directories with different content
        /// (both sub-directories exist in both directories A and B) are flagged as different if they contain
        /// different files/directories or not.
        /// 
        /// Setting this parameter to true ignores directories completely (they are never flagged as different).
        /// Setting it to false leads to generating entries with a hint towards difference even for directories.</param>
        /// <param name="filter">
        /// This can be used to setup a file filter that determines the type of file(s)
        /// that can be included or excluded in the comaparison of directories.
        /// 
        /// Setting this for instance to DirectoryDiffFileFilter("*.cs", true) leads
        /// to comparing only C-Sharp files (all other files are ignored).
        /// 
        /// And directories are equal if they contain the same sub-directory structure
        /// and either no C-Sharp files or equal C-Sharp files.
        /// </param>
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

        /// <summary>
        /// Hidden standard constructor
        /// </summary>
        private DirectoryDiff() { }
		#endregion

		#region Public Methods
        /// <summary>
        /// Compares <paramref name="directoryA"/> with <paramref name="directoryB"/> using the comparison
        /// options as defined in the constructor of this class.
        /// </summary>
        /// <param name="directoryA"></param>
        /// <param name="directoryB"></param>
        /// <returns></returns>
		public DirectoryDiffResults Execute(string directoryA, string directoryB) => this.Execute(new DirectoryInfo(directoryA), new DirectoryInfo(directoryB));

        /// <summary>
        /// Compares <paramref name="directoryA"/> with <paramref name="directoryB"/> using the comparison
        /// options as defined in the constructor of this class.
        /// </summary>
        /// <param name="directoryA"></param>
        /// <param name="directoryB"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Compares 2 sets of <see cref="FileSystemInfo"/> objects and returns their
        /// status in terms of difference in the <paramref name="entry"/> parameter.
        /// </summary>
        /// <param name="mergeIndex">Contains the 2 sets of objects to compare in a merged sorted list</param>
        /// <param name="entry">Contains the resulting list</param>
        /// <param name="checkIfFilesAreDifferent"></param>
        private void DiffDirectories(Merge.MergeIndex mergeIndex,
                                      DirectoryDiffEntry entry,
                                      bool checkIfFilesAreDifferent)
        {
            foreach (var item in mergeIndex.MergedEntries)
            {
                if (item.InfoA != null && item.InfoB != null)
                {
                    // The item is in both directories
                    // The item is in both directories
                    if (this.showDifferent || this.showSame)
                    {
                        bool different = false;
                        DirectoryDiffEntry newEntry = new DirectoryDiffEntry(item.InfoA.Name, false, true, true, false);

                        if (this.recursive)
                        {
                            this.Execute((DirectoryInfo)item.InfoA, (DirectoryInfo)item.InfoB, newEntry, checkIfFilesAreDifferent);
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

                        if (different)
                        {
                            entry.Different = true;
                        }
                    }


                }
                else if (item.InfoA != null && item.InfoB == null)
                {
                    // The item is only in A
                    if (this.showOnlyInA)
                    {
                        entry.Subentries.Add(new DirectoryDiffEntry(item.InfoA.Name, false, true, false, false));
                        entry.Different = true;
                    }
                }
                else
                {
                    // The item is only in B (both itemA and itemB null is not supported)
                    if (this.showOnlyInB)
                    {
                        entry.Subentries.Add(new DirectoryDiffEntry(item.InfoB.Name, false, false, true, false));
                        entry.Different = true;
                    }
                }
            }
        }

        /// <summary>
        /// Compares 2 sets of <see cref="FileSystemInfo"/> objects and returns their
        /// status in terms of difference in the <paramref name="entry"/> parameter.
        /// </summary>
        /// <param name="mergeIndex">Contains the 2 sets of objects to compare in a merged sorted list</param>
        /// <param name="entry">Contains the resulting list</param>
        /// <param name="checkIfFilesAreDifferent"></param>
        private void DiffFiles(Merge.MergeIndex mergeIndex,
                               DirectoryDiffEntry entry,
                               bool checkIfFilesAreDifferent)
        {
            foreach (var item in mergeIndex.MergedEntries)
            {
                if (item.InfoA != null && item.InfoB != null)
                {
                    // The item is in both directories
                    if (this.showDifferent || this.showSame)
                    {
                        bool different = false;
                        DirectoryDiffEntry newEntry = new DirectoryDiffEntry(item.InfoA.Name, true, true, true, false);

                        if (checkIfFilesAreDifferent)
                        {
                            try
                            {
                                different = DiffUtility.AreFilesDifferent((FileInfo)item.InfoA, (FileInfo)item.InfoB);
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

                        if (different)
                        {
                            entry.Different = true;
                        }
                    }
                }
                else if (item.InfoA != null && item.InfoB == null)
                {
                    // The item is only in A
                    if (this.showOnlyInA)
                    {
                        entry.Subentries.Add(new DirectoryDiffEntry(item.InfoA.Name, true, true, false, false));
                        entry.Different = true;
                    }
                }
                else
                {
                    // The item is only in B
                    if (this.showOnlyInB)
                    {
                        entry.Subentries.Add(new DirectoryDiffEntry(item.InfoB.Name, true, false, true, false));
                        entry.Different = true;
                    }
                }
            }
        }

        private void Execute(DirectoryInfo directoryA,
                             DirectoryInfo directoryB,
                             DirectoryDiffEntry entry,
                             bool checkIfFilesAreDifferent)
		{
            {
                // Get the arrays of subdirectories and merge them into 1 list
                DirectoryInfo[] directoriesA = directoryA.GetDirectories();
                DirectoryInfo[] directoriesB = directoryB.GetDirectories();

                // Merge them and Diff them
                var mergeIdx = new Merge.MergeIndex(directoriesA, directoriesB, false);
                mergeIdx.Merge();
                this.DiffDirectories(mergeIdx, entry, checkIfFilesAreDifferent);
            }

            {
                // Get the arrays of files and merge them into 1 list
                FileInfo[] filesA, filesB;
                Merge.MergeIndex mergeIdx = null;
                if (this.filter == null)
                {
                    filesA = directoryA.GetFiles();
                    filesB = directoryB.GetFiles();

                    mergeIdx = new Merge.MergeIndex(filesA, filesB, false);
                }
                else
                {
                    filesA = this.filter.Filter(directoryA);
                    filesB = this.filter.Filter(directoryB);

                    // Assumption: Filter generates sorted entries
                    mergeIdx = new Merge.MergeIndex(filesA, filesB, true);
                }

                // Merge and Diff them
                mergeIdx.Merge();
                DiffFiles(mergeIdx, entry, checkIfFilesAreDifferent);
            }
        }
		#endregion
	}
}
