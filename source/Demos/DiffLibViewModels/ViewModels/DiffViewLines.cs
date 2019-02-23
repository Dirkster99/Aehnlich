namespace DiffLibViewModels.ViewModels
{
    using DiffLib.Enums;
    using DiffLib.Text;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    internal sealed class DiffViewLines : ReadOnlyCollection<DiffViewLine>
    {
        #region Private Data Members

        private readonly int[] diffEndLines;
        private readonly int[] diffStartLines;
        private int maxLineNumber = 1;

        #endregion

        #region Constructors

        public DiffViewLines(IList<string> stringList, EditScript script, bool useA)
            : this()
        {
            int currentLine = 0;

            int totalEdits = script.Count;
            this.diffStartLines = new int[totalEdits];
            this.diffEndLines = new int[totalEdits];

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
                currentLine += this.AddUneditedLines(stringList, currentLine, startingLine, useA);

                // Record where the next diff starts and ends
                this.diffStartLines[e] = this.Count;
                this.diffEndLines[e] = this.Count + edit.Length - 1;

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

                    this.AddLine(text, number, edit.EditType, useA);
                }
            }

            // Put in any remaining unedited lines
            this.AddUneditedLines(stringList, currentLine, stringList.Count, useA);
        }

        public DiffViewLines(DiffViewLine lineOne, DiffViewLine lineTwo)
            : this()
        {
            this.AddLine(lineOne);
            this.AddLine(lineTwo);

            this.diffStartLines = new int[0];
            this.diffEndLines = new int[0];
        }

        private DiffViewLines()
            : base(new List<DiffViewLine>())
        {
            // Called by the other constructors.
        }

        #endregion

        #region Public Properties

        public int[] DiffEndLines => this.diffEndLines;

        public int[] DiffStartLines => this.diffStartLines;

        public int MaxLineNumber => this.maxLineNumber;

        #endregion

        #region Private Methods

        private void AddLine(string text, int? number, EditType editType, bool fromA)
        {
            this.AddLine(new DiffViewLine(text, number, editType, fromA));
        }

        private void AddLine(DiffViewLine line)
        {
            if (line.Number.HasValue && line.Number.Value > this.maxLineNumber)
            {
                this.maxLineNumber = line.Number.Value;
            }

            this.Items.Add(line);
        }

        private int AddUneditedLines(IList<string> stringList, int current, int end, bool fromA)
        {
            for (int i = current; i < end; i++)
            {
                this.AddLine(stringList[i], i, EditType.None, fromA);
            }

            // If the edit script isn't using changes (i.e., just inserts and deletes)
            // then we can hit cases where iEnd < iCurrent, but we don't want to
            // return a negative value for number of added lines.
            int result = Math.Max(end - current, 0);
            return result;
        }

        #endregion
    }
}