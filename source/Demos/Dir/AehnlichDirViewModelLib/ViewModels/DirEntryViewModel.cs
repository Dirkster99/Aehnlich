namespace AehnlichDirViewModelLib.ViewModels
{
    using AehnlichLib.Dir;
    using AehnlichLib.Interfaces;
    using System;

    public class DirEntryViewModel : Base.ViewModelBase
    {
        #region fields
        private readonly IDirectoryDiffEntry _Model;
        private readonly string _CurrentPathA, _CurrentPathB;
        #endregion fields

        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        public DirEntryViewModel(IDirectoryDiffEntry model,
                                 string currentPathA,
                                 string currentPathB)
            : this()
        {
            _Model = model;
            _CurrentPathA = currentPathA;
            _CurrentPathB = currentPathB;
        }

        /// <summary>
        /// Hidden class constructor
        /// </summary>
        protected DirEntryViewModel()
        {
        }
        #endregion ctors

        #region properties
        /// <summary>
        /// Gets the name of item A and item B (file or directory).
        ///	This name is only applicable if both of these items actually exist.
		/// Otherwise, the name may only be applicable to item A or item B
		/// (<see cref="IsItemInA"/> and <see cref="IsItemInB"/>).
        /// </summary>
        public string ItemName
        {
            get { return _Model.Name; }
        }

        /// <summary>
        /// Gets the full path of item A (file or directory A) if it exists.
		/// <see cref="IsItemInA"/>
        /// </summary>
        public string ItemPathA
        {
            get
            {
                return GetFullPath(_CurrentPathA, _Model.Name);
            }
        }

        /// <summary>
        /// Gets the full path of item B (file or directory A) if it exists.
		/// <see cref="IsItemInB"/>
        /// </summary>
        public string ItemPathB
        {
            get
            {
                return GetFullPath(_CurrentPathB, _Model.Name);
            }
        }

        /// <summary>
        /// Gets whether the item A (file or directory A) actually exists or not.
		/// Item B (file or directory B) may exist in the case that A does not exist
		/// and this entry may then be here to represent that difference.
        /// </summary>
        public bool IsItemInA
        {
            get
            {
                return _Model.InA;
            }
        }

        /// <summary>
        /// Gets whether the item B (file or directory B) actually exists or not.
		/// Item A (file or directory A) may exist in the case that B does not exist
		/// and this entry may then be here to represent that difference.
        /// </summary>
        public bool IsItemInB
        {
            get
            {
                return _Model.InB;
            }
        }

        /// <summary>
        /// Gets whether the item A (file or directory A) and item B (file or directory B)
		/// in this entry are equal (false) or not (true). Inequality (true) indicates that
		/// only one of the given items actually exists or their content is simply different.
        /// </summary>
        public bool IsItemDifferent
        {
            get
            {
                return _Model.Different;
            }
        }

        /// <summary>
        /// Gets whether this entry represent a file (true), or not (directory or drive).
        /// </summary>
        public bool IsFile
        {
            get
            {
                return _Model.IsFile;
            }
        }

        /// <summary>
        /// Gets the size of an item A (file or directory) in bytes.
        /// </summary>
        public double ItemLengthA
        {
            get
            {
                return _Model.LengthA;
            }
        }

        /// <summary>
        /// Gets the size of an item B (file or directory) in bytes.
        /// </summary>
        public double ItemLengthB
        {
            get
            {
                return _Model.LengthB;
            }
        }

        /// <summary>
        /// Gets the last date and time at which an item A (file or directory)
		/// was changed.
        /// </summary>
        public DateTime ItemLastUpdateA
        {
            get
            {
                return _Model.LastUpdateA;
            }
        }

        /// <summary>
        /// Gets the last date and time at which an item B (file or directory)
		/// was changed.
        /// </summary>
        public DateTime ItemLastUpdateB
        {
            get
            {
                return _Model.LastUpdateB;
            }
        }

        /// <summary>
        /// Gets a list of sub-directories and files that are stored underneath this entry.
        /// </summary>
        public DirectoryDiffEntryCollection Subentries
        {
            get
            {
                return _Model.Subentries;
            }
        }
        #endregion properties

        #region methods
        private string GetFullPath(string path, string name)
        {
            try
            {
                return System.IO.Path.Combine(path, name);
            }
            catch
            {
                // System.IO throws on invalid paths that are not easy to exclude...
            }

            try
            {
                return string.Format(@"{0}\{1}", path, name);
            }
            catch
            {
                // Absolute last resort - we should never be getting here ...
                return name;
            }
        }
        #endregion methods
    }
}