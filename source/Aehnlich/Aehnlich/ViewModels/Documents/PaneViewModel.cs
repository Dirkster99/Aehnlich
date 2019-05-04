namespace Aehnlich.ViewModels.Documents
{
    using System;

    internal abstract class PaneViewModel : Base.ViewModelBase
    {
        #region fields
        private string _Title;
        private string _ToolTip;
        private string _contentId;
        private bool _IsSelected;
        private bool _IsActive;
        #endregion fields

        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        protected PaneViewModel()
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
            get { return _Title; }
            protected set
            {
                if (_Title != value)
                {
                    _Title = value;
                    NotifyPropertyChanged(() => Title);
                }
            }
        }

        /// <summary>
        /// Gets the ToolTip of a document (for display on mouseover on document tab)
        /// </summary>
        public string ToolTip
        {
            get { return _ToolTip; }
            protected set
            {
                if (_ToolTip != value)
                {
                    _ToolTip = value;
                    NotifyPropertyChanged(() => ToolTip);
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
            get { return _IsSelected; }
            set
            {
                if (_IsSelected != value)
                {
                    _IsSelected = value;
                    NotifyPropertyChanged(() => IsSelected);
                }
            }
        }

        public bool IsActive
        {
            get { return _IsActive; }
            set
            {
                if (_IsActive != value)
                {
                    _IsActive = value;
                    NotifyPropertyChanged(() => IsActive);
                }
            }
        }
        #endregion properties

        #region methods

        #endregion methods
    }
}
