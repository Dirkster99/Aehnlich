namespace AehnlichDirViewModelLib.Models
{
    using AehnlichLib.Dir;

    public class ShowDirDiffArgs
    {
        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="leftDir"></param>
        /// <param name="rightDir"></param>
        public ShowDirDiffArgs(string leftDir, string rightDir)
            : this()
        {
            this.LeftDir = leftDir;
            this.RightDir = rightDir;
        }

        /// <summary>
        /// Hidden class constructor
        /// </summary>
        protected ShowDirDiffArgs()
        {
            ShowOnlyInA = true;
            ShowOnlyInB = true;
            ShowDifferent = true;
            ShowSame = true;
            Recursive = true;
            IgnoreDirectoryComparison = true;
        }
        #endregion ctors

        #region properties
        public string LeftDir { get; }

        public string RightDir { get; }

        public bool ShowOnlyInA { get; }

        public bool ShowOnlyInB { get; }

        public bool ShowDifferent { get; }

        public bool ShowSame { get; }

        public bool Recursive { get; }

        public bool IgnoreDirectoryComparison { get; }

        public DirectoryDiffFileFilter FileFilter { get; set; }
        #endregion properties
    }
}
