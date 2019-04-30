namespace Aehnlich.ViewModels.Documents
{
    using System;

    internal abstract class PaneViewModel : Base.ViewModelBase
    {
        #region fields
        private string _title;
        private string _contentId;
        private bool _isSelected;
        private bool _isActive;
        #endregion fields

        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        public PaneViewModel()
        {

        }
        #endregion ctors

        #region properties
        /// <summary>
        /// Gets a unique if for a document or document type (eg: Start page).
        /// 
        /// This information is used to find already existing documents in a
        /// collection of documents or id document layouts for store/restore
        /// on app exit/restart.
        /// </summary>
        public string ContentId
        {
            get { return _contentId; }
            protected set
            {
                if (_contentId != value)
                {
                    _contentId = value;
                    NotifyPropertyChanged(() => ContentId);
                }
            }
        }

        /// <summary>
        /// Gets the title of a document (for display in document tab)
        /// </summary>
        public string Title
        {
            get { return _title; }
            protected set
            {
                if (_title != value)
                {
                    _title = value;
                    NotifyPropertyChanged(() => Title);
                }
            }
        }

        /// <summary>
        /// Gets the Uri of the icon that should be shown for this document.
        /// </summary>
        public virtual Uri IconSource
        {
            get;

            protected set;
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    NotifyPropertyChanged(() => IsSelected);
                }
            }
        }

        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    NotifyPropertyChanged(() => IsActive);
                }
            }
        }
        #endregion properties

        #region methods

        #endregion methods
    }
}
