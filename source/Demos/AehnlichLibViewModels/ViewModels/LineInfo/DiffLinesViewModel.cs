namespace AehnlichLibViewModels.ViewModels.LineInfo
{
    using AehnlichLib.Enums;
    using AehnlichLib.Text;
    using AehnlichLibViewModels.Enums;
    using AehnlichViewLib.Enums;
    using AehnlichViewLib.Models;
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
        public string SetData(IList<string> stringList, EditScript script,
                              bool useA)
        {
            _diffEndLines = null;
            _diffStartLines = null;
            _maxImaginaryLineNumber = 1;

            var lines = GetLineModels(stringList, script, useA);

            string text = GetDocumentFromRawLines(lines);

            _DocLineDiffs.Clear();
            _DocLineDiffs.AddRange(lines, NotifyCollectionChangedAction.Reset);

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

        #region model collection
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stringList"></param>
        /// <param name="script"></param>
        /// <param name="useA">Set to true if this data represents the reference view
        /// (left view also known as ViewA) otherwise false.</param>
        internal IList<DiffLineViewModel> GetLineModels(IList<string> stringList,
                                                        EditScript script, bool useA)
        {
            var ret = new List<DiffLineViewModel>();
            int currentLine = 0;

            int totalEdits = script.Count;
            _diffStartLines = new int[totalEdits];
            _diffEndLines = new int[totalEdits];

            for (int e = 0; e < totalEdits; e++)
            {
                Edit edit = script[e];

                // Get the starting line for this Edit
                int startingLine;
                bool dummyLine = false;
                if (useA)
                {
                    dummyLine = edit.EditType == EditType.Insert;
                    startingLine = edit.StartA;
                }
                else
                {
                    dummyLine = edit.EditType == EditType.Delete;
                    startingLine = edit.StartB;
                }

                // Put in unedited lines up to this point
                currentLine += this.AddUneditedLines(stringList, currentLine, startingLine, useA, ret);

                // Record where the next diff starts and ends
                _diffStartLines[e] = ret.Count;
                _diffEndLines[e] = ret.Count + edit.Length - 1;

                // Since Inserts occur after the current line
                // instead of at it, we have to decrement the index.
                for (int i = 0; i < edit.Length; i++)
                {
                    // A - Shows Deletes and Changes
                    // B - Shows Inserts and Changes
                    string text = string.Empty;
                    int? number = null;
                    if (!dummyLine)
                    {
                        number = startingLine + i;
                        text = stringList[number.Value];
                        currentLine++;
                    }

                    this.AddLine(text, number, edit.EditType, useA, ret);
                }
            }

            // Put in any remaining unedited lines with Edittype.None
            this.AddUneditedLines(stringList, currentLine, stringList.Count, useA, ret);

            return ret;
        }

        /// <summary>
        /// Adds all lines between <paramref name="current"/> and <paramref name="end"/>
        /// with <see cref="EditType.None"/> into the inherited Items.
        /// </summary>
        /// <param name="stringList">List of items to be added</param>
        /// <param name="current">index of first item to be added</param>
        /// <param name="end">index+1 of last item to be added</param>
        /// <param name="fromA">Set to true if this data represents the reference view
        /// (left view also known as ViewA) otherwise false.</param>
        /// <returns>the actual number of added lines</returns>
        private int AddUneditedLines(IList<string> stringList, int current, int end, bool fromA,
                                     List<DiffLineViewModel> items)
        {
            for (int i = current; i < end; i++)
            {
                this.AddLine(stringList[i], i, EditType.None, fromA, items);
            }

            // If the edit script isn't using changes (i.e., just inserts and deletes)
            // then we can hit cases where iEnd < iCurrent, but we don't want to
            // return a negative value for number of added lines.
            int result = Math.Max(end - current, 0);
            return result;
        }

        /// <summary>
        /// Constructs a <see cref="DiffViewLine"/> object and
        /// adds it into the Items collection.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="number"></param>
        /// <param name="editType"></param>
        /// <param name="fromA">Set to true if this data represents the reference view
        /// (left view also known as ViewA) otherwise false.</param>
        /// <returns></returns>
        private void AddLine(string text, int? number, EditType editType, bool fromA,
                             List<DiffLineViewModel> items)
        {
            this.AddLine(new DiffViewLine(text, number, editType, fromA), items);
        }

        /// <summary>
        /// Adds another line in into the inherited Items collection.
        /// </summary>
        /// <param name="line"></param>
        private void AddLine(DiffViewLine line, List<DiffLineViewModel> items)
        {
            if (line.Number.HasValue && line.Number.Value > _maxImaginaryLineNumber)
            {
                _maxImaginaryLineNumber = line.Number.Value;
            }

            items.Add(TranslateLineContext(line));
        }
        #endregion model collection

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

        private string GetDocumentFromRawLines(IList<DiffLineViewModel> lines)
        {
            StringBuilder ret = new StringBuilder();

            foreach (var item in lines)
                ret.Append(item.Text + '\n');

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
