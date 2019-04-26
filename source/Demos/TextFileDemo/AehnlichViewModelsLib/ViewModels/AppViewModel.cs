namespace AehnlichViewModelsLib.ViewModels
{
    using AehnlichViewModelsLib.Enums;
    using AehnlichViewModelsLib.Models;
    using AehnlichViewModelsLib.ViewModels.Base;
    using AehnlichViewModelsLib.ViewModels.Suggest;
    using AehnlichViewLib.Enums;
    using AehnlichViewLib.Models;
    using System;
    using System.Windows.Input;
    using AehnlichViewModelsLib.Interfaces;

    internal class AppViewModel : Base.ViewModelBase, IAppViewModel
    {
        #region fields
        private ICommand _CompareFilesCommand;
        private ICommand _ViewPortChangedCommand;
        private ICommand _OpenFileFromActiveViewCommand;
        private ICommand _CopyTextSelectionFromActiveViewCommand;
        private ICommand _FindTextCommand;
        private ICommand _GotoLineCommand;

        private double _OverViewValue = 0;

        private int _NumberOfTextLinesInViewPort = 0;
        private bool _IgnoreNextSliderValueChange = false;
        private int _LastLineToSync = 0;
        private readonly DiffDocViewModel _DiffCtrl;
        private readonly GotoLineControllerViewModel _GotoLineController;
        private readonly object _lockObject = new object();

        private InlineDialogMode _InlineDialog;
        private bool _disposed;

        private DiffViewPort _LastViewPort;
        private Focus _FocusControl;
        private readonly SuggestSourceViewModel _FilePathA, _FilePathB;
        #endregion fields

        #region ctors
        /// <summary>
        /// Parameterized class constructor
        /// </summary>
        public AppViewModel(string fileA, string fileB)
            : this()
        {
            _FilePathA.FilePath = fileA;
            _FilePathB.FilePath = fileB;
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        public AppViewModel()
        {
            _FilePathA = new SuggestSourceViewModel();
            _FilePathB = new SuggestSourceViewModel();

            _InlineDialog = InlineDialogMode.None;
            _DiffCtrl = new DiffDocViewModel();
            _GotoLineController = new GotoLineControllerViewModel(DiffCtrl.GotoTextLine, ToogleInlineDialog);
            _FocusControl = Focus.LeftFilePath;
        }
        #endregion ctors

        #region properties
        /// <summary>
        /// Gets a command that refreshs (reloads) the comparison of 2 textfiles.
        /// </summary>
        public ICommand CompareFilesCommand
        {
            get
            {
                if (_CompareFilesCommand == null)
                {
                    _CompareFilesCommand = new RelayCommand<object>((p) =>
                    {
                        ISuggestSourceViewModel fileA;
                        ISuggestSourceViewModel fileB;

                        if ((p is object[]) == false)
                        {
                            fileA = this.FilePathA;
                            fileB = this.FilePathB;
                        }
                        else
                        {
                            var param = p as object[];

                            if (param == null)
                                return;

                            if (param.Length != 2)
                                return;

                            fileA = param[0] as SuggestSourceViewModel;
                            fileB = param[1] as SuggestSourceViewModel;
                        }

                        if (fileA == null || fileB == null)
                            return;

                        if (fileA.IsTextValid == false || fileB.IsTextValid == false)
                            return;

                        if (string.IsNullOrEmpty(fileA.FilePath) || string.IsNullOrEmpty(fileB.FilePath))
                            return;

                        CompareFilesCommand_Executed(fileA.FilePath, fileB.FilePath);
                    });
                }

                return _CompareFilesCommand;
            }
        }

        /// <summary>
        /// Gets a focus element indicator to indicate a ui element to focus
        /// (this is used to focus the lift diff view by default when loading new files)
        /// </summary>
        public Focus FocusControl
        {
            get { return _FocusControl; }
            protected set
            {
                if (_FocusControl != value)
                {
                    _FocusControl = value;
                    NotifyPropertyChanged(() => FocusControl);
                }
            }
        }

        /// <summary>
        /// Gets a command that should be invoked:
        /// - when either of the synchronized left or right text diff view changes its display line or 
        /// - when the size (width and/or height) of the text diff view changes
        /// 
        /// to sync with the Overview control and update parts of the application.
        /// </summary>
        public ICommand ViewPortChangedCommand
        {
            get
            {
                if (_ViewPortChangedCommand == null)
                {
                    _ViewPortChangedCommand = new RelayCommand<object>((p) =>
                    {
                        var param = p as DiffViewPort;
                        if (param == null)
                            return;

                        lock (_lockObject)
                        {
                            if (param.FirstLine == _LastLineToSync)
                                return;

                            NumberOfTextLinesInViewPort = (param.LastLine - param.FirstLine) - 1;

                            // Get value of first visible line and set it in Overview slider
                            double overViewValue = (uint)param.FirstLine - 1;

                            // Weight the visible first line with the viewport height towards
                            // the beginning and end of the document
                            // (make sure the Overview scrolls all the way down when scrolling to end of document)
                            if (DiffCtrl != null)
                            {
                                if (((double)DiffCtrl.MaxNumberOfLines - NumberOfTextLinesInViewPort) > 0)
                                {
                                    double valueFactor = overViewValue / ((double)DiffCtrl.MaxNumberOfLines - NumberOfTextLinesInViewPort);
                                    overViewValue += (valueFactor * (double)NumberOfTextLinesInViewPort);
                                }
                            }

                            // This change was caused by left/right diff view
                            // So, we do not need to sync it when the Overview says:
                            // 'Hey, I've changed my value' to break recursive loops(!)
                            if ((uint)(Math.Abs(_OverViewValue - overViewValue)) >= 1)
                            {
                                _IgnoreNextSliderValueChange = true;
                                OverViewValue = (uint)overViewValue;
                            }
                        }

                        _LastViewPort = param;
                    }
                    , (p) =>
                    {
                        return true;
                    });
                }

                return _ViewPortChangedCommand;
            }
        }

        /// <summary>
        /// Gets a command that opens the currently active file in Windows.
        /// </summary>
        public ICommand OpenFileFromActiveViewCommand
        {
            get
            {
                if (_OpenFileFromActiveViewCommand == null)
                {
                    _OpenFileFromActiveViewCommand = new RelayCommand<object>((p) =>
                    {
                        IDiffSideViewModel nonActView;
                        IDiffSideViewModel activeView = DiffCtrl.GetActiveView(out nonActView);

                        if (activeView != null)
                            FileSystemCommands.OpenInWindows(activeView.FileName);
                        else
                        {
                            if (nonActView != null)
                                FileSystemCommands.OpenInWindows(nonActView.FileName);
                        }
                    }, (p) =>
                     {
                         IDiffSideViewModel nonActView;
                         IDiffSideViewModel activeView = DiffCtrl.GetActiveView(out nonActView);

                         if (activeView != null)
                         {
                             if (string.IsNullOrEmpty(activeView.FileName) == false)
                                 return true;
                         }

                         if (nonActView != null)
                         {
                             if (string.IsNullOrEmpty(nonActView.FileName) == false)
                                 return true;
                         }

                         return false;
                     });
                }

                return _OpenFileFromActiveViewCommand;
            }
        }

        /// <summary>
        /// Gets a command that copies the currently selected text into the Windows Clipboard.
        /// </summary>
        public ICommand CopyTextSelectionFromActiveViewCommand
        {
            get
            {
                if (_CopyTextSelectionFromActiveViewCommand == null)
                {
                    _CopyTextSelectionFromActiveViewCommand = new RelayCommand<object>((p) =>
                    {
                        IDiffSideViewModel nonActView;
                        IDiffSideViewModel activeView = DiffCtrl.GetActiveView(out nonActView);

                        string textSelection = activeView.TxtControl.GetSelectedText();
                        FileSystemCommands.CopyString(textSelection);
                    }, (p) =>
                    {
                        IDiffSideViewModel nonActView;
                        IDiffSideViewModel activeView = DiffCtrl.GetActiveView(out nonActView);

                        if (activeView != null)
                            return (string.IsNullOrEmpty(activeView.TxtControl.GetSelectedText()) == false);

                        return false;
                    });
                }

                return _CopyTextSelectionFromActiveViewCommand;
            }
        }

        /// <summary>
        /// Gets the number of text lines currently visible in the text view
        /// of the left view A and the right view B.
        /// </summary>
        public int NumberOfTextLinesInViewPort
        {
            get { return _NumberOfTextLinesInViewPort; }
            private set
            {
                if (_NumberOfTextLinesInViewPort != value)
                {
                    _NumberOfTextLinesInViewPort = value;
                    NotifyPropertyChanged(() => NumberOfTextLinesInViewPort);
                }
            }
        }

        /// <summary>
        /// Gets/sets the current overview value shown in a seperate overview diff control.
        /// This control is similar to a scrollbar - it can be used to scroll to a certain
        /// postion - but its thumb and color background also indicates the current cursor
        /// line within a birds eye view.
        /// </summary>
        public double OverViewValue
        {
            get { return _OverViewValue; }
            set
            {
                if ((int)(Math.Abs(_OverViewValue - value)) >= 1)
                {
                    _OverViewValue = value;
                    NotifyPropertyChanged(() => OverViewValue);

                    if (OverviewValueChangedCanExecute())
                        OverviewValueChanged(_OverViewValue);
                }
            }
        }

        /// <summary>
        /// Gets the document viewmodel that manages left and right viewmodel
        /// which drive the synchronized diff text view.
        /// </summary>
        public IDiffDocViewModel DiffCtrl
        {
            get { return _DiffCtrl; }
        }

        /// <summary>
        /// Gets the path of file A in the comparison.
        /// </summary>
        public ISuggestSourceViewModel FilePathA
        {
            get
            {
                return _FilePathA;
            }
        }

        /// <summary>
        /// Gets the path of file B in the comparison.
        /// </summary>
        public ISuggestSourceViewModel FilePathB
        {
            get
            {
                return _FilePathB;
            }
        }

        /// <summary>
        /// Implements a find command via AvalonEdits build in search panel which can be
        /// activated if the right or left control has focus.
        /// </summary>
        public ICommand FindTextCommand
        {
            get
            {
                if (_FindTextCommand == null)
                {
                    _FindTextCommand = new RelayCommand<object>((p) =>
                    {
                        ApplicationCommands.Find.Execute(null, null);
                    },
                    (p) =>
                    {
                        return ApplicationCommands.Find.CanExecute(null, null);
                    });
                }

                return _FindTextCommand;
            }
        }

        /// <summary>
        /// Gets a command to scroll the view to a certain line within the available lines.
        /// </summary>
        public ICommand GotoLineCommand
        {
            get
            {
                if (_GotoLineCommand == null)
                {
                    _GotoLineCommand = new RelayCommand<object>((p) =>
                    {
                        ToogleInlineDialog(InlineDialogMode.Goto);
                    }, (p) =>
                     {
                         return DiffCtrl.IsDiffDataAvailable;
                     });
                }

                return _GotoLineCommand;
            }
        }

        /// <summary>
        /// Gets/sets the current inline dialog mode.
        /// </summary>
        public InlineDialogMode InlineDialog
        {
            get { return _InlineDialog; }
            set
            {
                if (_InlineDialog != value)
                {
                    _InlineDialog = value;
                    NotifyPropertyChanged(() => InlineDialog);
                }
            }
        }

        /// <summary>
        /// Gets view model that drives the Goto Line inline dialog view.
        /// </summary>
        public IGotoLineControllerViewModel GotoLineController
        {
            get
            {
                return _GotoLineController;
            }
        }
        #endregion properties

        #region methods
        private void CompareFilesCommand_Executed(string fileA, string fileB)
        {
            try
            {
                if (System.IO.File.Exists(fileA) == false ||
                    System.IO.File.Exists(fileB) == false)
                    return;

                _DiffCtrl.ShowDifferences(new ShowDiffArgs(fileA, fileB, DiffType.File));

                FocusControl = Focus.None;
                FocusControl = Focus.LeftView;
                GotoLineController.MaxLineValue = _DiffCtrl.NumberOfLines;

                // Position view on first difference if thats available
                if (_DiffCtrl.GoToFirstDifferenceCommand.CanExecute(null))
                    _DiffCtrl.GoToFirstDifferenceCommand.Execute(null);

                NotifyPropertyChanged(() => DiffCtrl);
            }
            catch
            {
                // Catch any System.IO exception to make sure application keeps running...
            }
        }

        private InlineDialogMode ToogleInlineDialog(InlineDialogMode forThisDialog)
        {
            if (InlineDialog != forThisDialog)
                InlineDialog = forThisDialog;
            else
                InlineDialog = InlineDialogMode.None;

            return InlineDialog;
        }

        #region IDisposable
        /// <summary>
        /// Standard dispose method of the <seealso cref="IDisposable" /> interface.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Source: http://www.codeproject.com/Articles/15360/Implementing-IDisposable-and-the-Dispose-Pattern-P
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed == false)
            {
                if (disposing == true)
                {
                    // Dispose of the currently used inner disposables
                    _DiffCtrl.Dispose();
                    _FilePathA.Dispose();
                    _FilePathB.Dispose();
                }

                // There are no unmanaged resources to release, but
                // if we add them, they need to be released here.
            }

            _disposed = true;

            //// If it is available, make the call to the
            //// base class's Dispose(Boolean) method
            ////base.Dispose(disposing);
        }
        #endregion IDisposable

        #region OverviewValueChanged
        private void OverviewValueChanged(object p)
        {
            lock (_lockObject)
            {
                if ((p is double) == false)
                    return;

                double param = (double)p;

                if (_IgnoreNextSliderValueChange == true)
                {
                    if (_LastLineToSync == (int)param)
                        return;

                    _IgnoreNextSliderValueChange = false;
                    return;
                }

                _LastLineToSync = (int)param;

                IDiffSideViewModel nonActView;
                IDiffSideViewModel activeView = DiffCtrl.GetActiveView(out nonActView);
                var gotoPos = new DiffViewPosition((int)param, 0);
                DiffCtrl.ScrollToLine(gotoPos, nonActView, activeView, false);
            }
        }

        private bool OverviewValueChangedCanExecute()
        {
            IDiffSideViewModel nonActView;
            IDiffSideViewModel activeView = DiffCtrl.GetActiveView(out nonActView);
            if (activeView == null)
                return false;

            return true;
        }
        #endregion OverviewValueChanged
        #endregion methods
    }
}
