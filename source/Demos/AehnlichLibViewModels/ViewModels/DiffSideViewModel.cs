namespace AehnlichLibViewModels.ViewModels
{
    using AehnlichLib.Text;
    using AehnlichLibViewModels.Enums;
    using AehnlichLibViewModels.Events;
    using AehnlichLibViewModels.ViewModels.LineInfo;
    using AehnlichViewLib.Controls.AvalonEditEx;
    using ICSharpCode.AvalonEdit.Document;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    public class DiffSideViewModel : Base.ViewModelBase
    {
        #region fields
        private ChangeDiffOptions _ChangeDiffOptions;
        private readonly DiffLinesViewModel _DiffLines;
        private TextDocument _document = null;
        private TextBoxController _TxtControl;

        private DiffViewPosition _position;
        private int _Column;
        private int _Line;

        private DateTime _ViewActivation;
        private bool _isDirty = false;
        private string _FileName;
        #endregion fields

        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        public DiffSideViewModel()
        {
            _DiffLines = new DiffLinesViewModel();
            _Line = 0;
            _Column = 0;

            _TxtControl = new TextBoxController();
            _ViewActivation = DateTime.MinValue;
        }
        #endregion ctors

        public EventHandler<CaretPositionChangedEvent> CaretPositionChanged;

        #region properties
        public TextDocument Document
        {
            get { return this._document; }
            set
            {
                if (this._document != value)
                {
                    this._document = value;
                    NotifyPropertyChanged(() => Document);
                }
            }
        }

        /// <summary>
        /// Gets/sets the textbox controller that is used to drive the view
        /// from within the viewmodel (with event based commands like goto line x,y).
        /// </summary>
        public TextBoxController TxtControl
        {
            get { return _TxtControl; }

            private set
            {
                if (_TxtControl != value)
                {
                    _TxtControl = value;
                    NotifyPropertyChanged(() => TxtControl);
                }
            }
        }

        /// <summary>
        /// Gets/set the time stamp of the last time when the attached view
        /// has been activated (GotFocus).
        /// </summary>
        public DateTime ViewActivation
        {
            get
            {
                return _ViewActivation;
            }

            set
            {
                if (_ViewActivation != value)
                {
                    _ViewActivation = value;
                    NotifyPropertyChanged(() => ViewActivation);
                }
            }
        }

        #region Caret Position
        public int Column
        {
            get
            {
                return _Column;
            }

            set
            {
                if (_Column != value)
                {
                    _Column = value;
                    NotifyPropertyChanged(() => Column);

                    CaretPositionChanged?.Invoke(this,
                        new CaretPositionChangedEvent(_Line, _Column, CaretChangeType.Column));
                }
            }
        }

        public int Line
        {
            get
            {
                return _Line;
            }

            set
            {
                if (_Line != value)
                {
                    _Line = value;
                    NotifyPropertyChanged(() => Line);

                    CaretPositionChanged?.Invoke(this,
                        new CaretPositionChangedEvent(_Line, _Column, CaretChangeType.Line));
                }
            }
        }
        #endregion Caret Position

        public DiffLinesViewModel DiffLines
        {
            get
            {
                return _DiffLines;
            }
        }

        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                if (_isDirty != value)
                {
                    _isDirty = value;
                    NotifyPropertyChanged(() => IsDirty);
                }
            }
        }

        public ChangeDiffOptions ChangeDiffOptions
        {
            get
            {
                return _ChangeDiffOptions;
            }

            internal set
            {
                if (_ChangeDiffOptions != value)
                {
                    _ChangeDiffOptions = value;
                    NotifyPropertyChanged(() => ChangeDiffOptions);
                }
            }
        }

        public string FileName
        {
            get { return this._FileName; }
            set
            {
                if (this._FileName != value)
                {
                    this._FileName = value;
                    NotifyPropertyChanged(() => FileName);
                }
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Used to setup the ViewA/ViewB view that shows the left and right text views
        /// with the textual content and imaginary lines.
        /// each other.
        /// </summary>
        /// <param name="lineOne"></param>
        /// <param name="lineTwo"></param>
        /// <param name="filename"></param>
        /// <param name="stringList"></param>
        /// <param name="script"></param>
        /// <param name="useA">Set to true if this data represents the reference view
        /// (left view also known as ViewA) otherwise false.</param>
        public void SetData(string filename,
                            IList<string> stringList,
                            EditScript script, bool useA)
        {
            this.FileName = filename;

            string text = _DiffLines.SetData(stringList, script, useA);
            Document = new TextDocument(text);
            NotifyPropertyChanged(() => Document);
        }

        /// <summary>
        /// Used to setup the ViewLineDiff view that shows only 2 lines over each other
        /// representing the currently active line from the left/right side views under
        /// each other.
        /// </summary>
        /// <param name="lineOne"></param>
        /// <param name="lineTwo"></param>
        internal void SetData(DiffLineViewModel lineOneVM,
                              DiffLineViewModel lineTwoVM,
                              int spacesPerTab)
        {
            string text = _DiffLines.SetData(lineOneVM, lineTwoVM, spacesPerTab, this.ChangeDiffOptions);

            // Update text document
            Document = new TextDocument(text);
            NotifyPropertyChanged(() => Document);
        }

        #region FirstDiff NextDiff PrevDiff LastDiff
        /// <summary>
        /// Determine whether or not we can goto the first difference in the model.
        /// 
        /// The function returns false:
        /// - if there is no difference or
        /// - if current positioning already indicates positioning on the 1st difference
        /// </summary>
        public bool CanGoToFirstDiff()
        {
            if (_DiffLines.DiffStartLines == null || _DiffLines.DiffEndLines == null)
                return false;

            bool result = false;

            int[] starts = _DiffLines.DiffStartLines;
            int[] ends = _DiffLines.DiffEndLines;

            result = starts.Length > 0 &&
                        ends.Length > 0 &&
                        (_position.Line < starts[0] || _position.Line > ends[0]);

            return result;
        }

        public bool CanGoToNextDiff()
        {
            if (_DiffLines.DiffStartLines == null)
                return false;

            bool result = false;

            int[] starts = _DiffLines.DiffStartLines;
            result = starts.Length > 0 && _position.Line < starts[starts.Length - 1];

            return result;
        }

        public bool CanGoToPreviousDiff()
        {
            if (_DiffLines.DiffEndLines == null)
                return false;

            bool result = false;

            int[] ends = _DiffLines.DiffEndLines;
            result = ends.Length > 0 && _position.Line > ends[0];

            return result;
        }

        public bool CanGoToLastDiff()
        {
            if (_DiffLines.DiffStartLines == null || _DiffLines.DiffEndLines == null)
                return false;

            bool result = false;

            int[] starts = _DiffLines.DiffStartLines;
            int[] ends = _DiffLines.DiffEndLines;
            result = starts.Length > 0 && ends.Length > 0 &&
                (_position.Line < starts[starts.Length - 1] || _position.Line > ends[ends.Length - 1]);

            return result;
        }

        internal void GetChangeEditScript(int firstLine, int lastLine, int spacesPerTab)
        {
            if (firstLine < 0 || lastLine <= 0)
                return;

            for (int i = firstLine; i < _DiffLines.LineCount && i < lastLine; i++)
            {
                // We've previously seen and computed this?
                if (_DiffLines.DocLineDiffs[i].LineEditScriptSegmentsIsDirty == false)
                    continue;

                if (_DiffLines.DocLineDiffs[i].LineEditScriptSegmentsIsDirty)
                    _DiffLines.DocLineDiffs[i].GetChangeEditScript(this.ChangeDiffOptions, spacesPerTab);
            }
        }

        internal void SetPosition(DiffViewPosition gotoPos)
        {
            _position = new DiffViewPosition(gotoPos.Line, gotoPos.Column);
        }

        /// <summary>
        /// Gets the position of the first difference in the text.
        /// </summary>
        /// <returns></returns>
        internal DiffViewPosition GetFirstDiffPosition()
        {
           return new DiffViewPosition(_DiffLines.DiffStartLines[0], _position.Column);
        }

        /// <summary>
        /// Gets the position of the next difference in the text.
        /// </summary>
        /// <returns></returns>
        internal DiffViewPosition GetNextDiffPosition()
        {
            int[] starts = _DiffLines.DiffStartLines;
            int numStarts = starts.Length;
            for (int i = 0; i < numStarts; i++)
            {
                if (_position.Line < starts[i])
                {
                    return new DiffViewPosition(starts[i], _position.Column);
                }
            }

            // We should never get here.
            Debug.Assert(false, "CanGoToPreviousDiff was wrong.");
            return default(DiffViewPosition);
        }

        /// <summary>
        /// Gets the position of the previous difference in the text.
        /// </summary>
        /// <returns></returns>
        internal DiffViewPosition GetPrevDiffPosition()
        {
            int[] ends = _DiffLines.DiffEndLines;
            int numEnds = ends.Length;
            for (int i = numEnds - 1; i >= 0; i--)
            {
                if (_position.Line > ends[i])
                {
                    // I'm intentionally setting the line to Starts[i] here instead of Ends[i].
                    return new DiffViewPosition(_DiffLines.DiffStartLines[i], _position.Column);
                }
            }

            // We should never get here.
            Debug.Assert(false, "CanGoToPreviousDiff was wrong.");
            return default(DiffViewPosition);
        }

        /// <summary>
        /// Gets the position of the last difference in the text.
        /// </summary>
        /// <returns></returns>
        internal DiffViewPosition GetLastDiffPosition()
        {
            int[] starts = _DiffLines.DiffStartLines;

            return new DiffViewPosition(starts[starts.Length - 1], _position.Column);
        }
        #endregion FirstDiff NextDiff PrevDiff LastDiff

        /// <summary>
        /// Scrolls the attached view to line <paramref name="n"/>
        /// where n should in the range of [1 ... max lines].
        /// </summary>
        /// <param name="n"></param>
        internal void ScrollToLine(int n)
        {
            DocumentLine line = Document.GetLineByNumber(n);
            TxtControl.SelectText(line.Offset, 0);          // Select text with length 0 and scroll to where
            TxtControl.ScrollToLine(n);                    // we are supposed to be at
        }

        /// <summary>
        /// Sets the Counterpart property in each line property of each
        /// <see cref="DiffSideViewModel"/> to refer to each other. This information
        /// can be used for finding equivelant from left to right lines[] collection
        /// and vice versa.
        /// </summary>
        /// <param name="counterpartView"></param>
        public void SetCounterPartLines(DiffSideViewModel counterpartView)
        {
            _DiffLines.SetCounterPartLines(counterpartView._DiffLines);
        }

        internal DiffLineViewModel GetLine(int line)
        {
            if (line >= _DiffLines.LineCount || _DiffLines.LineCount == 0)
                return null;

            return _DiffLines.DocLineDiffs[line];
        }
        #endregion methods
    }
}