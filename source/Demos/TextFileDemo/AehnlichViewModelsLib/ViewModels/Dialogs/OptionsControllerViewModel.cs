namespace AehnlichViewModelsLib.ViewModels.Dialogs
{
    using AehnlichLib.Enums;
    using AehnlichLib.Models;
    using AehnlichViewModelsLib.Enums;
    using AehnlichViewModelsLib.ViewModels.Base;
    using ICSharpCode.AvalonEdit;
    using System;
    using System.Windows.Input;

    internal class OptionsControllerViewModel : Base.ViewModelBase, IOptionsControllerViewModel
    {
        #region fields
        readonly private Func<InlineDialogMode, InlineDialogMode> _closeFunc;
        private RelayCommand<object> _CloseDialogCommand;
        private CompareType _OptionCompareType;
        private bool _IgnoreCase;
        private bool _IgnoreTextWhitespace;
        private bool _IgnoreXmlWhitespace;
        private bool _ShowChangeAsDeleteInsert;

        private TextEditorOptions _DiffDisplayOptions;
        private uint _SpacesPerTabValue;
        #endregion fields

        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="closeFunc"></param>
        public OptionsControllerViewModel(Func<InlineDialogMode, InlineDialogMode> closeFunc)
            : this()
        {
            _closeFunc = closeFunc;
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        protected OptionsControllerViewModel()
        {
            _IgnoreCase = true;
            _IgnoreXmlWhitespace = true;
            _IgnoreTextWhitespace = true;

            _SpacesPerTabValue = 4;

            _DiffDisplayOptions = new TextEditorOptions()
            {
                ShowTabs = false,
                ConvertTabsToSpaces = true,
                IndentationSize = 4,
                HighlightCurrentLine = true,
                EnableVirtualSpace = true,
                AllowScrollBelowDocument = false
            };
        }
        #endregion ctors

        #region properties
        /// <summary>
        /// Gets the view options for the AvalonEdit view/editor.
        /// </summary>
        public TextEditorOptions DiffDisplayOptions
        {
            get
            {
                return _DiffDisplayOptions;
            }
        }

        /// <summary>
        /// Gets/sets whether the type of media being compared is determined automatically,
        /// or should be interpreted as Text, XML, or Binary.
        /// </summary>
        public CompareType OptionCompareType
        {
            get { return _OptionCompareType; }

            set
            {
                if (_OptionCompareType != value)
                {
                    _OptionCompareType = value;
                    NotifyPropertyChanged(() => OptionCompareType);
                }
            }
        }

        /// <summary>
        /// Gets/sets wether the text should by compared text case-sensitive
        /// or not (case-insensitive).
        /// </summary>
        public bool IgnoreCase
        {
            get { return _IgnoreCase; }

            set
            {
                if (_IgnoreCase != value)
                {
                    _IgnoreCase = value;
                    NotifyPropertyChanged(() => IgnoreCase);
                }
            }
        }


        /// <summary>
        /// Gets/sets whether to ignore starting and ending white spaces when comparing strings.
        /// 
        /// Turn this on to get these equalities:
        /// - Strings that contain only whitespaces are equal.
        /// - Two strings like '  A' and 'A  ' are equal.
        /// </summary>
        public bool IgnoreTextWhitespace
        {
            get { return _IgnoreTextWhitespace; }

            set
            {
                if (_IgnoreTextWhitespace != value)
                {
                    _IgnoreTextWhitespace = value;
                    NotifyPropertyChanged(() => IgnoreTextWhitespace);
                }
            }
        }

        /// <summary>
        /// Gets/sets whether to ignore insignificant white space when comparing Xml content.
        /// </summary>
        public bool IgnoreXmlWhitespace
        {
            get { return _IgnoreXmlWhitespace; }

            set
            {
                if (_IgnoreXmlWhitespace != value)
                {
                    _IgnoreXmlWhitespace = value;
                    NotifyPropertyChanged(() => IgnoreXmlWhitespace);
                }
            }
        }

        /// <summary>
        /// Gets/sets whether lines that can be aligned as change are displayed as
        /// changed lines with in-line differences, or whether line differences are
        /// only compared and displayed with inserted and deleted lines.
        /// </summary>
        public bool ShowChangeAsDeleteInsert
        {
            get { return _ShowChangeAsDeleteInsert; }

            set
            {
                if (_ShowChangeAsDeleteInsert != value)
                {
                    _ShowChangeAsDeleteInsert = value;
                    NotifyPropertyChanged(() => ShowChangeAsDeleteInsert);
                }
            }
        }

        #region SpacesPerTab Option
        public uint SpacesPerTabValue
        {
            get { return _SpacesPerTabValue; }

            set
            {
                if (_SpacesPerTabValue != value)
                {
                    _SpacesPerTabValue = value;
                    NotifyPropertyChanged(() => SpacesPerTabValue);
                }
            }
        }

        public uint SpacesPerTabMin { get { return 1; } }

        public uint SpacesPerTabMax { get { return 8; } }

        #endregion SpacesPerTab Option

        /// <summary>
        /// Gets a command that closes the dialog via the delegate function
        /// that was supplied at constructor time of this object.
        /// </summary>
        public ICommand CloseDialogCommand
        {
            get
            {
                if (_CloseDialogCommand == null)
                {
                    _CloseDialogCommand = new RelayCommand<object>((p) =>
                    {
                        _closeFunc(InlineDialogMode.None);
                    },
                    (p) =>
                    {
                        return (_closeFunc == null ? false : true);
                    });
                }

                return _CloseDialogCommand;
            }
        }
        #endregion properties

        #region methods
        internal TextBinaryDiffArgs GetTextBinaryDiffSetup(string A, string B)
        {
            var setup = new TextBinaryDiffArgs(A, B, DiffType.File);

            setup.CompareType = this.OptionCompareType;
            setup.IgnoreCase = this.IgnoreCase;
            setup.IgnoreTextWhitespace = this.IgnoreTextWhitespace;
            setup.IgnoreXmlWhitespace = this.IgnoreXmlWhitespace;
            setup.ShowChangeAsDeleteInsert = this.ShowChangeAsDeleteInsert;
            setup.SpacesPerTab = (int)this.SpacesPerTabValue;

            return setup;
        }
        #endregion methods
    }
}
