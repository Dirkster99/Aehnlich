namespace Aehnlich.ViewModels.Documents
{
    using AehnlichDirViewModelLib.Events;
    using AehnlichDirViewModelLib.Interfaces;
    using System;
    using System.Windows;
    using System.Windows.Input;

    internal class DirDiffDocViewViewModel : Base.ViewModelBase
    {
        private ICommand _ViewLoadedCommand;
        private bool _ViewLoadedCommandCanExecute;

        private readonly string _initLeftDirPath, _initRightDirPath;
        #region fields
        #endregion fields

        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        public DirDiffDocViewViewModel(EventHandler<OpenFileDiffEventArgs> openFileContentDiffRequestHandler
                                       , string initLeftDirPath
                                        , string initRightDirPath)
            : this()
        {
            _initLeftDirPath = initLeftDirPath;
            _initRightDirPath = initRightDirPath;

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
            _ViewLoadedCommandCanExecute = true;
        }
        #endregion ctors

        #region properties
        public AehnlichDirViewModelLib.Interfaces.IAppViewModel DirDiffDoc { get; }

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
        #endregion properties

        #region methods
        /// <summary>
        /// (Re-)Loads all data items necessary to display the diff information for
        /// left view A and right view B.
        /// </summary>
        private void ExecuteCompare()
        {
            // Initialize directory diff and execute async task based comparison
            DirDiffDoc.Initialize(_initLeftDirPath, _initRightDirPath);

            DirDiffDoc.CompareDirectoriesCommand.Execute(
                new object[2] { _initLeftDirPath, _initRightDirPath });
        }
        #endregion methods
    }
}
