namespace AehnlichLibViewModels.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using AehnlichLib.Text;
    using AehnlichLibViewModels.Enums;
    using AehnlichLibViewModels.Events;

    /// <summary>
    /// DiffControl
    /// </summary>
    public class DiffDocViewModel : Base.ViewModelBase
    {
        #region fields
        private readonly DiffSideViewModel _ViewA;
        private readonly DiffSideViewModel _ViewB;
        private readonly DiffSideViewModel _ViewLineDiff;

        private int _LineDiffHeight = 38;
        private string _Similarity_Text;
        private bool _edtLeft_Right_Visible;
        private string _edtRight_Text;
        private string _edtLeft_Text;
        private int currentDiffLine = -1;
        private int _SynchronizedLine = 1;
        private int _SynchronizedColumn = 0;

        ////        private int currentDiffLine = -1;
        #endregion fields

        #region ctors
        /// <summary>
        /// class constructor
        /// </summary>
        public DiffDocViewModel()
        {
            _ViewA = new DiffSideViewModel();
            _ViewB = new DiffSideViewModel();
            _ViewLineDiff = new DiffSideViewModel();

            _ViewA.CaretPositionChanged += OnViewACaretPositionChanged;
            _ViewB.CaretPositionChanged += OnViewBCaretPositionChanged;
        }
        #endregion ctors

        #region properties
        public DiffSideViewModel ViewA
        {
            get { return _ViewA; }
        }

        public DiffSideViewModel ViewB
        {
            get { return _ViewB; }
        }

        /// <summary>
        /// Gets whether both viewmodels ViewA or ViewB hold more than
        /// no line to compare (enabling comparison functions makes no sense if this is false).
        /// </summary>
        public bool IsDiffDataAvailable
        {
            get
            {
                return _ViewA.LineCount > 0 || _ViewB.LineCount > 0;
            }
        }

        #region Caret Position
        public int SynchronizedColumn
        {
            get
            {
                return _SynchronizedColumn;
            }

            set
            {
                if (_SynchronizedColumn != value)
                {
                    _SynchronizedColumn = value;
                    NotifyPropertyChanged(() => SynchronizedColumn);
                }
            }
        }

        public int SynchronizedLine
        {
            get
            {
                return _SynchronizedLine;
            }

            set
            {
                if (_SynchronizedLine != value)
                {
                    _SynchronizedLine = value;
                    NotifyPropertyChanged(() => SynchronizedLine);
                }
            }
        }
        #endregion Caret Position

        /// <summary>
        /// Gets the similarity value (0% - 100%) between 2 things shown in toolbar
        /// </summary>
        public string Similarity_Text
        {
            get
            {
                return _Similarity_Text;
            }

            internal set
            {
                if (_Similarity_Text != value)
                {
                    _Similarity_Text = value;
                    NotifyPropertyChanged(() => Similarity_Text);
                }
            }
        }

        #region Left and Right File Name Labels
        /// <summary>
        /// Gets whether left and right file name labels over each ViewA and ViewB
        /// are visible or not.
        /// </summary>
        public bool edtLeft_Right_Visible
        {
            get
            {
                return _edtLeft_Right_Visible;
            }

            internal set
            {
                if (_edtLeft_Right_Visible != value)
                {
                    _edtLeft_Right_Visible = value;
                    NotifyPropertyChanged(() => edtLeft_Right_Visible);
                }
            }
        }

        /// <summary>
        /// Gets the left text label (file name) displayed over the left diff view (ViewA).
        /// </summary>
        public string edtLeft_Text
        {
            get
            {
                return _edtLeft_Text;
            }

            internal set
            {
                if (_edtLeft_Text != value)
                {
                    _edtLeft_Text = value;
                    NotifyPropertyChanged(() => edtLeft_Text);
                }
            }
        }

        /// <summary>
        /// Gets the right text label (file name) displayed over the right diff view (ViewA).
        /// </summary>
        public string edtRight_Text
        {
            get
            {
                return _edtRight_Text;
            }

            internal set
            {
                if (_edtRight_Text != value)
                {
                    _edtRight_Text = value;
                    NotifyPropertyChanged(() => edtRight_Text);
                }
            }
        }
        #endregion Left and Right File Name Labels

        public DiffSideViewModel ViewLineDiff
        {
            get { return _ViewLineDiff; }
        }

        internal DiffSideViewModel GetActiveView(out object nonActView)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets/sets the height of the bottom panel view that shows diff
        /// of the currently selected line with a 2 line view.
        /// </summary>
        public int LineDiffHeight
        {
            get
            {
                return _LineDiffHeight;
            }

            internal set
            {
                if (_LineDiffHeight != value)
                {
                    _LineDiffHeight = value;
                    NotifyPropertyChanged(() => LineDiffHeight);
                }
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Sets up the left and right diff viewmodels which contain line by line information
        /// with reference to textual contents and whether it should be handled as insertion,
        /// deletion, change, or no change when comparing left side (ViewA) with right side (ViewB).
        /// </summary>
        /// <param name="listA"></param>
        /// <param name="listB"></param>
        /// <param name="script"></param>
        /// <param name="nameA"></param>
        /// <param name="nameB"></param>
        /// <param name="changeDiffIgnoreCase"></param>
        /// <param name="changeDiffIgnoreWhiteSpace"></param>
        /// <param name="changeDiffTreatAsBinaryLines"></param>
        internal void SetData(IList<string> listA, IList<string> listB, EditScript script,
                              string nameA, string nameB,
                              bool changeDiffIgnoreCase,
                              bool changeDiffIgnoreWhiteSpace,
                              bool changeDiffTreatAsBinaryLines)
        {
            ChangeDiffOptions changeDiffOptions = ChangeDiffOptions.None;
            if (changeDiffTreatAsBinaryLines)
            {
                changeDiffOptions |= ChangeDiffOptions.IgnoreBinaryPrefix;
            }
            else
            {
                if (changeDiffIgnoreCase)
                {
                    changeDiffOptions |= ChangeDiffOptions.IgnoreCase;
                }

                if (changeDiffIgnoreWhiteSpace)
                {
                    changeDiffOptions |= ChangeDiffOptions.IgnoreWhitespace;
                }
            }

            _ViewA.ChangeDiffOptions = changeDiffOptions;
            _ViewB.ChangeDiffOptions = changeDiffOptions;
            _ViewLineDiff.ChangeDiffOptions = changeDiffOptions;

            _ViewA.SetData(nameA, listA, script, true);
            _ViewB.SetData(nameB, listB, script, false);
            NotifyPropertyChanged(() => this.IsDiffDataAvailable);

            Debug.Assert(this._ViewA.LineCount == this._ViewB.LineCount, "Both DiffView's LineCounts must be the same");

            // Sets the similarity value (0% - 100%) between 2 things shown in toolbar
            this.Similarity_Text = string.Format("{0:P}", script.Similarity);

            this._ViewA.SetCounterpartLines(this._ViewB);
////            this.Overview.DiffView = this.ViewA;

            // Show left and right file name labels over each ViewA and ViewB
            bool showNames = !string.IsNullOrEmpty(nameA) || !string.IsNullOrEmpty(nameB);
            this.edtLeft_Right_Visible = showNames;
////            this.edtRight.Visible = showNames;
            if (showNames)
            {
                this.edtLeft_Text = nameA;
                this.edtRight_Text = nameB;
            }

////            this.UpdateButtons();
            this.currentDiffLine = -1;
            this.UpdateLineDiff();

////            this.ActiveControl = this.ViewA;
        }

        /// <summary>
        /// Gets the view (of the 2 side by side views) that was activated last
        /// (had the focus the last time)
        /// </summary>
        /// <returns></returns>
        internal DiffSideViewModel GetActiveView(out DiffSideViewModel nonActiveView)
        {
            nonActiveView = null;

            if (ViewA == null && ViewB == null)
                return null;

            if (ViewA == null)
                return ViewB;

            if (ViewB == null)
                return ViewA;

            if (ViewA.ViewActivation < ViewB.ViewActivation)
            {
                nonActiveView = ViewA;
                return ViewB;
            }

            nonActiveView = ViewB;
            return ViewA;
        }

        /// <summary>
        /// Moves both views to the requested line position.
        /// </summary>
        /// <param name="gotoPos"></param>
        /// <param name="viewA"></param>
        /// <param name="viewB"></param>
        internal void ScrollToLine(DiffViewPosition gotoPos,
                                   DiffSideViewModel viewA,
                                   DiffSideViewModel viewB)
        {
            if (viewA != null)
            {
                viewA.ScrollToLine(gotoPos.Line + 1);
                viewA.SetPosition(gotoPos);
            }

            if (viewB != null)
            {
                viewB.ScrollToLine(gotoPos.Line + 1);
                viewB.SetPosition(gotoPos);
            }
        }

        private void UpdateLineDiff()
        {
            // Determine current cursor position in line n
            int line = Math.Max(0, SynchronizedLine - 1);
            if (line == this.currentDiffLine)
            {
                return;
            }

            this.currentDiffLine = line;

            DiffViewLine lineOne = null;
            DiffViewLine lineTwo = null;
            if (line < this.ViewA.LineCount)
            {
                lineOne = this.ViewA.GetLine(line);
            }

            // Normally, ViewA.LineCount == ViewB.LineCount, but during
            // SetData they'll be mismatched momentarily as each view
            // rebuilds its lines.
            if (line < this.ViewB.LineCount)
            {
                lineTwo = this.ViewB.GetLine(line);
            }

            if (lineOne != null && lineTwo != null)
            {
                this._ViewLineDiff.SetData(lineOne, lineTwo);
            }
        }

        /// <summary>
        /// Is invoked when the cursor position in view B has been changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnViewACaretPositionChanged(object sender, CaretPositionChangedEvent e)
        {
            switch (e.ChangeType)
            {
                case CaretChangeType.ColumnAndLine:
                    SynchronizedLine = e.Line;
                    SynchronizedColumn = e.Column;
                    break;
                case CaretChangeType.Column:
                    SynchronizedColumn = e.Column;
                    break;
                case CaretChangeType.Line:
                    SynchronizedLine = e.Line;
                    break;
                default:
                    throw new NotImplementedException(e.ChangeType.ToString());
            }

            if (e.ChangeType == CaretChangeType.Line || e.ChangeType == CaretChangeType.ColumnAndLine)
            {
                UpdateLineDiff();
                UpdateOtherView(e, ViewB);
            }
        }

        /// <summary>
        /// Is invoked when the cursor position in view A has been changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnViewBCaretPositionChanged(object sender, CaretPositionChangedEvent e)
        {
            switch (e.ChangeType)
            {
                case CaretChangeType.ColumnAndLine:
                    SynchronizedLine = e.Line;
                    SynchronizedColumn = e.Column;
                    break;
                case CaretChangeType.Column:
                    SynchronizedColumn = e.Column;
                    break;
                case CaretChangeType.Line:
                    SynchronizedLine = e.Line;
                    break;
                default:
                    throw new NotImplementedException(e.ChangeType.ToString());
            }

            if (e.ChangeType == CaretChangeType.Line || e.ChangeType == CaretChangeType.ColumnAndLine)
            {
                UpdateLineDiff();
                UpdateOtherView(e, ViewA);
            }
        }

        private void UpdateOtherView(CaretPositionChangedEvent e,
                                     DiffSideViewModel otherView)
        {
            if (otherView != null)
            {
                switch (e.ChangeType)
                {
                    case CaretChangeType.ColumnAndLine:
                        otherView.Line = e.Line;
                        otherView.Column = e.Column;
                        break;
                    case CaretChangeType.Column:
                        otherView.Column = e.Column;
                        break;
                    case CaretChangeType.Line:
                        otherView.Line = e.Line;
                        break;
                    default:
                        throw new NotImplementedException(e.ChangeType.ToString());
                }
            }
        }
        #endregion methods
    }
}