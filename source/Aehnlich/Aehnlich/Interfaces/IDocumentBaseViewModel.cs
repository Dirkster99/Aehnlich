namespace Aehnlich.Interfaces
{
    using System;
    using System.ComponentModel;
    using HL.Interfaces;

    /// <summary>
    /// Defines a base interface that should be implemented
    /// by all document viewmodels in the application.
    /// </summary>
    public interface IDocumentBaseViewModel : INotifyPropertyChanged
    {
        #region properties
        /// <summary>
        /// Gets a unique if for a document or document type (eg: Start page).
        /// 
        /// This information is used to find already existing documents in a
        /// collection of documents or id document layouts for store/restore
        /// on app exit/restart.
        /// </summary>
        string ContentId { get; }

        /// <summary>
        /// Gets the title of a document (for display in document tab)
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Gets the Uri of the icon that should be shown for this document.
        /// </summary>
        Uri IconSource { get; }
        #endregion properties

        #region methods
        /// <summary>
        /// Invoke this method before application shut down
        /// to save all relevant user settings for recovery on appliaction re-start.
        /// </summary>
        void SaveSettings();
        #endregion methods
    }
}