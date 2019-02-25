namespace AehnlichLibViewModels.ViewModels
{
    using AehnlichLibViewModels.ViewModels.Base;
    using ICSharpCode.AvalonEdit.Document;
    using System.Windows.Input;

    public class AppViewModel : Base.ViewModelBase
    {
        #region fields
        private string _FilePathA;
        private string _FilePathB;
        private ICommand _CompareFilesCommand;
        private ICommand _GoToFirstDifferenceCommand;
        private ICommand _GoToNextDifferenceCommand;
        private ICommand _GoToPrevDifferenceCommand;
        private ICommand _GoToLastDifferenceCommand;
        private readonly FileDiffFormViewModel _DiffForm;
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
                        if (GoToFirstDifferenceCommand.CanExecute(null))
                            GoToFirstDifferenceCommand.Execute(null);
                    });
                }

                return _CompareFilesCommand;
            }
        }

        #region Goto Diff Commands
        /// <summary>
        /// Gets a command that positions the diff viewer at the first detected difference.
        /// </summary>
        public ICommand GoToFirstDifferenceCommand
        {
            get
            {
                if (_GoToFirstDifferenceCommand == null)
                {
                    _GoToFirstDifferenceCommand = new RelayCommand<object>((p) =>
                    {
                        DiffSideViewModel nonActView;
                        DiffSideViewModel activeView = DiffForm.DiffCtrl.GetActiveView(out nonActView);
                        DiffViewPosition gotoPos = activeView.GetFirstDiffPosition();
                        DiffForm.DiffCtrl.ScrollToLine(gotoPos, nonActView, activeView);
                    },
                    (p) =>
                    {
                        DiffSideViewModel nonActView;
                        DiffSideViewModel activeView = DiffForm.DiffCtrl.GetActiveView(out nonActView);
                        if (activeView == null)
                            return false;

                        bool isEnabled = activeView.CanGoToFirstDiff();

                        return isEnabled;
                    });
                }

                return _GoToFirstDifferenceCommand;
            }
        }

        /// <summary>
        /// Gets a command that positions the diff viewer at the next detected difference.
        /// </summary>
        public ICommand GoToNextDifferenceCommand
        {
            get
            {
                if (_GoToNextDifferenceCommand == null)
                {
                    _GoToNextDifferenceCommand = new RelayCommand<object>((p) =>
                    {
                        DiffSideViewModel nonActView;
                        DiffSideViewModel activeView = DiffForm.DiffCtrl.GetActiveView(out nonActView);
                        DiffViewPosition gotoPos = activeView.GetNextDiffPosition();
                        DiffForm.DiffCtrl.ScrollToLine(gotoPos, nonActView, activeView);
                    },
                    (p) =>
                    {
                        DiffSideViewModel nonActView;
                        DiffSideViewModel activeView = DiffForm.DiffCtrl.GetActiveView(out nonActView);
                        if (activeView == null)
                            return false;

                        bool isEnabled = activeView.CanGoToNextDiff();

                        return isEnabled;
                    });
                }

                return _GoToNextDifferenceCommand;
            }
        }

        /// <summary>
        /// Gets a command that positions the diff viewer at a previously detected difference.
        /// </summary>
        public ICommand GoToPrevDifferenceCommand
        {
            get
            {
                if (_GoToPrevDifferenceCommand == null)
                {
                    _GoToPrevDifferenceCommand = new RelayCommand<object>((p) =>
                    {
                        DiffSideViewModel nonActView;
                        DiffSideViewModel activeView = DiffForm.DiffCtrl.GetActiveView(out nonActView);
                        DiffViewPosition gotoPos = activeView.GetPrevDiffPosition();
                        DiffForm.DiffCtrl.ScrollToLine(gotoPos, nonActView, activeView);
                    },
                    (p) =>
                    {
                        DiffSideViewModel nonActView;
                        DiffSideViewModel activeView = DiffForm.DiffCtrl.GetActiveView(out nonActView);
                        if (activeView == null)
                            return false;

                        bool isEnabled = activeView.CanGoToPreviousDiff();

                        return isEnabled;
                    });
                }

                return _GoToPrevDifferenceCommand;
            }
        }

        /// <summary>
        /// Gets a command that positions the diff viewer at the last detected difference.
        /// </summary>
        public ICommand GoToLastDifferenceCommand
        {
            get
            {
                if (_GoToLastDifferenceCommand == null)
                {
                    _GoToLastDifferenceCommand = new RelayCommand<object>((p) =>
                    {
                        DiffSideViewModel nonActView;
                        DiffSideViewModel activeView = DiffForm.DiffCtrl.GetActiveView(out nonActView);
                        DiffViewPosition gotoPos = activeView.GetLastDiffPosition();
                        DiffForm.DiffCtrl.ScrollToLine(gotoPos, nonActView, activeView);
                    },
                    (p) =>
                    {
                        DiffSideViewModel nonActView;
                        DiffSideViewModel activeView = DiffForm.DiffCtrl.GetActiveView(out nonActView);
                        if (activeView == null)
                            return false;

                        bool isEnabled = activeView.CanGoToLastDiff();

                        return isEnabled;
                    });
                }

                return _GoToLastDifferenceCommand;
            }
        }
        #endregion Goto Diff Commands

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
