namespace AehnlichLib.Dir.Merge
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;

    /// <summary>
    /// Implements a merge algorithm to give all common entries (entries with same name)
    /// the same index while all other entries (occurring only in A or only in B) are at
    /// an index where the other item is null.
    /// </summary>
    internal class MergeIndex
    {
        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="infosA"></param>
        /// <param name="infosB"></param>
        /// <param name="isSorted"></param>
        public MergeIndex(FileSystemInfo[] infosA,
                          FileSystemInfo[] infosB,
                          bool isSorted)
            : this()
        {
            this.InfosA = infosA;
            this.InfosB = infosB;
            this.IsSorted = isSorted;
        }

        /// <summary>
        /// Hidden standard constructor
        /// </summary>
        protected MergeIndex()
        {
        }
        #endregion ctors

        #region properties
        public FileSystemInfo[] InfosA { get; }

        public FileSystemInfo[] InfosB { get; }

        public bool IsSorted { get; protected set; }

        public List<MergedEntry> MergedEntries { get; protected set; }
        #endregion properties

        /// <summary>
        /// Method merges the lists in <see cref="InfosA"/> and <see cref="InfosB"/> by
        /// - sorting (if necessary) both lists and
        /// - comparing their items in each index and
        /// - inserting empty items in <see cref="InfosA"/> or <seealso cref="InfosB"/>
        /// </summary>
        /// <returns>Resulting number of merged entries - this should always be:
        /// max(<seealso cref="InfosA"/>.Count, <seealso cref="InfosB"/>.Count)</returns>
        public int Merge()
        {
            // Not much to merge here
            if (InfosA.Length == 0 && InfosB.Length == 0)
            {
                MergedEntries = new List<MergedEntry>();
                return 0;
            }

            if (IsSorted == false)
            {
                // Are we sort/merging directories or files?
                if (InfosA is DirectoryInfo[] && InfosB is DirectoryInfo[])
                {
                    // Sort them
                    Array.Sort((DirectoryInfo[])InfosA, FileSystemInfoComparer.DirectoryComparer);
                    Array.Sort((DirectoryInfo[])InfosB, FileSystemInfoComparer.DirectoryComparer);
                }
                else
                {
                    // Sort them
                    Array.Sort((FileInfo[])InfosA, FileSystemInfoComparer.FileComparer);
                    Array.Sort((FileInfo[])InfosB, FileSystemInfoComparer.FileComparer);
                }

                IsSorted = true;
            }

            List<MergedEntry> mergedEntries = new List<MergedEntry>();

            int indexA = 0;
            int indexB = 0;

            int countA = InfosA.Length;
            int countB = InfosB.Length;

            // Go through each line and align (merge) it with the other
            while (indexA < countA && indexB < countB)
            {
                FileSystemInfo infoA = InfosA[indexA];
                FileSystemInfo infoB = InfosB[indexB];

                int compareResult = string.Compare(infoA.Name, infoB.Name, true);
                if (compareResult == 0)
                {
                    // The item is in both directories
                    mergedEntries.Add(new MergedEntry(infoA, infoB));
                    indexA++;
                    indexB++;
                }
                else if (compareResult < 0)
                {
                    // iCompareResult < 0 -> the item is only in A
                    mergedEntries.Add(new MergedEntry(infoA, null));
                    indexA++;
                }
                else
                {
                    // iCompareResult > 0 -> The item is only in B
                    mergedEntries.Add(new MergedEntry(null, infoB));
                    indexB++;
                }
            }

            // Add any remaining entries
            if (indexA < countA)
            {
                for (; indexA < countA; indexA++)
                {
                    mergedEntries.Add(new MergedEntry(InfosA[indexA++], null));
                }
            }
            else if (indexB < countB)
            {
                for (; indexB < countB; indexB++)
                {
                    mergedEntries.Add(new MergedEntry(null, InfosB[indexB++]));
                }
            }

            Debug.Assert(mergedEntries.Count <= (InfosA.Length + InfosB.Length));
            MergedEntries = mergedEntries;

            return mergedEntries.Count;
        }
    }
}
