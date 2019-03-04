namespace AehnlichLibViewModels.ViewModels.LineInfo
{
    using AehnlichLib.Enums;
    using AehnlichLib.Text;
    using AehnlichLibViewModels.Interfaces;
    using AehnlichViewLib.Enums;
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Implements an object that constructs a line model and viewmodel out of
    /// two lists of strings and an edit script.
    /// </summary>
    internal class LinesFactory
    {
        #region fields
        private readonly DiffLines _linesA;
        private readonly DiffLines _linesB;

        private string _textA;
        private string _textB;
        #endregion fields

        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        public LinesFactory()
        {
            _linesA = new DiffLines();
            _linesB = new DiffLines();
        }
        #endregion ctors

        #region properties
        /// <summary>
        /// Gets the line differences for document A.
        /// </summary>
        internal IDiffLines LinesA { get { return _linesA; } }

        /// <summary>
        /// Gets the line differences for document B.
        /// </summary>
        internal IDiffLines LinesB { get { return _linesB; } }

        /// <summary>
        /// Gets the text for document A (including imaginary lines).
        /// </summary>
        internal string TextA { get { return _textA; } }

        /// <summary>
        /// Gets the text for document B (including imaginary lines).
        /// </summary>
        internal string TextB { get { return _textB; } }
        #endregion properties

        #region methods
        /// <summary>
        /// Inititializes the document A and B properties in this object
        /// from the given parameters.
        /// </summary>
        /// <param name="stringListA"></param>
        /// <param name="stringListB"></param>
        /// <param name="script"></param>
        public void SetData(IList<string> stringListA,
                            IList<string> stringListB,
                            EditScript script)
        {
            _textA = _linesA.SetData(stringListA, script, true);
            _textB = _linesB.SetData(stringListB, script, false);

            _linesA.SetCounterPartLines(_linesB);
        }
        #endregion methods

        #region private classes
        /// <summary>
        /// Implements a class that translates string lines into line objects
        /// that describe differences between compared lines.
        /// </summary>
        private class DiffLines : IDiffLines
        {
            #region fields
            private readonly List<DiffLineViewModel> _DocLineDiffs;

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
            public DiffLines()
            {
                _DocLineDiffs = new List<DiffLineViewModel>();
            }
            #endregion ctors

            #region properties
            public IReadOnlyCollection<DiffLineViewModel> DocLineDiffs
            {
                get
                {
                    return _DocLineDiffs;
                }
            }

            public int [] DiffEndLines { get { return _diffEndLines; }}
            public int[] DiffStartLines { get { return _diffStartLines; }}

            /// <summary>
            /// Maximum imaginary line number which incorporates not only real text lines
            /// but also imaginary line that where inserted on either side of the comparison
            /// view to sync both sides into a consistent display.
            /// </summary>
            public int MaxImaginaryLineNumber { get { return _maxImaginaryLineNumber; } }
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
                _DocLineDiffs.AddRange(lines);

                return text;
            }

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

            private string GetDocumentFromRawLines(IList<DiffLineViewModel> lines)
            {
                StringBuilder ret = new StringBuilder();

                foreach (var item in lines)
                    ret.Append(item.Text + '\n');

                return ret.ToString().Replace("\t", "    ");
            }


            /// <summary>
            /// Sets the Counterpart property in each line property of each
            /// <see cref="DiffLinesViewModel"/> to refer to each other. This information
            /// can be used for finding equivelant lines from left to right lines[] collection
            /// and vice versa.
            /// </summary>
            /// <param name="counterpartView"></param>
            public void SetCounterPartLines(DiffLines counterpartView)
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
                    line.SetCounterPart(counterpart);
                    counterpart.SetCounterPart(line);
                }
            }
            #endregion methods
        }
        #endregion private classes
    }
}
