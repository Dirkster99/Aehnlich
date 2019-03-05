namespace AehnlichLibViewModels.ViewModels
{
    using AehnlichLibViewModels.Enums;
    using AehnlichLibViewModels.Models;
    using AehnlichLibViewModels.ViewModels.Base;
    using AehnlichViewLib.Models;
    using System;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Threading;

    public class AppViewModel : Base.ViewModelBase
    {
        #region fields
        private string _FilePathA;
        private string _FilePathB;
        private ICommand _CompareFilesCommand;
        private ICommand _OpenFileFromActiveViewCommand;
        private ICommand _CopyTextSelectionFromActiveViewCommand;

        private double _OverViewValue = 0;
        private int _NumberOfTextLinesInViewPort = 0;
        private bool _IgnoreNextSliderValueChange = false;
        private bool _IgnoreNextTextSyncValueChange = true;
        private int _LastLineToSync = 0;
        private ICommand _ViewPortChangedCommand;
        private ICommand _OverviewValueChangedCommand;
        private DiffViewPort _LastViewPort;
        private ICommand _FindTextCommand;
        private readonly DiffDocViewModel _DiffCtrl;
        private readonly object _lockObject = new object();

        private InlineDialogMode _InlineDialog;
        #endregion fields

        #region ctors
        /// <summary>
        /// Parameterized class constructor
        /// </summary>
        public AppViewModel(string fileA, string fileB)
            : this()
        {
            _FilePathA = fileA;
            _FilePathB = fileB;
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        public AppViewModel()
        {
            _InlineDialog = InlineDialogMode.None;
            _DiffCtrl = new DiffDocViewModel();
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
                        var param = p as object[];

                        if (param == null)
                            return;

                        if (param.Length != 2)
                            return;

                        string fileA = param[0] as string;
                        string fileB = param[1] as string;

                        if (string.IsNullOrEmpty(fileA) || string.IsNullOrEmpty(fileB))
                            return;

                        _DiffCtrl.ShowDifferences(new Models.ShowDiffArgs(fileA, fileB, Enums.DiffType.File));

                        if (_LastViewPort != null)
                            _DiffCtrl.GetChangeEditScript(_LastViewPort.FirstLine - 1, _LastViewPort.LastLine - 1, 4);

                        // Position view on first difference if thats available
                        if (_DiffCtrl.GoToFirstDifferenceCommand.CanExecute(null))
                            _DiffCtrl.GoToFirstDifferenceCommand.Execute(null);

                        NotifyPropertyChanged(() => DiffCtrl);
                    });
                }

                return _CompareFilesCommand;
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
                        DiffSideViewModel nonActView;
                        DiffSideViewModel activeView = DiffCtrl.GetActiveView(out nonActView);

                        if (activeView != null)
                            FileSystemCommands.OpenInWindows(activeView.FileName);
                        else
                        {
                            if (nonActView != null)
                                FileSystemCommands.OpenInWindows(nonActView.FileName);
                        }
                    },(p) =>
                    {
                        DiffSideViewModel nonActView;
                        DiffSideViewModel activeView = DiffCtrl.GetActiveView(out nonActView);

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
                        DiffSideViewModel nonActView;
                        DiffSideViewModel activeView = DiffCtrl.GetActiveView(out nonActView);

                        string textSelection = activeView.TxtControl.GetSelectedText();
                        FileSystemCommands.CopyString(textSelection);
                    }, (p) =>
                    {
                        DiffSideViewModel nonActView;
                        DiffSideViewModel activeView = DiffCtrl.GetActiveView(out nonActView);

                        if (activeView != null)
                            return (string.IsNullOrEmpty(activeView.TxtControl.GetSelectedText()) == false);

                        return false;
                    });
                }

                return _CopyTextSelectionFromActiveViewCommand;
            }
        }

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

        public double OverViewValue
        {
            get { return _OverViewValue; }
            private set
            {
                if (_OverViewValue != value)
                {
                    _OverViewValue = value;
                    NotifyPropertyChanged(() => OverViewValue);
                }
            }
        }

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
                            if (_IgnoreNextTextSyncValueChange == true)
                            {
                                if (param.FirstLine == _LastLineToSync)
                                    return;

                                _IgnoreNextTextSyncValueChange = false;
                                return;
                            }

                            _IgnoreNextSliderValueChange = true;

                            NumberOfTextLinesInViewPort = (param.LastLine - param.FirstLine) - 1;

                            // Get value of first visible line and set it in Overview slider
                            OverViewValue = param.FirstLine;
                        }

                        int spacesPerTab = 4;

                        // Translate from 1-based values to zero-based values
                        int count = _DiffCtrl.GetChangeEditScript(param.FirstLine-1, param.LastLine-1, spacesPerTab);

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

        public ICommand OverviewValueChangedCommand
        {
            get
            {
                if (_OverviewValueChangedCommand == null)
                {
                    _OverviewValueChangedCommand = new RelayCommand<object>((p) =>
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
                            _IgnoreNextTextSyncValueChange = true;

                            DiffSideViewModel nonActView;
                            DiffSideViewModel activeView = DiffCtrl.GetActiveView(out nonActView);
                            DiffViewPosition gotoPos = new DiffViewPosition((int)param, 0);
                            DiffCtrl.ScrollToLine(gotoPos, nonActView, activeView);
                        }
                    },
                    (p) =>
                    {
                        DiffSideViewModel nonActView;
                        DiffSideViewModel activeView = DiffCtrl.GetActiveView(out nonActView);
                        if (activeView == null)
                            return false;

                        return true;
                    });
                }

                return _OverviewValueChangedCommand;
            }
        }

        public DiffDocViewModel DiffCtrl
        {
            get { return _DiffCtrl; }
        }

        /// <summary>
        /// Gets the path of file A in the comparison.
        /// </summary>
        public string FilePathA
        {
            get
            {
                return _FilePathA;
            }

            set
            {
                if (_FilePathA != value)
                {
                    _FilePathA = value;
                    NotifyPropertyChanged(() => FilePathA);
                }
            }
        }

        /// <summary>
        /// Gets the path of file B in the comparison.
        /// </summary>
        public string FilePathB
        {
            get
            {
                return _FilePathB;
            }

            set
            {
                if (_FilePathB != value)
                {
                    _FilePathB = value;
                    NotifyPropertyChanged(() => FilePathB);
                }
            }
        }

        /// <summary>
        /// Implements a find command via AvalonEdits build in searxh panel which can be
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

                        ////if (InlineDialog != InlineDialogMode.Find)
                        ////{
                        ////    InlineDialog = InlineDialogMode.Find;
                        ////}
                        ////else
                        ////{
                        ////    InlineDialog = InlineDialogMode.None;
                        ////}
                    },
                    (p) =>
                    {
                        return ApplicationCommands.Find.CanExecute(null, null);

                        ////return DiffCtrl.IsDiffDataAvailable;
                    });
                }

                return _FindTextCommand;
            }
        }

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
        #endregion properties

        #region methods

        #endregion methods
    }
}
