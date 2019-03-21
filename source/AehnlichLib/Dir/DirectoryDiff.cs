namespace AehnlichLib.Dir
{
    using AehnlichLib.Dir.Merge;
    using AehnlichLib.Interfaces;
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
        public IDirectoryDiffRoot Execute(string directoryA, string directoryB)
        {
            try
            {
                var dirA = new DirectoryInfo(directoryA);
                var dirB = new DirectoryInfo(directoryB);
                
                if (dirA.Exists == false || dirB.Exists == false)
                    return null;
                
                return this.Execute(dirA, dirB);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Compares <paramref name="directoryA"/> with <paramref name="directoryB"/> using the comparison
        /// options as defined in the constructor of this class.
        /// </summary>
        /// <param name="directoryA"></param>
        /// <param name="directoryB"></param>
        /// <returns></returns>
        public IDirectoryDiffRoot Execute(DirectoryInfo directoryA, DirectoryInfo directoryB)
        {
            // Non-recursive diff match
            var diffRoot = this.BuildSubDirs(directoryA, directoryB, this._Recursive, this._Filter);
            this.AddFiles(diffRoot);

            return diffRoot;
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
        /// <param name="directoryA"></param>
        /// <param name="directoryB"></param>
        /// <param name="recursive"></param>
        /// <param name="filter"></param>
        /// <returns>A root diff entry that describes directory differences through its properties.</returns>
        private DirectoryDiffRoot BuildSubDirs(DirectoryInfo directoryA,
                                               DirectoryInfo directoryB,
											   bool recursive,
											   DirectoryDiffFileFilter filter)
        {
            var queue = new Queue<Tuple<int, MergedEntry>>();
            var index = new Dictionary<string, IDirectoryDiffEntry>();

            // Create a faux base entry to pass to Execute
            DirectoryDiffRoot diffRoot = new DirectoryDiffRoot(directoryA.FullName, directoryB.FullName, recursive, filter);

            // Associate root level entry with empty path since path associations
            // below works with RELATIVE path references to given root entries
            index.Add(string.Empty, diffRoot.RootEntry);

            // Assign base directories of level order traversal
            var root = new MergedEntry(directoryA, directoryB);

            queue.Enqueue(new Tuple<int, MergedEntry>(0, root));

            while (queue.Count() > 0)
            {
                var queueItem = queue.Dequeue();
                int iLevel = queueItem.Item1;
                MergedEntry current = queueItem.Item2;

                if (iLevel > 0)
                {
                    string basePath = current.GetBasePath(diffRoot.RootPathA, diffRoot.RootPathB);
                    string parentPath = GetParentPath(basePath);

                    IDirectoryDiffEntry parentItem;
                    if (index.TryGetValue(parentPath, out parentItem) == true)
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
                        // parentPath should always been pushed before since we do Level Order Traversal
                        // So, it must be available here - something is horribly wrong if we ever got here
                        throw new NotSupportedException(string.Format("ParentPath '{0}', BasePath '{1}'"
                                                                    , parentPath, basePath));
                    }
                }

                if (_Recursive || iLevel == 0)
                {
                    // Process the node if either sub-directory has children
                    DirectoryInfo[] directoriesA = null;
                    DirectoryInfo[] directoriesB = null;

                    // Get the arrays of subdirectories and merge them into 1 list
                    if (current.InfoA != null)
                        directoriesA = ((DirectoryInfo)current.InfoA).GetDirectories();
                    else
                        directoriesA = new DirectoryInfo[] { };

                    if (current.InfoB != null)
                        directoriesB = ((DirectoryInfo)current.InfoB).GetDirectories();
                    else
                        directoriesB = new DirectoryInfo[] { };

                    // Merge them and Diff them
                    var mergeIdx = new Merge.MergeIndex(directoriesA, directoriesB, false);
                    mergeIdx.Merge();

                    foreach (var item in mergeIdx.MergedEntries)
                        queue.Enqueue(new Tuple<int, MergedEntry>(iLevel + 1, item));
                }
            }
            
            return diffRoot;
        }


        /// <summary>
        /// Gets the parent directory of a given directory based
        /// on the last observed delimiter (if any).
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Parent directory or empty string if given path contains not delimiter.</returns>
        private string GetParentPath(string path)
        {
            if (path.Contains('\\') == false)
                return string.Empty;

            return path.Substring(0, path.LastIndexOf('\\'));
        }

        /// <summary>
        /// Converts a <see cref="MergedEntry"/> <paramref name="item"/> into a
        /// <see cref="IDirectoryDiffEntry"/> item and returns it.
        /// </summary>
        /// <param name="basePath"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private IDirectoryDiffEntry ConvertDirEntry(string basePath, MergedEntry item)
        {
            DirectoryDiffEntry newEntry = null;

            DateTime lastUpdateA = default(DateTime);
            DateTime lastUpdateB = default(DateTime);

            if (item.InfoA != null)
            {
                lastUpdateA = item.InfoA.LastWriteTime;
            }

            if (item.InfoB != null)
            {
                lastUpdateB = item.InfoB.LastWriteTime;
            }

            if (item.InfoA != null && item.InfoB != null)
            {
                // The item is in both directories
                if (this._ShowDifferent || this._ShowSame)
                {
                    newEntry = new DirectoryDiffEntry(basePath, item.InfoA.Name, false, true, true,
                                                      lastUpdateA, lastUpdateB);
                }
            }
            else if (item.InfoA != null && item.InfoB == null)
            {
                // The item is only in A
                if (this._ShowOnlyInA)
                {
                    newEntry = new DirectoryDiffEntry(basePath, item.InfoA.Name, false, true, false,
                                                      lastUpdateA, lastUpdateB);
                }
            }
            else
            {
                // The item is only in B
                if (this._ShowOnlyInB)
                {
                    newEntry = new DirectoryDiffEntry(basePath, item.InfoB.Name, false, false, true,
                                                      lastUpdateA, lastUpdateB);
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
        /// to aggregate results (sub-directory is different, size) up-wards through the hierarchy.
        /// </summary>
        /// <param name="root"></param>
        private void AddFiles(DirectoryDiffRoot root)
        {
            // If the base paths are the same, we don't need to check for file differences.
            bool checkIfFilesAreDifferent = string.Compare(root.RootPathA, root.RootPathB, true) != 0;

            var toVisit = new Stack<IDirectoryDiffEntry>();
            var visitedAncestors = new Stack<IDirectoryDiffEntry>();

            toVisit.Push(root.RootEntry);
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

                // Load files only if either directory is available and recursion is on
                // -> Load files only for root directory if recursion is turned off
                if ((node.InA == true || node.InB == true) && (this._Recursive || string.IsNullOrEmpty(node.BasePath)))
                {
                    if (node.CountSubDirectories() > 0)
                    {
                        foreach (var item in node.Subentries) // Aggregate size of sub-directories up
                        {
                            if (node.InA == true)
                                node.LengthA += item.LengthA;

                            if (node.InB == true)
                                node.LengthB += item.LengthB;
                        }
                    }

                    string sDirA, sDirB;
                    DirectoryInfo dirA = null, dirB = null;
                    bool dirA_Exists = false;
                    bool dirB_Exists = false;

                    try
                    {
                        if (node.InA)
                        {
                            sDirA = System.IO.Path.Combine(root.RootPathA, node.BasePath);
                            dirA = new DirectoryInfo(sDirA);
                            dirA_Exists = dirA.Exists;
                        }
                    }
                    catch // System.IO may throw on non-existing, authorization issues
                    {    // So, we catch it it and ignore these for now
                        dirA_Exists = false;
                    }

                    try
                    {
                        if (node.InB)
                        {
                            sDirB = System.IO.Path.Combine(root.RootPathB, node.BasePath);
                            dirB = new DirectoryInfo(sDirB);
                            dirB_Exists = dirB.Exists;
                        }
                    }
                    catch // System.IO may throw on non-existing, authorization issues
                    {    // So, we catch it it and ignore these for now
                        dirB_Exists = false;
                    }


                    if (dirA_Exists == true || dirB_Exists == true)
                    {
                        // Get the arrays of files and merge them into 1 list
                        FileInfo[] filesA, filesB;
                        Merge.MergeIndex mergeIdx = null;
                        if (root.Filter == null)
                        {
                            if (dirA_Exists)
                                filesA = dirA.GetFiles();
                            else
                                filesA = new FileInfo[] { };

                            if (dirB_Exists)
                                filesB = dirB.GetFiles();
                            else
                                filesB = new FileInfo[] { };

                            mergeIdx = new Merge.MergeIndex(filesA, filesB, false);
                        }
                        else
                        {
                            if (dirA_Exists)
                                filesA = root.Filter.Filter(dirA);
                            else
                                filesA = new FileInfo[] { };

                            if (dirB_Exists)
                                filesB = root.Filter.Filter(dirB);
                            else
                                filesB = new FileInfo[] { };

                            // Assumption: Filter generates sorted entries
                            mergeIdx = new Merge.MergeIndex(filesA, filesB, true);
                        }

                        // Merge and Diff them
                        mergeIdx.Merge();
                        double lengthSumA, lengthSumB;
                        DiffFiles(root, mergeIdx, node, checkIfFilesAreDifferent, out lengthSumA, out lengthSumB);

                        // Add size of files to size of this directory (which includes size of sub-directories)
                        if (dirA_Exists)
                        {
                            node.LengthA += lengthSumA;
                        }

                        if (dirB_Exists)
                        {
                            node.LengthB += lengthSumB;
                        }

                        node.SetDiffBasedOnChildren(_IgnoreDirectoryComparison);
                    }
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
        /// Compares 2 sets of aligned <see cref="FileSystemInfo"/> objects and returns their
        /// status in terms of difference in the <paramref name="entry"/> parameter.
        /// </summary>
        /// <param name="root"></param>
        /// <param name="mergeIndex">Contains the 2 sets of objects to compare in a merged sorted list</param>
        /// <param name="entry">Contains the resulting list</param>
        /// <param name="checkIfFilesAreDifferent"></param>
        /// <param name="lengthSumA"></param>
        /// <param name="lengthSumB"></param>
        private void DiffFiles(DirectoryDiffRoot root,
                               Merge.MergeIndex mergeIndex,
                               IDirectoryDiffEntry entry,
                               bool checkIfFilesAreDifferent,
                               out double lengthSumA, out double lengthSumB)
        {
            lengthSumA = 0;
            lengthSumB = 0;

            foreach (var item in mergeIndex.MergedEntries)
            {
                DateTime lastUpdateA = default(DateTime);
                DateTime lastUpdateB = default(DateTime);
                double lengthA = 0.0, lengthB = 0.0;

                if (item.InfoA != null)
                {
                    lastUpdateA = item.InfoA.LastWriteTime;

                    try
                    {
                        if (item.InfoA is FileInfo)
                        {
                            lengthA = ((FileInfo)item.InfoA).Length;
                            lengthSumA += lengthA;
                        }
                    }
                    catch
                    {
                        lengthA = 0;
                    }
                }

                if (item.InfoB != null)
                {
                    lastUpdateB = item.InfoB.LastWriteTime;

                    try
                    {
                        if (item.InfoB is FileInfo)
                        {
                            lengthB = ((FileInfo)item.InfoB).Length;
                            lengthSumB += lengthB;
                        }
                    }
                    catch
                    {
                        lengthB = 0;
                    }
                }

                string basePath = item.GetBasePath(root.RootPathA, root.RootPathB);
                IDirectoryDiffEntry newEntry = null;
                
                if (item.InfoA != null && item.InfoB != null)
                {
                    // The item is in both directories
                    if (_ShowDifferent || _ShowSame)
                    {
                        bool different = false;
                        Exception except = null;

                        if (checkIfFilesAreDifferent)
                        {
                            try
                            {
                                different = DiffUtility.AreFilesDifferent((FileInfo)item.InfoA, (FileInfo)item.InfoB);
                            }
                            catch (IOException ex)
                            {
                                except = ex;
                            }
                            catch (UnauthorizedAccessException ex)
                            {
                                except = ex;
                            }
                        }

                        if ((different && _ShowDifferent) || (!different && _ShowSame))
                        {
                            newEntry = new DirectoryDiffEntry(basePath, item.InfoA.Name, true, true, true,
                                                              lastUpdateA, lastUpdateB, lengthA, lengthB);

                            newEntry.Different = different;
                            
                            if (except != null)
                                newEntry.Error = string.Format("'{0}' -> '{1}'", except.Message, except.StackTrace);
                        }
                    }
                }
                else if (item.InfoA != null && item.InfoB == null)
                {
                    // The item is only in A
                    if (this._ShowOnlyInA)
                    {
                        newEntry = new DirectoryDiffEntry(basePath, item.InfoA.Name, true, true, false,
                                                          lastUpdateA, default(DateTime), lengthA, 0.0);
                    }
                }
                else
                {
                    // The item is only in B
                    if (this._ShowOnlyInB)
                    {
                        newEntry = new DirectoryDiffEntry(basePath, item.InfoB.Name, true, false, true,
                                                          default(DateTime), lastUpdateB, 0.0, lengthB);
                        
                    }
                }
                
                if (newEntry != null)
                {
                    // Mark directory as different if containing files are different
                    if (newEntry.Different == true)
                    {
                        entry.Different = true;
                        root.AddDiffFile(newEntry);   // Add into collection of different files
                    }
                    
                    entry.AddSubEntry(newEntry);
                }
            }
        }
        #endregion Aggregate Dir Contents - Post Order
        #endregion
    }
}
