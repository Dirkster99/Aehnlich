namespace Aehnlich.ViewModels.Documents
{
    using Aehnlich.Interfaces;
    using System.Windows.Input;

    /// <summary>
    /// Implements a document viewmodel for documents that display (text) file diff information.
    /// </summary>
    internal class DocDiffDocViewViewModel : DocumentBaseViewModel
    {
        #region fields
        private readonly IDocumentManagerViewModel _DocumentManager;
        private ICommand _CloseCommand;
        private ICommand _ViewLoadedCommand;
        private bool _ViewLoadedCommandCanExecute;
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
            this.ContentId = GetContentId(leftFilePath, rightFilePath);

            Title = GetTitle(leftFilePath, rightFilePath);
            ToolTip = GetTooltip(leftFilePath, rightFilePath);
            DocDiffDoc = AehnlichViewModelsLib.ViewModels.Factory.ConstructAppViewModel(leftFilePath, rightFilePath);
        }

        /// <summary>
        /// Hidden class constructor
        /// </summary>
        protected DocDiffDocViewViewModel()
        {
            _ViewLoadedCommandCanExecute = true;
        }

        #endregion ctors

        #region properties
        /// <summary>
        /// Exposes the application viewmodel that contains all elements for displaying
        /// the text document diff view.
        /// </summary>
        public AehnlichViewModelsLib.ViewModels.IAppViewModel DocDiffDoc { get; }

        /// <summary>
        /// Gets a command that should execute when the view is loaded vor the very  first time.
        /// This opportunity is used to do initial processing with progress bar display to ensure
        /// fluent ui with cancelable processings while the user might be waiting for results...
        /// </summary>
        public ICommand ViewLoadedCommand
        {
            get
            {
                if (_ViewLoadedCommand == null)
                {
                    _ViewLoadedCommand = new Base.RelayCommand<object>(
                    (p) =>
                    {
                        if (_ViewLoadedCommandCanExecute)
                        {
                            ExecuteCompare();

                            // This command should run only once since this implementation
                            // does not support virtualization (save/restore opertaions), yet
                            _ViewLoadedCommandCanExecute = false;
                        }
                    },
                    (p) =>
                    {
                        return _ViewLoadedCommandCanExecute;
                    });
                }

                return _ViewLoadedCommand;
            }
        }

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
        /// <summary>
        /// Invoke this method before application shut down
        /// to save all relevant user settings for recovery on appliaction re-start.
        /// </summary>
        public override void SaveSettings() { }

        /// <summary>
        /// Gets the content id for a (text) diff document based on the left and right paths
        /// of the diff document viewmodel.
        /// </summary>
        /// <param name="leftFilePath"></param>
        /// <param name="rightFilePath"></param>
        /// <returns></returns>
        internal static string GetContentId(string leftFilePath, string rightFilePath)
        {
            return string.Format("DocDiff{0}_{1}", leftFilePath, rightFilePath);
        }

        /// <summary>
        /// (Re-)Loads all data items necessary to display the diff information for
        /// left view A and right view B.
        /// </summary>
        private void ExecuteCompare()
        {
            object[] param = new object[2] { this.DocDiffDoc.FilePathA, this.DocDiffDoc.FilePathB };

            DocDiffDoc.CompareFilesCommand.Execute(param);
        }
        #endregion methods
    }
}
