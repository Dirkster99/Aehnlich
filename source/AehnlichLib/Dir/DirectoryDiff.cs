namespace AehnlichLib.Dir
{
    using AehnlichLib.Dir.Merge;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// Compares sub-directories and files in 2 given folders and returns the result of the comparison
    /// via Execute(...) method.
    /// </summary>
    public sealed class DirectoryDiff
    {
        #region Private Data Members

        private readonly bool _IgnoreDirectoryComparison;
        private readonly bool _Recursive;
        private readonly bool _ShowDifferent;
        private readonly bool _ShowOnlyInA;
        private readonly bool _ShowOnlyInB;
        private readonly bool _ShowSame;
        private readonly DirectoryDiffFileFilter _Filter;

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
            _ShowOnlyInA = showOnlyInA;
            _ShowOnlyInB = showOnlyInB;
            _ShowDifferent = showDifferent;
            _ShowSame = showSame;
            _Recursive = recursive;
            _IgnoreDirectoryComparison = ignoreDirectoryComparison;
            _Filter = filter;
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

            // Non-recursive diff match version
            DirectoryDiffEntry rootEntry = new DirectoryDiffEntry(string.Empty, true, true, true, false);
            var mergedRootEntry = new MergedEntry(directoryA, directoryB);
            this.BuildSubDirs(mergedRootEntry, rootEntry, directoryA, directoryB);
            this.AddFiles(rootEntry, _Filter, directoryA, directoryB);

            DirectoryDiffResults results = new DirectoryDiffResults(directoryA, directoryB,
                                                                    rootEntry.Subentries, this._Recursive,
                                                                    this._Filter);

            return results;
        }

        #endregion

        #region Private Methods
        #region Build Dir Structure - Level Order
        /// <summary>
        /// Builds an initial directory structure that contains all sub-directories being contained
        /// in A and B (the structure ends at any point where a given directory occurs only in A or
        /// only in B).
        /// 
        /// The structure is build with a Level Order traversal algorithm.
        /// 
        /// Tip: Use a Post Order algorithm to look at each directory in the structure and aggregate
        ///      results up-wards within the directory structure.
        /// </summary>
        /// <param name="root"></param>
        /// <param name="rootEntry"></param>
        /// <param name="directoryA"></param>
        /// <param name="directoryB"></param>
        internal void BuildSubDirs(MergedEntry root,
                                   DirectoryDiffEntry rootEntry,
                                   DirectoryInfo directoryA, DirectoryInfo directoryB)
        {
            var queue = new Queue<Tuple<int, MergedEntry>>();
            var index = new Dictionary<string, DirectoryDiffEntry>();

            if (root != null)
                queue.Enqueue(new Tuple<int, MergedEntry>(0, root));

            while (queue.Count() > 0)
            {
                var queueItem = queue.Dequeue();
                int iLevel = queueItem.Item1;
                MergedEntry current = queueItem.Item2;

                string nameA = (current.InfoA == null ? string.Empty : current.InfoA.Name);
                string nameB = (current.InfoB == null ? string.Empty : current.InfoB.Name);
                string basePath = string.Empty;

                if (iLevel == 0)
                    index.Add(basePath, rootEntry);
                else
                {
                    basePath = current.GetBasePath(directoryA, directoryB);
                    string parentPath = GetParentPath(basePath);

                    DirectoryDiffEntry parentItem;
                    if(index.TryGetValue(parentPath, out parentItem) == true)
                    {
                        var entry = ConvertDirEntry(basePath, current);
                        if (entry != null)
                        {
                            index.Add(basePath, entry);
                            parentItem.AddSubEntry(entry);
                            parentItem.SetDiffBasedOnChildren(_IgnoreDirectoryComparison);
                        }
                    }
                    else
                    {
                        // parentPath should always been pushed before and be available here
                        // Should never get here - There is something horribly going wrong if we got here
                        throw new NotSupportedException(string.Format("ParentPath '{0}', BasePath '{1}'"
                                                                    , parentPath, basePath));
                    }
                }

                // Process the node if both sub-directories have children
                if (current.BothGotChildren && (_Recursive || iLevel == 0))
                {
                    // Get the arrays of subdirectories and merge them into 1 list
                    DirectoryInfo[] directoriesA = ((DirectoryInfo)current.InfoA).GetDirectories();
                    DirectoryInfo[] directoriesB = ((DirectoryInfo)current.InfoB).GetDirectories();

                    // Merge them and Diff them
                    var mergeIdx = new Merge.MergeIndex(directoriesA, directoriesB, false);
                    mergeIdx.Merge();

                    foreach (var item in mergeIdx.MergedEntries)
                        queue.Enqueue(new Tuple<int, MergedEntry>(iLevel + 1, item));
                }
            }
        }

        /// <summary>
        /// Gets a relative path between the given root path A and B and the sub-dir.
        /// 
        /// The relative path is equal for A and B if this directory occurs in A and B in the same spot.
        /// </summary>
        /// <param name="directoryA"></param>
        /// <param name="directoryB"></param>
        /// <param name="current"></param>
        /// <param name="iLevel"></param>
        /// <returns></returns>
        private string GetBasePath(DirectoryInfo directoryA, DirectoryInfo directoryB,
                                   MergedEntry current, int iLevel)
        {
            string nameA = (current.InfoA == null ? string.Empty : current.InfoA.FullName);
            string nameB = (current.InfoB == null ? string.Empty : current.InfoB.FullName);
            string basePath = string.Empty;

            if (iLevel == 0)
                basePath = string.Empty;
            else
            {
                if (string.IsNullOrEmpty(nameA) == false)
                    basePath = nameA.Substring(directoryA.FullName.Length + 1);
                else
                    basePath = nameB.Substring(directoryB.FullName.Length + 1);
            }

            return basePath;
        }

        private string GetParentPath(string path)
        {
            if (path.Contains('\\') == false)
                return string.Empty;

            return path.Substring(0, path.LastIndexOf('\\'));
        }

        /// <summary>
        /// Converts a <see cref="MergedEntry"/> <paramref name="item"/> into a
        /// <see cref="DirectoryDiffEntry"/> item and returns it.
        /// </summary>
        /// <param name="basePath"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private DirectoryDiffEntry ConvertDirEntry(string basePath, MergedEntry item)
        {
            DirectoryDiffEntry newEntry = null;

            if (item.InfoA != null && item.InfoB != null)
            {
                // The item is in both directories
                if (this._ShowDifferent || this._ShowSame)
                {
                    newEntry = new DirectoryDiffEntry(basePath, item.InfoA.Name, false, true, true);
                }
            }
            else if (item.InfoA != null && item.InfoB == null)
            {
                // The item is only in A
                if (this._ShowOnlyInA)
                {
                    newEntry = new DirectoryDiffEntry(basePath, item.InfoA.Name, false, true, false);
                }
            }
            else
            {
                // The item is only in B
                if (this._ShowOnlyInB)
                {
                    newEntry = new DirectoryDiffEntry(basePath, item.InfoB.Name, false, false, true);
                }
            }

            return newEntry;
        }
        #endregion Build Dir Structure - Level Order

        #region Aggregate Dir Contents - Post Order
        /// <summary>
        /// Adds files into a given <paramref name="root"/> directory structure of sub-directories
        /// and re-evaluates their status in terms of difference.
        /// 
        /// The algorithm used implements a Post-Order traversal algorithm which also allows us
        /// to aggregate results (sub-directory is different) up-wards through the hierarchy.
        /// </summary>
        /// <param name="root"></param>
        /// <param name="filter"></param>
        /// <param name="directoryA"></param>
        /// <param name="directoryB"></param>
        public void AddFiles(DirectoryDiffEntry root,
                             DirectoryDiffFileFilter filter,
                             DirectoryInfo directoryA, DirectoryInfo directoryB)
        {
            // If the base paths are the same, we don't need to check for file differences.
            bool checkIfFilesAreDifferent = string.Compare(directoryA.FullName, directoryB.FullName, true) != 0;

            var toVisit = new Stack<DirectoryDiffEntry>();
            var visitedAncestors = new Stack<DirectoryDiffEntry>();

            toVisit.Push(root);
            while (toVisit.Count > 0)
            {
                var node = toVisit.Peek();
                if (node.CountSubDirectories() > 0)
                {
                    if (PeekOrDefault(visitedAncestors) != node)
                    {
                        visitedAncestors.Push(node);
                        PushReverse(toVisit, node.Subentries.ToList());
                        continue;
                    }

                    visitedAncestors.Pop();
                }

                // Load files only if both directories are available and recursion is on
                // -> Load files only for root directory if recursion is turned off
                if (node.InA == true && node.InB == true && (this._Recursive || string.IsNullOrEmpty(node.BasePath)))
                {
                    string sDirA = System.IO.Path.Combine(directoryA.FullName, node.BasePath);
                    string sDirB = System.IO.Path.Combine(directoryB.FullName, node.BasePath);

                    var dirA = new DirectoryInfo(sDirA);
                    var dirB = new DirectoryInfo(sDirB);

                    // Get the arrays of files and merge them into 1 list
                    FileInfo[] filesA, filesB;
                    Merge.MergeIndex mergeIdx = null;
                    if (filter == null)
                    {
                        filesA = dirA.GetFiles();
                        filesB = dirB.GetFiles();

                        mergeIdx = new Merge.MergeIndex(filesA, filesB, false);
                    }
                    else
                    {
                        filesA = filter.Filter(dirA);
                        filesB = filter.Filter(dirB);

                        // Assumption: Filter generates sorted entries
                        mergeIdx = new Merge.MergeIndex(filesA, filesB, true);
                    }

                    // Merge and Diff them
                    mergeIdx.Merge();
                    DiffFiles(mergeIdx, node, checkIfFilesAreDifferent);

                    node.SetDiffBasedOnChildren(_IgnoreDirectoryComparison);
                }

                toVisit.Pop();
            }
        }

        private static T PeekOrDefault<T>(Stack<T> s)
        {
            return s.Count == 0 ? default(T) : s.Peek();
        }

        private static void PushReverse<T>(Stack<T> s, List<T> list)
        {
            foreach (var l in list.ToArray().Reverse())
            {
                s.Push(l);
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
                    if (_ShowDifferent || _ShowSame)
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

                        if ((different && _ShowDifferent) || (!different && _ShowSame))
                        {
                            entry.AddSubEntry(newEntry);
                        }

                        if (different) // Mark directory as different if files are different
                        {
                            entry.Different = true;
                        }
                    }
                }
                else if (item.InfoA != null && item.InfoB == null)
                {
                    // The item is only in A
                    if (this._ShowOnlyInA)
                    {
                        entry.AddSubEntry(new DirectoryDiffEntry(item.InfoA.Name, true, true, false, false));

                        // Mark directory as different if files are different
                        entry.Different = true;
                    }
                }
                else
                {
                    // The item is only in B
                    if (this._ShowOnlyInB)
                    {
                        entry.AddSubEntry(new DirectoryDiffEntry(item.InfoB.Name, true, false, true, false));

                        // Mark directory as different if files are different
                        entry.Different = true;
                    }
                }
            }
        }
        #endregion Aggregate Dir Contents - Post Order
        #endregion
    }
}
