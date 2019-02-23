namespace DiffLibViewModels.ViewModels
{
    using DiffLib.Text;
    using DiffLibViewModels.Enums;
    using DiffLibViewModels.Events;
    using DiffViewLib.Enums;
    using ICSharpCode.AvalonEdit.Document;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Text;

    public class DiffSideViewModel : Base.ViewModelBase
    {
        #region fields
        private ChangeDiffOptions _ChangeDiffOptions;

        private DiffViewLines _lines;
        private TextDocument _document = null;

        private readonly ObservableRangeCollection<DiffContext> _DocLineDiffs;

        private bool _isDirty = false;
        private int _Column;
        private int _Line;
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
        #endregion properties

        #region methods

        /// <summary>
        /// Used to setup the ViewA/ViewB view that shows the left and right text views
        /// with the textual content and imaginary lines.
        /// each other.
        /// </summary>
        /// <param name="lineOne"></param>
        /// <param name="lineTwo"></param>
        public void SetData(IList<string> stringList, EditScript script, bool useA)
        {
            this._lines = new DiffViewLines(stringList, script, useA);
            NotifyPropertyChanged(() => LineCount);

            IList<DiffContext> lineDiffs;
            string text = GetDocumentFromRawLines(out lineDiffs);

            _DocLineDiffs.Clear();
            _DocLineDiffs.AddRange(lineDiffs, NotifyCollectionChangedAction.Reset);

            Document = new TextDocument(text);

            NotifyPropertyChanged(() => Document);

            this.UpdateAfterSetData();
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
                case DiffLib.Enums.EditType.Delete:
                    lineContext = DiffContext.Deleted;
                    break;
                case DiffLib.Enums.EditType.Insert:
                    lineContext = DiffContext.Added;
                    break;
                case DiffLib.Enums.EditType.Change:
                    lineContext = DiffContext.Context;
                    break;

                case DiffLib.Enums.EditType.None:
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