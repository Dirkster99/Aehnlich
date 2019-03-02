namespace AehnlichLibViewModels.ViewModels
{
    using AehnlichLibViewModels.Models;
    using AehnlichLibViewModels.ViewModels.Base;
    using AehnlichViewLib.Models;
    using ICSharpCode.AvalonEdit;
    using System.Windows.Input;

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
        private readonly FileDiffFormViewModel _DiffForm;
        private readonly object lockObject = new object();
        #endregion fields

        #region ctors
        public AppViewModel(string fileA, string fileB)
            : this()
        {
            _FilePathA = fileA;
            _FilePathB = fileB;
        }

        public AppViewModel()
        {
            _DiffForm = new FileDiffFormViewModel();
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

                        _DiffForm.ShowDifferences(new Models.ShowDiffArgs(fileA, fileB, Enums.DiffType.File));
                        NotifyPropertyChanged(() => DiffForm);

                        // Position view on first difference if thats available
                        if (_DiffForm.DiffCtrl.GoToFirstDifferenceCommand.CanExecute(null))
                            _DiffForm.DiffCtrl.GoToFirstDifferenceCommand.Execute(null);
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
                        DiffSideViewModel activeView = DiffForm.DiffCtrl.GetActiveView(out nonActView);

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
                        DiffSideViewModel activeView = DiffForm.DiffCtrl.GetActiveView(out nonActView);

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
                        DiffSideViewModel activeView = DiffForm.DiffCtrl.GetActiveView(out nonActView);

                        string textSelection = activeView.TxtControl.GetSelectedText();
                        FileSystemCommands.CopyString(textSelection);
                    }, (p) =>
                    {
                        DiffSideViewModel nonActView;
                        DiffSideViewModel activeView = DiffForm.DiffCtrl.GetActiveView(out nonActView);

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

                        lock (lockObject)
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

                        // Translate from 1-based values to tero-based values
                        _DiffForm.GetChangeEditScript(param.FirstLine-1, param.LastLine-1, spacesPerTab);

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
                        lock (lockObject)
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
                            DiffSideViewModel activeView = DiffForm.DiffCtrl.GetActiveView(out nonActView);
                            DiffViewPosition gotoPos = new DiffViewPosition((int)param, 0);
                            DiffForm.DiffCtrl.ScrollToLine(gotoPos, nonActView, activeView);
                        }
                    },
                    (p) =>
                    {
                        DiffSideViewModel nonActView;
                        DiffSideViewModel activeView = DiffForm.DiffCtrl.GetActiveView(out nonActView);
                        if (activeView == null)
                            return false;

                        return true;
                    });
                }

                return _OverviewValueChangedCommand;
            }
        }

        public FileDiffFormViewModel DiffForm
        {
            get { return _DiffForm; }
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

            protected set
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

            protected set
            {
                if (_FilePathB != value)
                {
                    _FilePathB = value;
                    NotifyPropertyChanged(() => FilePathB);
                }
            }
        }
        #endregion properties

        #region methods

        #endregion methods
    }
}
