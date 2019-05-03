namespace Aehnlich.ViewModels.Documents
{
    using AehnlichDirViewModelLib.Events;
    using AehnlichDirViewModelLib.Interfaces;
    using System;
    using System.Windows;

    internal class DirDiffDocViewViewModel : Base.ViewModelBase
    {
        #region fields
        #endregion fields

        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        public DirDiffDocViewViewModel(EventHandler<OpenFileDiffEventArgs> openFileContentDiffRequestHandler)
            : this()
        {
            DirDiffDoc = AehnlichDirViewModelLib.ViewModels.Factory.ConstructAppViewModel();

            //DirDiffDoc.DirDiffDoc.CompareFilesRequest += openFileContentDiffRequestHandler;
            WeakEventManager<IDirDiffDocViewModel, OpenFileDiffEventArgs>.AddHandler(
                DirDiffDoc.DirDiffDoc, "CompareFilesRequest", openFileContentDiffRequestHandler);
        }

        /// <summary>
        /// Hidden class constructor
        /// </summary>
        protected DirDiffDocViewViewModel()
        {
        }
        #endregion ctors

        #region properties
        public AehnlichDirViewModelLib.Interfaces.IAppViewModel DirDiffDoc { get; }
        #endregion properties
        
        #region methods
        public void Initilize(string leftDirPath, string rightDirPath)
        {
            DirDiffDoc.Initialize(leftDirPath, rightDirPath);
        }
        #endregion methods
    }
}
