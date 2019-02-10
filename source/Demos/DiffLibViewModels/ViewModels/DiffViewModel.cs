namespace DiffLibViewModels.ViewModels
{
    using DiffLib.Text;
    using DiffLibViewModels.Enums;
    using ICSharpCode.AvalonEdit.Document;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    public class DiffViewModel : Base.ViewModelBase
    {
        #region fields
        private ChangeDiffOptions _ChangeDiffOptions;
        private DiffViewLines lines;
        #endregion fields

        #region ctors
        public DiffViewModel()
        {
        }
        #endregion ctors

        #region properties
        private TextDocument _document = null;
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

        private bool _isDirty = false;
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

        [Browsable(false)]
        public int LineCount => this.lines != null ? this.lines.Count : 0;
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
            this.lines = new DiffViewLines(stringList, script, useA);
            Document = new TextDocument(GetDocumentFromRawLines());

            this.UpdateAfterSetData();
        }

        private string GetDocumentFromRawLines()
        {
            string ret = string.Empty;

            foreach (var item in lines)
            {
                ret += item.Text + '\n';
            }

            return ret;
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
            this.lines = new DiffViewLines(lineOne, lineTwo);
            this.UpdateAfterSetData();
        }

        /// <summary>
        /// Sets the Counterpart property in each line property of each
        /// <see cref="DiffViewModel"/> to refer to each other. This information
        /// can be used for finding equivelant from left to right lines[] collection
        /// and vice versa.
        /// </summary>
        /// <param name="counterpartView"></param>
        public void SetCounterpartLines(DiffViewModel counterpartView)
        {
            int numLines = this.LineCount;
            if (numLines != counterpartView.LineCount)
            {
                throw new ArgumentException("The counterpart view has a different number of view lines.", nameof(counterpartView));
            }

            for (int i = 0; i < numLines; i++)
            {
                DiffViewLine line = this.lines[i];
                DiffViewLine counterpart = counterpartView.lines[i];

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
        #endregion methods
    }
}