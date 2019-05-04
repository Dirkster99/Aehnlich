namespace Aehnlich.Interfaces
{
    using AehnlichDirViewModelLib.Events;
    using System.Collections.Generic;

    internal interface IDocumentManagerViewModel : System.IDisposable
    {
        IEnumerable<IDocumentBaseViewModel> Documents { get; }

        bool CloseDocument(IDocumentBaseViewModel closeMe);

        void SaveSettings();

        /// <summary>
        /// Is raised when the user requests to view a file content diff (binary or text).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DocDiffDoc_CompareFilesRequest(object sender, OpenFileDiffEventArgs e);
    }
}