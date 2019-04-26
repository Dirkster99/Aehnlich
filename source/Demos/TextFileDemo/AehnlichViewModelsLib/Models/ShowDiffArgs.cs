namespace AehnlichViewModelsLib.Models
{
    using AehnlichLib.Enums;
    using AehnlichViewModelsLib.Enums;

    internal class ShowDiffArgs
    {
        #region Constructors
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="itemA"></param>
        /// <param name="itemB"></param>
        /// <param name="diffType"></param>
        /// <param name="spacesPerTab"></param>
        public ShowDiffArgs(string itemA, string itemB, DiffType diffType, int spacesPerTab)
            : this()
        {
            this.A = itemA;
            this.B = itemB;
            this.DiffType = diffType;
            this.SpacesPerTab = spacesPerTab;
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="itemA"></param>
        /// <param name="itemB"></param>
        /// <param name="diffType"></param>
        public ShowDiffArgs(string itemA, string itemB, DiffType diffType)
            :this()
        {
            this.A = itemA;
            this.B = itemB;
            this.DiffType = diffType;
        }

        /// <summary>
        /// Hidden standard constructor
        /// </summary>
        protected ShowDiffArgs()
        {
            SpacesPerTab = 4;
            LeadingCharactersToIgnore = 0;
            IgnoreCase = true;
            IgnoreTextWhitespace = true;
            ShowChangeAsDeleteInsert = false;

            IgnoreXmlWhitespace = true;

            HashType = HashType.HashCode;
            CompareType = CompareType.Auto;

            BinaryFootprintLength = 8;
        }
        #endregion

        #region Public Properties

        public string A { get; }

        public string B { get; }

        public DiffType DiffType { get; }

        public int SpacesPerTab { get; }

        public int LeadingCharactersToIgnore { get; }

        public bool IgnoreCase { get; }

        public bool IgnoreTextWhitespace { get; }

        public bool ShowChangeAsDeleteInsert { get; }

        public bool IgnoreXmlWhitespace { get; }

        public HashType HashType { get; }

        public CompareType CompareType { get; set; }

        public int BinaryFootprintLength { get; }
        #endregion
    }
}
