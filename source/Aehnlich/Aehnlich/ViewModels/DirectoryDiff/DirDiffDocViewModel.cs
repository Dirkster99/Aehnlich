﻿namespace Aehnlich.ViewModels.Documents
{
    using Aehnlich.Interfaces;
    using AehnlichDirViewModelLib.Events;
    using System;
    using System.Windows;
    using System.Windows.Input;

    internal enum DirDiffDocStages
    {
        /// <summary>
        /// Give user a chance to supply parameters and edit options.
        /// </summary>
        DirDiffSetup,

        /// <summary>
        /// Review results and work with the actual directory compare data.
        /// </summary>
        DirDiffView
    }

    internal class DirDiffDocViewModel : DocumentBaseViewModel
    {
        #region fields
        public const string SetupContentID = "_Compare Directories_";

        private ICommand _CloseCommand;
        private DirDiffDocStages _CurrentStage;
        private object _SelectedDirDiffItem;

        private readonly IDocumentManagerViewModel _DocumentManager;
        #endregion fields

        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="docManager"></param>
        /// <param name="leftDirPath"></param>
        /// <param name="rightDirPath"></param>
        public DirDiffDocViewModel(IDocumentManagerViewModel docManager,
                                   string leftDirPath,
                                   string rightDirPath)
            : this()
        {
            _DocumentManager = docManager;

            Title = "Compare Directories";
            ContentId = SetupContentID;

            _CurrentStage = DirDiffDocStages.DirDiffSetup;

            var createViewPageCommand = new Base.RelayCommand<object>(
                (p) => CreatePageViewCommand_Executed(p),
                (p) => CreatePageViewCommand_CanExecute(p));

            _SelectedDirDiffItem = new DirDiffDocSetupViewModel(createViewPageCommand, leftDirPath, rightDirPath);
        }

        /// <summary>
        /// Hidden standard clas constructor
        /// </summary>
        protected DirDiffDocViewModel()
            : base()
        {
        }
        #endregion ctors

        #region properties
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
                        var asyncProc = SelectedDirDiffItem as DirDiffDocViewViewModel;
                        if (asyncProc != null)
                        {
                            if (asyncProc.DirDiffDoc.DiffProgress.IsProgressbarVisible == true)
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

        public DirDiffDocStages CurrentStage
        {
            get { return _CurrentStage; }
            private set
            {
                if (_CurrentStage != value)
                {
                    _CurrentStage = value;
                    NotifyPropertyChanged(() => CurrentStage);
                }
            }
        }

        public object SelectedDirDiffItem
        {
            get { return _SelectedDirDiffItem; }
            private set
            {
                if (_SelectedDirDiffItem != value)
                {
                    _SelectedDirDiffItem = value;
                    NotifyPropertyChanged(() => SelectedDirDiffItem);
                }
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Invoke this method before application shut down
        /// to save all relevant user settings for recovery on appliaction re-start.
        /// </summary>
        public override void SaveSettings()
        {
            var compare = this.SelectedDirDiffItem as DirDiffDocViewViewModel;
            if (compare != null)
            {
                Properties.Settings.Default.LeftDirPath = compare.DirDiffDoc.RightDirPath;
                Properties.Settings.Default.RightDirPath = compare.DirDiffDoc.LeftDirPath;
            }
        }

        private bool CreatePageViewCommand_CanExecute(object parameter)
        {
            var DirDiff = SelectedDirDiffItem as DirDiffDocSetupViewModel;
            if (DirDiff == null)
                return false;

            try
            {
                if (System.IO.Directory.Exists(DirDiff.LeftDirectoryPath) == false)
                    return false;

                if (System.IO.Directory.Exists(DirDiff.RightDirectoryPath) == false)
                    return false;
            }
            catch
            {
                return false;
            }

            return true;
        }

        private void CreatePageViewCommand_Executed(object parameter)
        {
            switch (this.CurrentStage)
            {
                case DirDiffDocStages.DirDiffSetup:
                    var setupPage = SelectedDirDiffItem as DirDiffDocSetupViewModel;

                    if (setupPage != null)
                    {
                        ContentId = SetupContentID + Guid.NewGuid().ToString();

                        // Subscripe document manager to diff file open event
                        var newPage = new DirDiffDocViewViewModel(_DocumentManager.DirDiffDoc_CompareFilesrequest);

                        // Initialize directory diff and execute async task based comparison
                        newPage.Initilize(setupPage.LeftDirectoryPath, setupPage.RightDirectoryPath);

                        newPage.DirDiffDoc.CompareDirectoriesCommand.Execute(
                            new object[2] { setupPage.LeftDirectoryPath, setupPage.RightDirectoryPath });

                        SelectedDirDiffItem = newPage;
                        CurrentStage = DirDiffDocStages.DirDiffView;
                    }
                    break;

                case DirDiffDocStages.DirDiffView:
                default:
                    break;
            }

        }
        #endregion methods
    }
}
