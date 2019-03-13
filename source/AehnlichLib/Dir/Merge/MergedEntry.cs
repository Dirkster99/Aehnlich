namespace AehnlichLib.Dir.Merge
{
    using System.IO;

    internal class MergedEntry
    {
        #region ctors
        public MergedEntry(FileSystemInfo infoA,
                           FileSystemInfo infoB)
            : this()
        {
            InfoA = infoA;
            InfoB = infoB;
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
        #endregion properties
    }
}
