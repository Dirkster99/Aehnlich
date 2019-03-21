namespace AehnlichLib.Dir.Merge
{
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    [DebuggerDisplay("InfoA = {InfoA}, InfoB = {InfoB}, BothGotChildren = {BothGotChildren}")]
    internal class MergedEntry
    {
        #region ctors
        public MergedEntry(FileSystemInfo infoA,
                           FileSystemInfo infoB)
            : this()
        {
            this.InfoA = infoA;
            this.InfoB = infoB;
        }

        /// <summary>
        /// Hidden standard constructor
        /// </summary>
        protected MergedEntry()
        {
        }
        #endregion ctors


        #region properties
        public FileSystemInfo InfoA { get; }

        public FileSystemInfo InfoB { get; }

        public bool BothGotChildren
        {
            get
            {
                var dirA = InfoA as DirectoryInfo;
                var dirB = InfoB as DirectoryInfo;

                if (dirA == null || dirB == null)
                    return false;

                try
                {
                    if (dirA.Exists == false || dirB.Exists == false)
                        return false;

                    return dirA.GetDirectories().Any() && dirB.GetDirectories().Any();
                }
                catch
                {
                    // GetDirectories() can throw exceptions
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets a relative path between the given root path A and B and the sub-dir.
        /// 
        /// The relative path is equal for A and B if this directory occurs in A and B
        /// in the same relative spot.
        /// </summary>
        /// <param name="sItemB"></param>
        /// <param name="sItemA"></param>
        /// <returns></returns>
        internal string GetBasePath(string sItemA, string sItemB)
        {
            string nameA = (this.InfoA == null ? string.Empty : this.InfoA.FullName);
            string nameB = (this.InfoB == null ? string.Empty : this.InfoB.FullName);
            string basePath = string.Empty;

            if (string.IsNullOrEmpty(nameA) == false)
                basePath = nameA.Substring(sItemA.Length + 1);
            else
                basePath = nameB.Substring(sItemB.Length + 1);

            return basePath;
        }
        #endregion properties
    }
}
