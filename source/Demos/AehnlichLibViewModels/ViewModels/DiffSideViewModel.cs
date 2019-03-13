namespace AehnlichLibViewModels.ViewModels
{
    using AehnlichLibViewModels.Enums;
    using AehnlichLibViewModels.Events;
    using AehnlichLibViewModels.Interfaces;
    using AehnlichLibViewModels.Tasks;
    using AehnlichViewLib.Controls.AvalonEditEx;
    using AehnlichViewLib.Events;
    using AehnlichViewLib.Interfaces;
    using ICSharpCode.AvalonEdit.Document;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    public class DiffSideViewModel : Base.ViewModelBase, ILineDiffProvider, IDisposable
    {
        #region fields
        private ChangeDiffOptions _ChangeDiffOptions;
        private TextDocument _document = null;
        private TextBoxController _TxtControl;

        private readonly DiffViewPosition _position;
        private int _Column;
        private int _Line;
        private int _spacesPerTab = 4;

        private DateTime _ViewActivation;
        private bool _isDirty = false;
        private string _FileName;
        private OneTaskLimitedScheduler _oneTaskScheduler;

        #region DiffLines
        private readonly ObservableRangeCollection<DiffLineViewModel> _DocLineDiffs;

        private int[] _diffEndLines = null;
        private int[] _diffStartLines = null;

        /// <summary>
        /// Maximum imaginary line number which incorporates not only real text lines
        /// but also imaginary line that where inserted on either side of the comparison
        /// view to sync both sides into a consistent display.
        /// </summary>
        private int _maxImaginaryLineNumber = 1;
        private bool _disposed;
        #endregion DiffLines
        #endregion fields

        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        public DiffSideViewModel()
        {
            _position = new DiffViewPosition(0,0);
            _DocLineDiffs = new ObservableRangeCollection<DiffLineViewModel>();
            _Line = 0;
            _Column = 0;

            _TxtControl = new TextBoxController();
            _ViewActivation = DateTime.MinValue;

            _oneTaskScheduler = new OneTaskLimitedScheduler();
        }
        #endregion ctors

        public event EventHandler<CaretPositionChangedEvent> CaretPositionChanged;

        /// <summary>
        /// Event is raised when newly requested line diff edit script segments
        /// have been computed and are available for hightlighting.
        /// 
        /// <seealso cref="ILineDiffProvider"/>
        /// </summary>
        public event EventHandler<DiffLineInfoChangedEvent> DiffLineInfoChanged;

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

        #region DiffLines
        public IReadOnlyList<DiffLineViewModel> DocLineDiffs
        {
            get
            {
                return _DocLineDiffs;
            }
        }

        public int LineCount => DocLineDiffs.Count;

        public int[] DiffStartLines => this._diffStartLines;
        public int[] DiffEndLines => this._diffEndLines;
        public int MaxLineNumber => this._maxImaginaryLineNumber;
        #endregion DiffLines

        #endregion properties

        #region methods
        /// <summary>
        /// Implements a method that is invoked by the view to request
        /// the matching (edit script computation) of the indicated text lines.
        /// 
        /// This method should be called on the UI thread since
        /// the resulting event <see cref="ILineDiffProvider.DiffLineInfoChanged"/>
        /// will be raised on the calling thread.
        /// </summary>
        /// <returns>Number of lines matched (may not be as requested if line appears to have been matched already).</returns>
        void ILineDiffProvider.RequestLineDiff(IEnumerable<int> linenumbers)
        {
            // Capture the current context and make sure resulting event is raised on that context
            var uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();

            Task.Factory.StartNew<List<int>>(() =>
            {
                var linesChanged = new List<int>();

                foreach (var i in linenumbers)
                {
                    if (_DocLineDiffs.Count <= i)
                        continue;

                    // We've previously seen and computed this?
                    if (DocLineDiffs[i].LineEditScriptSegmentsIsDirty == false)
                        continue;

                    DocLineDiffs[i].GetChangeEditScript(this.ChangeDiffOptions, _spacesPerTab);

                    // Its possible that we have to match this even though there is no result
                    // So, we optimize empty results away like this
                    if (DocLineDiffs[i].LineEditScriptSegments != null)
                        linesChanged.Add(i);
                }

                return linesChanged;
            }
            , CancellationToken.None, TaskCreationOptions.PreferFairness, _oneTaskScheduler
            ).ContinueWith((r) =>
            {
                // Tell the view we are done please redraw these if you like
                if (r.Result.Count > 0)
                {
                    this.DiffLineInfoChanged?.Invoke(this,
                        new DiffLineInfoChangedEvent(DiffLineInfoChange.LineEditScriptSegments, r.Result));
                }
            },
            uiScheduler);
        }

        /// <summary>
        /// Used to setup the ViewA/ViewB view that shows the left and right text views
        /// with the textual content and imaginary lines.
        /// each other.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="lines"></param>
        /// <param name="text"></param>
        internal void SetData(string filename,
                              IDiffLines lines, string text, int spacesPerTab)
        {
            this.FileName = filename;
            _position.SetPosition(0, 0);
            _spacesPerTab = spacesPerTab;
            Line = 0;
            Column = 0;

            if (lines != null)
            {
                _diffEndLines = lines.DiffEndLines;
                _diffStartLines = lines.DiffStartLines;
                _maxImaginaryLineNumber = lines.MaxImaginaryLineNumber;

                _DocLineDiffs.ReplaceRange(lines.DocLineDiffs);
            }
            else
            {
                _diffEndLines = null;
                _diffStartLines = null;
                _maxImaginaryLineNumber = 1;
                _DocLineDiffs.Clear();
            }

            Document = new TextDocument(text);
            NotifyPropertyChanged(() => Document);
        }

        /// <summary>
        /// Used to setup the ViewLineDiff view that shows only 2 lines over each other
        /// representing the currently active line from the left/right side views under
        /// each other.
        /// </summary>
        /// <param name="lineOneVM"></param>
        /// <param name="lineTwoVM"></param>
        internal void SetData(DiffLineViewModel lineOneVM,
                              DiffLineViewModel lineTwoVM,
                              int spacesPerTab)
        {
            _spacesPerTab = spacesPerTab;
            var documentLineDiffs = new List<DiffLineViewModel>();

            string text = string.Empty;

            if (lineOneVM != null && lineOneVM.LineEditScriptSegmentsIsDirty == true)
                lineOneVM.GetChangeEditScript(this.ChangeDiffOptions, spacesPerTab);

            if (lineTwoVM != null && lineTwoVM.LineEditScriptSegmentsIsDirty == true)
                lineTwoVM.GetChangeEditScript(this.ChangeDiffOptions, spacesPerTab);

            if (lineOneVM != null && lineTwoVM != null)
            {
                documentLineDiffs.Add(lineOneVM);
                text += lineOneVM.Text + '\n';

                documentLineDiffs.Add(lineTwoVM);
                text += lineTwoVM.Text + "\n";
            }

            text = text.Replace("\t", "    ");

            // Update LineInfo viewmodels
            _DocLineDiffs.Clear();
            _DocLineDiffs.AddRange(documentLineDiffs, NotifyCollectionChangedAction.Reset);
            NotifyPropertyChanged(() => DocLineDiffs);

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
            if (DiffStartLines == null || DiffEndLines == null)
                return false;

            bool result = false;

            int[] starts = DiffStartLines;
            int[] ends = DiffEndLines;

            result = starts.Length > 0 &&
                        ends.Length > 0 &&
                        (_position.Line < starts[0] || _position.Line > ends[0]);

            return result;
        }

        public bool CanGoToNextDiff()
        {
            if (DiffStartLines == null)
                return false;

            bool result = false;

            int[] starts = DiffStartLines;
            result = starts.Length > 0 && _position.Line < starts[starts.Length - 1];

            return result;
        }

        public bool CanGoToPreviousDiff()
        {
            if (DiffEndLines == null)
                return false;

            bool result = false;

            int[] ends = DiffEndLines;
            result = ends.Length > 0 && _position.Line > ends[0];

            return result;
        }

        public bool CanGoToLastDiff()
        {
            if (DiffStartLines == null || DiffEndLines == null)
                return false;

            bool result = false;

            int[] starts = DiffStartLines;
            int[] ends = DiffEndLines;
            result = starts.Length > 0 && ends.Length > 0 &&
                (_position.Line < starts[starts.Length - 1] || _position.Line > ends[ends.Length - 1]);

            return result;
        }

        internal void SetPosition(DiffViewPosition gotoPos)
        {
            _position.SetPosition(gotoPos.Line, gotoPos.Column);
        }

        /// <summary>
        /// Gets the position of the first difference in the text.
        /// </summary>
        /// <returns></returns>
        internal DiffViewPosition GetFirstDiffPosition()
        {
           return new DiffViewPosition(DiffStartLines[0], _position.Column);
        }

        /// <summary>
        /// Gets the position of the next difference in the text.
        /// </summary>
        /// <returns></returns>
        internal DiffViewPosition GetNextDiffPosition()
        {
            int[] starts = DiffStartLines;
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
            int[] ends = DiffEndLines;
            int numEnds = ends.Length;
            for (int i = numEnds - 1; i >= 0; i--)
            {
                if (_position.Line > ends[i])
                {
                    // I'm intentionally setting the line to Starts[i] here instead of Ends[i].
                    return new DiffViewPosition(DiffStartLines[i], _position.Column);
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
            int[] starts = DiffStartLines;

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
        /// Gets the n-th line of the diff stored in this viewmodel and returns it.
        /// </summary>
        /// <param name="lineN"></param>
        /// <returns></returns>
        internal DiffLineViewModel GetLine(int lineN)
        {
            if (lineN >= LineCount || LineCount == 0)
                return null;

            return DocLineDiffs[lineN];
        }

        internal DiffLineViewModel GotoTextLine(int thisLine)
        {
            DocumentLine line = Document.GetLineByNumber(thisLine);

            TxtControl.SelectText(line.Offset, 0);  // Select text with length 0 and scroll to where
            TxtControl.ScrollToLine(thisLine);     // we are supposed to be at

            return _DocLineDiffs[thisLine-1];
        }

        internal int FindThisTextLine(int thisLine)
        {
            // Translate given line number into real line number (adding virtual lines if any)
            int idx = Math.Min(thisLine, _DocLineDiffs.Count-1);
            if (idx < 0)
                idx = 0;

            var model = _DocLineDiffs[idx];

            int iCurrLineNumber = (model.ImaginaryLineNumber == null ? 0 : (int)model.ImaginaryLineNumber);

            // TODO: Naive search should be binary later on
            if (iCurrLineNumber < thisLine)
            {
                for (; idx < _DocLineDiffs.Count; idx++)
                {
                    model = _DocLineDiffs[idx];
                    iCurrLineNumber = (model.ImaginaryLineNumber == null ? 0 : (int)model.ImaginaryLineNumber);

                    if (iCurrLineNumber >= thisLine)
                        break;
                }
            }

            return idx;
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
                    _oneTaskScheduler.Dispose();
                    _oneTaskScheduler = null;
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
        #endregion methods
    }
}