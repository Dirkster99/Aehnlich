namespace AehnlichDirViewModelLib.ViewModels
{
    using AehnlichLib.Dir;

    public class DirEntryViewModel : Base.ViewModelBase
    {
        #region fields
        private readonly DirectoryDiffEntry _Model;
        private readonly string _CurrentPathA, _CurrentPathB;
        #endregion fields

        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        public DirEntryViewModel(DirectoryDiffEntry model,
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
        public string ItemName
        {
            get { return _Model.Name; }
        }

        public string ItemPathA
        {
            get
            {
                return GetFullPath(_CurrentPathA, _Model.Name);
            }
        }

        public string ItemPathB
        {
            get
            {
                return GetFullPath(_CurrentPathB, _Model.Name);
            }
        }

        public bool IsItemInA
        {
            get
            {
                return _Model.InA;
            }
        }

        public bool IsItemInB
        {
            get
            {
                return _Model.InB;
            }
        }

        public bool IsItemDifferent
        {
            get
            {
                return _Model.Different;
            }
        }

        public bool IsFile
        {
            get
            {
                return _Model.IsFile;
            }
        }

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
                // System.IO likes to throw on invalide paths ...
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