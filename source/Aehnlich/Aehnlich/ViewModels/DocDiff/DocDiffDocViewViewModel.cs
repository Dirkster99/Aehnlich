namespace Aehnlich.ViewModels.Documents
{
    using Aehnlich.Interfaces;
    using System.Windows.Input;

    internal class DocDiffDocViewViewModel : DocumentBaseViewModel
    {
        #region fields
        private readonly IDocumentManagerViewModel _DocumentManager;
        private ICommand _CloseCommand;
        #endregion fields

        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="docManager"></param>
        /// <param name="leftDirPath"></param>
        /// <param name="rightDirPath"></param>
        public DocDiffDocViewViewModel(IDocumentManagerViewModel docManager,
                                       string leftFilePath,
                                       string rightFilePath)
            : this()
        {
            _DocumentManager = docManager;
            this.ContentId = string.Format("DocDiff{0}_{1}", leftFilePath, rightFilePath);

            Title = "Compare Files";
            DocDiffDoc = AehnlichViewModelsLib.ViewModels.Factory.ConstructAppViewModel(leftFilePath, rightFilePath);
        }

        /// <summary>
        /// Hidden class constructor
        /// </summary>
        protected DocDiffDocViewViewModel()
        {
        }

        #endregion ctors

        #region properties
        public AehnlichViewModelsLib.ViewModels.IAppViewModel DocDiffDoc { get; }

        /// <summary>
        /// Gets a command that is invoked when the user clicks the document tab close button.
        /// </summary>
        public override ICommand CloseCommand
        {
            get
            {
                if (_CloseCommand == null)
                {
                    _CloseCommand = new Base.RelayCommand<object>(
                    (p) =>
                    {
                        SaveSettings();
                        _DocumentManager.CloseDocument(this);
                    },
                    (p) =>
                    {
                        if (DocDiffDoc != null)
                        {
                            if (DocDiffDoc.DiffProgress.IsProgressbarVisible == true)
                                return false;
                        }

                        return true;
                    });
                }

                return _CloseCommand;
            }
        }

        /// <summary>
        /// Gets a command that is invoked when a document is saved.
        /// 
        /// Returning null here should disable controls that bind to this command,
        /// which is just what we need for documents that cannot save any data (eg.: Start Page).
        /// </summary>
        public override ICommand SaveCommand { get { return null; } }
        #endregion properties

        #region methods
        public void ExecuteCompare()
        {
            object[] param = new object[2] { this.DocDiffDoc.FilePathA, this.DocDiffDoc.FilePathB };

            DocDiffDoc.CompareFilesCommand.Execute(param);
        }

        /// <summary>
        /// Invoke this method before application shut down
        /// to save all relevant user settings for recovery on appliaction re-start.
        /// </summary>
        public override void SaveSettings() { }
        #endregion methods
    }
}
