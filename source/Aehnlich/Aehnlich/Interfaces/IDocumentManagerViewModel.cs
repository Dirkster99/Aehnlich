namespace Aehnlich.Interfaces
{
    using System.Collections.Generic;

    internal interface IDocumentManagerViewModel : System.IDisposable
    {
        IEnumerable<IDocumentBaseViewModel> Documents { get; }

        bool CloseDocument(IDocumentBaseViewModel closeMe);

        void SaveSettings();
    }
}