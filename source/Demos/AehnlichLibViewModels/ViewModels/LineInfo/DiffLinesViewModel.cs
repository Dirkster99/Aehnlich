namespace AehnlichLibViewModels.ViewModels.LineInfo
{
    using AehnlichLib.Text;
    using AehnlichLibViewModels.Enums;
    using AehnlichViewLib.Enums;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Text;

    public class DiffLinesViewModel : Base.ViewModelBase
    {
        #region fields
        private readonly ObservableRangeCollection<DiffLineViewModel> _DocLineDiffs;

        private int[] _diffEndLines = null;
        private int[] _diffStartLines = null;

        /// <summary>
        /// Maximum imaginary line number which incorporates not only real text lines
        /// but also imaginary line that where inserted on either side of the comparison
        /// view to sync both sides into a consistent display.
        /// </summary>
        private int _maxImaginaryLineNumber = 1;
        #endregion fields

        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        public DiffLinesViewModel()
        {
            _DocLineDiffs = new ObservableRangeCollection<DiffLineViewModel>();
        }
        #endregion ctors

        #region properties
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
        #endregion properties

        #region methods
        public string SetData(IList<string> stringList, EditScript script, bool useA)
        {
            _diffEndLines = null;
            _diffStartLines = null;
            _maxImaginaryLineNumber = 1;

            var lines = new DiffViewLines(stringList, script, useA);

            _diffStartLines = lines.DiffStartLines;
            _diffEndLines = lines.DiffEndLines;
            _maxImaginaryLineNumber = lines.MaxLineNumber;

            IList<DiffLineViewModel> lineDiffs;
            string text = GetDocumentFromRawLines(lines, out lineDiffs);

            _DocLineDiffs.Clear();
            _DocLineDiffs.AddRange(lineDiffs, NotifyCollectionChangedAction.Reset);

            return text;
        }

        /// <summary>
        /// Used to setup the ViewLineDiff view that shows only 2 lines over each other
        /// representing the currently active line from the left/right side views under
        /// each other.
        /// </summary>
        /// <param name="lineOne"></param>
        /// <param name="lineTwo"></param>
        internal string SetData(DiffLineViewModel lineOneVM,
                                DiffLineViewModel lineTwoVM,
                                int spacesPerTab, ChangeDiffOptions changeDiffOptions)
        {
            var documentLineDiffs = new List<DiffLineViewModel>();

            string text = string.Empty;

            if (lineOneVM != null && lineOneVM.LineEditScriptSegmentsIsDirty == true)
                lineOneVM.GetChangeEditScript(changeDiffOptions, spacesPerTab);

            if (lineTwoVM != null && lineTwoVM.LineEditScriptSegmentsIsDirty == true)
                lineOneVM.GetChangeEditScript(changeDiffOptions, spacesPerTab);

            if (lineOneVM != null && lineTwoVM != null)
            {
                documentLineDiffs.Add(lineOneVM);
                text += lineOneVM.Text + '\n';

                documentLineDiffs.Add(lineTwoVM);
                text += lineOneVM.Text + "\n";
            }

            text = text.Replace("\t", "    ");

            // Update LineInfo viewmodels
            _DocLineDiffs.Clear();
            _DocLineDiffs.AddRange(documentLineDiffs, NotifyCollectionChangedAction.Reset);
            NotifyPropertyChanged(() => DocLineDiffs);

            return text;
        }


        /// <summary>
        /// Sets the Counterpart property in each line property of each
        /// <see cref="DiffLinesViewModel"/> to refer to each other. This information
        /// can be used for finding equivelant from left to right lines[] collection
        /// and vice versa.
        /// </summary>
        /// <param name="counterpartView"></param>
        public void SetCounterPartLines(DiffLinesViewModel counterpartView)
        {
            int numLines = _DocLineDiffs.Count;
            if (numLines != counterpartView._DocLineDiffs.Count)
            {
                throw new ArgumentException("The counterpart view has a different number of view lines.", nameof(counterpartView));
            }

            for (int i = 0; i < numLines; i++)
            {
                var line = _DocLineDiffs[i];
                var counterpart = counterpartView._DocLineDiffs[i];

                // Make the counterpart lines refer to each other.
                line.Counterpart = counterpart;
                counterpart.Counterpart = line;
            }
        }

        private string GetDocumentFromRawLines(DiffViewLines lines,
                                               out IList<DiffLineViewModel> documentLineDiffs)
        {
            documentLineDiffs = new List<DiffLineViewModel>();
            StringBuilder ret = new StringBuilder();

            foreach (var item in lines)
            {
                documentLineDiffs.Add(TranslateLineContext(item));
                ret.Append(item.Text + '\n');
            }

            return ret.ToString().Replace("\t", "    ");
        }

        private DiffLineViewModel TranslateLineContext(DiffViewLine item)
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

            return new DiffLineViewModel(lineContext, item);
        }
        #endregion methods
    }
}
