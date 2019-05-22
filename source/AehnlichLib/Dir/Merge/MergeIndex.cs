namespace AehnlichLib.Dir.Merge
{
    using AehnlichLib.Interfaces.Dir;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

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
        public MergeIndex(IFileSystemInfo[] infosA,
                          IFileSystemInfo[] infosB,
                          bool isSorted,
                          bool showOnlyInA,
                          bool showOnlyInB)
            : this()
        {
            this.InfosA = infosA;
            this.InfosB = infosB;
            this.IsSorted = isSorted;

            this.ShowOnlyInA = showOnlyInA;
            this.ShowOnlyInB = showOnlyInB;
        }

        /// <summary>
        /// Hidden standard constructor
        /// </summary>
        protected MergeIndex()
        {
        }
        #endregion ctors

        #region properties
        public IFileSystemInfo[] InfosA { get; }

        public IFileSystemInfo[] InfosB { get; }

        public int InfosA_Length { get { return (InfosA == null ? 0 : InfosA.Length); } }

        public int InfosB_Length { get { return (InfosB == null ? 0 : InfosB.Length); } }

        public bool IsSorted { get; protected set; }

        public bool ShowOnlyInA { get; }

        public bool ShowOnlyInB { get; }

        public List<MergedEntry> MergedEntries { get; protected set; }
        #endregion properties

        /// <summary>
        /// Method merges the lists in <see cref="InfosA"/> and <see cref="InfosB"/> by
        /// - sorting (if necessary) both lists and
        /// - comparing their items in each index and
        /// - inserting empty items in <see cref="InfosA"/> or <seealso cref="InfosB"/>
        /// </summary>
        /// <returns>Resulting number of merged entries - this should always be:
        /// max(<seealso cref="InfosA_Length"/>, <seealso cref="InfosB_Length"/>)</returns>
        public int Merge()
        {
            // Not much to merge here
            if (InfosA_Length == 0 && InfosB_Length == 0)
            {
                MergedEntries = new List<MergedEntry>();
                return 0;
            }

            // Sort entries alphabetically if not already sorted (pre-condition for merge)
            if (IsSorted == false)
            {
                // Sort directories/files using their common base implementation
                if (InfosA_Length > 0)
                    Array.Sort((IFileSystemInfo[])InfosA, FileSystemInfoComparer.Comparer);

                if (InfosB_Length > 0)
                    Array.Sort((IFileSystemInfo[])InfosB, FileSystemInfoComparer.Comparer);

                IsSorted = true;
            }

            List<MergedEntry> mergedEntries = new List<MergedEntry>();

            int indexA = 0;
            int indexB = 0;

            int countA = InfosA_Length;
            int countB = InfosB_Length;

            // Go through each line and align (merge) it with the other
            while (indexA < countA && indexB < countB)
            {
                IFileSystemInfo infoA = InfosA[indexA];
                IFileSystemInfo infoB = InfosB[indexB];

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
                    if (ShowOnlyInA == true)
                    {
                        mergedEntries.Add(new MergedEntry(infoA, null));
                    }

                    indexA++;
                }
                else
                {
                    // iCompareResult > 0 -> The item is only in B
                    if (ShowOnlyInB == true)
                    {
                        mergedEntries.Add(new MergedEntry(null, infoB));
                    }

                    indexB++;
                }
            }

            if (indexA < countA && ShowOnlyInA == true)
            {
                // Add any remaining entries that are only one the left
                for (; indexA < countA; indexA++)
                {
                    mergedEntries.Add(new MergedEntry(InfosA[indexA], null));
                }
            }
            else if (indexB < countB && ShowOnlyInB == true)
            {
                // Add any remaining entries that are only one the right
                for (; indexB < countB; indexB++)
                {
                    mergedEntries.Add(new MergedEntry(null, InfosB[indexB]));
                }
            }

            Debug.Assert(mergedEntries.Count <= (InfosA_Length + InfosB_Length));
            MergedEntries = mergedEntries;

            return mergedEntries.Count;
        }
    }
}
