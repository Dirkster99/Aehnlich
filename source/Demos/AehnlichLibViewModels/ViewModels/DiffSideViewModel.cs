namespace AehnlichLibViewModels.ViewModels
{
    using AehnlichLib.Text;
    using AehnlichLibViewModels.Enums;
    using AehnlichLibViewModels.Events;
    using AehnlichViewLib.Controls;
    using AehnlichViewLib.Enums;
    using ICSharpCode.AvalonEdit.Document;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Text;

    public class DiffSideViewModel : Base.ViewModelBase
    {
        #region fields
        private ChangeDiffOptions _ChangeDiffOptions;

        private DiffViewLines _lines;
        private DiffViewPosition _position;

        private TextDocument _document = null;
        private TextBoxController _TxtControl;

        private readonly ObservableRangeCollection<DiffContext> _DocLineDiffs;

        private DateTime _ViewActivation;
        private bool _isDirty = false;
        private int _Column;
        private int _Line;
        private string _FileName;
        #endregion fields

        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        public DiffSideViewModel()
        {
            _DocLineDiffs = new ObservableRangeCollection<DiffContext>();
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

        public IReadOnlyList<DiffContext> DocLineDiffs
        {
            get
            {
                return _DocLineDiffs;
            }
        }

        public int LineCount => this._lines != null ? this._lines.Count : 0;

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
        public void SetData(string filename,
                            IList<string> stringList,
                            EditScript script, bool useA)
        {
            this.FileName = filename;
            _lines = new DiffViewLines(stringList, script, useA);
            NotifyPropertyChanged(() => LineCount);

            IList<DiffContext> lineDiffs;
            string text = GetDocumentFromRawLines(out lineDiffs);

            _DocLineDiffs.Clear();
            _DocLineDiffs.AddRange(lineDiffs, NotifyCollectionChangedAction.Reset);

            Document = new TextDocument(text);

            NotifyPropertyChanged(() => Document);

            UpdateAfterSetData();
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
            bool result = false;

            if (_lines != null)
            {
                int[] starts = _lines.DiffStartLines;
                int[] ends = _lines.DiffEndLines;
                result = starts.Length > 0 &&
                            ends.Length > 0 &&
                            (_position.Line < starts[0] || _position.Line > ends[0]);
            }

            return result;
        }

        public bool CanGoToNextDiff()
        {
            bool result = false;
            if (_lines != null)
            {
                int[] starts = _lines.DiffStartLines;
                result = starts.Length > 0 && _position.Line < starts[starts.Length - 1];
            }

            return result;
        }

        public bool CanGoToPreviousDiff()
        {
            bool result = false;

            if (_lines != null)
            {
                int[] ends = _lines.DiffEndLines;
                result = ends.Length > 0 && _position.Line > ends[0];
            }

            return result;
        }

        public bool CanGoToLastDiff()
        {
            bool result = false;

            if (_lines != null)
            {
                int[] starts = _lines.DiffStartLines;
                int[] ends = _lines.DiffEndLines;
                result = starts.Length > 0 && ends.Length > 0 &&
                    (_position.Line < starts[starts.Length - 1] || _position.Line > ends[ends.Length - 1]);
            }

            return result;
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
           return new DiffViewPosition(_lines.DiffStartLines[0], _position.Column);
        }

        /// <summary>
        /// Gets the position of the next difference in the text.
        /// </summary>
        /// <returns></returns>
        internal DiffViewPosition GetNextDiffPosition()
        {
            int[] starts = _lines.DiffStartLines;
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
            int[] ends = _lines.DiffEndLines;
            int numEnds = ends.Length;
            for (int i = numEnds - 1; i >= 0; i--)
            {
                if (_position.Line > ends[i])
                {
                    // I'm intentionally setting the line to Starts[i] here instead of Ends[i].
                    return new DiffViewPosition(_lines.DiffStartLines[i], _position.Column);
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
            int[] starts = _lines.DiffStartLines;

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

        private string GetDocumentFromRawLines(out IList<DiffContext> documentLineDiffs)
        {
            documentLineDiffs = new List<DiffContext>();
            StringBuilder ret = new StringBuilder();

            foreach (var item in _lines)
            {
                documentLineDiffs.Add(TranslateLineContext(item));
                ret.Append(item.Text + '\n');
            }

            return ret.ToString();
        }

        private DiffContext TranslateLineContext(DiffViewLine item)
        {
            DiffContext lineContext = DiffContext.Blank;
            switch (item.EditType)
            {
                case AehnlichLib.Enums.EditType.Delete:
                    lineContext = DiffContext.Deleted;
                    break;
                case AehnlichLib.Enums.EditType.Insert:
                    lineContext = DiffContext.Added;
                    break;
                case AehnlichLib.Enums.EditType.Change:
                    lineContext = DiffContext.Context;
                    break;

                case AehnlichLib.Enums.EditType.None:
                default:
                    break;
            }

            return lineContext;
        }

        /// <summary>
        /// Used to setup the ViewLineDiff view that shows only 2 lines over each other
        /// representing the currently active line from the left/right side views under
        /// each other.
        /// </summary>
        /// <param name="lineOne"></param>
        /// <param name="lineTwo"></param>
        internal void SetData(DiffViewLine lineOne, DiffViewLine lineTwo)
        {
            _lines = new DiffViewLines(lineOne, lineTwo);
            var documentLineDiffs = new List<DiffContext>();

            string text = string.Empty;

            if (lineOne != null && lineTwo != null)
            {
                _DocLineDiffs.Add(TranslateLineContext(lineOne));
                documentLineDiffs.Add(TranslateLineContext(lineOne));
                text += lineOne.Text + '\n';

                _DocLineDiffs.Add(TranslateLineContext(lineTwo));
                documentLineDiffs.Add(TranslateLineContext(lineTwo));
                text += lineTwo.Text + "\n";
            }

            _DocLineDiffs.Clear();
            _DocLineDiffs.AddRange(documentLineDiffs, NotifyCollectionChangedAction.Reset);
            NotifyPropertyChanged(() => DocLineDiffs);

            Document = new TextDocument(text);
            NotifyPropertyChanged(() => Document);

            this.UpdateAfterSetData();
        }

        /// <summary>
        /// Sets the Counterpart property in each line property of each
        /// <see cref="DiffSideViewModel"/> to refer to each other. This information
        /// can be used for finding equivelant from left to right lines[] collection
        /// and vice versa.
        /// </summary>
        /// <param name="counterpartView"></param>
        public void SetCounterpartLines(DiffSideViewModel counterpartView)
        {
            int numLines = this.LineCount;
            if (numLines != counterpartView.LineCount)
            {
                throw new ArgumentException("The counterpart view has a different number of view lines.", nameof(counterpartView));
            }

            for (int i = 0; i < numLines; i++)
            {
                DiffViewLine line = this._lines[i];
                DiffViewLine counterpart = counterpartView._lines[i];

                // Make the counterpart lines refer to each other.
                line.Counterpart = counterpart;
                counterpart.Counterpart = line;
            }
        }

        private void UpdateAfterSetData()
        {
            // Reset the position before we start calculating things
////            this.position = new DiffViewPosition(0, 0);
////            this.selectionStart = DiffViewPosition.Empty;
////
////            // We have to call this to recalc the gutter width
////            this.UpdateTextMetrics(false);
////
////            // We have to call this to setup the scroll bars
////            this.SetupScrollBars();
////
////            // Reset the scroll position
////            this.VScrollPos = 0;
////            this.HScrollPos = 0;
////
////            // Update the caret
////            this.UpdateCaret();
////
////            // Force a repaint
////            this.Invalidate();
////
////            // Fire the LinesChanged event
////            if (this.LinesChanged != null)
////            {
////                this.LinesChanged(this, EventArgs.Empty);
////            }
////
////            // Fire the position changed event
////            if (this.PositionChanged != null)
////            {
////                this.PositionChanged(this, EventArgs.Empty);
////            }
////
////            this.FireSelectionChanged();
        }

        internal DiffViewLine GetLine(int line)
        {
            if (line >= this.LineCount)
                return null;

            return _lines[line];
        }
        #endregion methods
    }
}