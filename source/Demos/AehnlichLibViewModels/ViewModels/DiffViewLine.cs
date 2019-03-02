namespace AehnlichLibViewModels.ViewModels
{
    using AehnlichLib.Binaries;
    using AehnlichLib.Enums;
    using AehnlichLib.Text;
    using AehnlichLibViewModels.Enums;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

    [DebuggerDisplay("Number = {Number}, EditType = {EditType}, Text = {Text}, FromA = {FromA}")]
    internal sealed class DiffViewLine
    {
        #region Public Fields

        public static readonly DiffViewLine Empty = new DiffViewLine();

        #endregion

        #region Private Data Members
        /// <summary>
        /// Cache the edit script between this line and its counterpart in order to ensure
        /// optimal performance when displaying line diff information.
        /// </summary>
////        private EditScript _changeEditScript;
        private readonly int? number;
        private readonly string text;
        private readonly EditType _editType;

        #endregion

        #region Constructors
        /// <summary>
        /// Class constructor.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="number"></param>
        /// <param name="editType"></param>
        /// <param name="useA">Set to true if this data represents the reference view
        /// (left view also known as ViewA) otherwise false.</param>
        public DiffViewLine(string text, int? number, EditType editType, bool fromA)
        {
            this.text = text;
            this.number = number;
            this._editType = editType;
            this.FromA = fromA;
        }

        /// <summary>
        /// Hidden standard class constructor.
        /// </summary>
        private DiffViewLine()
        {
            this.text = string.Empty;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the equivalent line from the left view to the right view
        /// and vice versa.
        /// </summary>
        /// <param name="counterpartView"></param>
        public DiffViewLine Counterpart { get; internal set; }

        /// <summary>
        /// Gets whether the equivalent line of this line is different
        /// (requires an edit operation - delete, insert, change
        /// - to match both compared texts) or not.
        /// </summary>
        [SuppressMessage("", "SA1101", Justification = "The EditType reference is to the type not to this.EditType.")]
        public bool Edited => this._editType != EditType.None;

        /// <summary>
        /// Gets the type of edit operation (delete, insert, change, none)
        /// to signal how this line compares to its equivalent line linked
        /// in the <see cref="Counterpart"/> property.
        /// </summary>
        public EditType EditType => this._editType;

        /// <summary>
        /// Gets whether this line represents the reference view
        /// (left view also known as ViewA), otherwise false.</param>
        /// </summary>
        public bool FromA { get; }

        /// <summary>
        /// Get the line number that should be displayed for this line of text.
        /// 
        /// This line number is not the real line number i = 1...n from the original text
        /// but a line number that accomodates for imaginary lines that are inserted to
        /// sync the left and right view in a comparison.
        /// 
        /// Therefore, not all lines have an imaginary line number (this property is nullable).
        /// </summary>
        public int? Number => this.number;

        /// <summary>
        /// Gets the original text that was used when comparing this line to its
        /// <see cref="Counterpart"/> line.
        /// </summary>
        public string Text => this.text;
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets th edit script of this line in comparison to its <see cref="Counterpart"/>.
        /// The change edit script can then be used to color each letter position in the line
        /// to indicate how one line can completely match the other using character based
        /// change operations (insert, delete, change, none).
        /// 
        /// The method should only be invoked on demand (when a line is actually
        /// displayed) - we should wait with pulling it until we have to have it for rendering.
        /// 
        /// This object will NOT cache the edit script so the caller should implement an external
        /// caching algorithm to avoid multiple computations of the same answer.
        /// 
        /// Getting all intra-line diffs at once makes the whole process into an O(n^2) operation
        /// instead of just an O(n) operation for line-by-line diffs.  So we try to defer the
        /// extra work until the user requests to see the changed line.  It's still
        /// the same amount of work if the user views every line, but it makes the
        /// user interface more responsive to split it up like this.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public EditScript GetChangeEditScript(ChangeDiffOptions options)
        {
            EditScript _changeEditScript = null;

            if (//_changeEditScript == null &&
                _editType == EditType.Change && this.Counterpart != null)
            {
                if (this.FromA)
                {
                    int trimCountA, trimCountB;
                    MyersDiff<char> diff = new MyersDiff<char>(
                        GetCharactersToDiff(this.text, options, out trimCountA),
                        GetCharactersToDiff(this.Counterpart.text, options, out trimCountB),
                        false); // We don't want Change edits; just Deletes and Inserts.
                    
                    _changeEditScript = diff.Execute();

                    // If we trimmed/ignored leading whitespace, we have to offset each Edit to account for that.
                    foreach (Edit edit in _changeEditScript)
                    {
                        edit.Offset(trimCountA, trimCountB);
                    }
                }
                else if (this.Counterpart.FromA && this.Counterpart.Counterpart == this)
                {
                    // Defer to the A line because its edit script changes A into B.
                    _changeEditScript = this.Counterpart.GetChangeEditScript(options);
                }
            }

            return _changeEditScript;
        }

        #endregion

        #region Private Methods

        private static CharList GetCharactersToDiff(string lineText, ChangeDiffOptions options, out int leadingTrimCount)
        {
            leadingTrimCount = 0;

            // Check binary prefix first because the prefix length is a fixed number of characters.
            if (options.HasFlag(ChangeDiffOptions.IgnoreBinaryPrefix) && lineText.Length >= BinaryDiffLines.PrefixLength)
            {
                lineText = lineText.Substring(BinaryDiffLines.PrefixLength);
                leadingTrimCount += BinaryDiffLines.PrefixLength;
            }

            // Check whitespace next because this will shorten the string.
            if (options.HasFlag(ChangeDiffOptions.IgnoreWhitespace))
            {
                string trimmedLine = lineText.Trim();
                if (string.IsNullOrEmpty(trimmedLine))
                {
                    leadingTrimCount += lineText.Length;
                }
                else
                {
                    leadingTrimCount += lineText.IndexOf(trimmedLine[0]);
                }

                lineText = trimmedLine;
            }

            // Check case last because the line is now as short as it's going to get.
            if (options.HasFlag(ChangeDiffOptions.IgnoreCase))
            {
                lineText = lineText.ToUpper();
            }

            // Use CharList so we don't have to make separate char[] for the string.
            return new CharList(lineText);
        }

        #endregion

        #region Private Types

        private sealed class CharList : IList<char>
        {
            #region Private Data Members

            private readonly string text;

            #endregion

            #region Constructors

            public CharList(string text)
            {
                this.text = text ?? string.Empty;
            }

            #endregion

            #region Public Properties

            public int Count => this.text.Length;

            public bool IsReadOnly => true;

            public char this[int index]
            {
                get
                {
                    return this.text[index];
                }

                set
                {
                    throw new NotSupportedException();
                }
            }

            #endregion

            #region Public Methods

            public void Add(char item)
            {
                throw new NotSupportedException();
            }

            public void Clear()
            {
                throw new NotSupportedException();
            }

            public bool Contains(char item) => this.text.IndexOf(item) >= 0;

            public void CopyTo(char[] array, int arrayIndex)
            {
                this.text.CopyTo(0, array, arrayIndex, this.text.Length);
            }

            public IEnumerator<char> GetEnumerator() => this.text.GetEnumerator();

            public int IndexOf(char item) => this.text.IndexOf(item);

            public void Insert(int index, char item)
            {
                throw new NotSupportedException();
            }

            public bool Remove(char item)
            {
                throw new NotSupportedException();
            }

            public void RemoveAt(int index)
            {
                throw new NotSupportedException();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => this.GetEnumerator();

            #endregion
        }

        #endregion
    }
}