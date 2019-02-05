namespace Menees.Diffs.Controls
{
    using DiffLib.BinaryFileDiff;
    using DiffLib.Enums;
    using DiffLib.Text;
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

		private EditScript changeEditScript;
		private int? number;
		private string text;
		private EditType editType;

		#endregion

		#region Constructors

		public DiffViewLine(string text, int? number, EditType editType, bool fromA)
		{
			this.text = text;
			this.number = number;
			this.editType = editType;
			this.FromA = fromA;
		}

		private DiffViewLine()
		{
			this.text = string.Empty;
		}

		#endregion

		#region Public Properties

		public DiffViewLine Counterpart { get; internal set; }

		[SuppressMessage("", "SA1101", Justification = "The EditType reference is to the type not to this.EditType.")]
		public bool Edited => this.editType != EditType.None;

		public EditType EditType => this.editType;

		public bool FromA { get; }

		public int? Number => this.number;

		public string Text => this.text;

		#endregion

		#region Public Methods

		public EditScript GetChangeEditScript(ChangeDiffOptions options)
		{
			if (this.changeEditScript == null && this.editType == EditType.Change && this.Counterpart != null)
			{
				if (this.FromA)
				{
					int trimCountA, trimCountB;
					MyersDiff<char> diff = new MyersDiff<char>(
						GetCharactersToDiff(this.text, options, out trimCountA),
						GetCharactersToDiff(this.Counterpart.text, options, out trimCountB),
						false); // We don't want Change edits; just Deletes and Inserts.
					this.changeEditScript = diff.Execute();

					// If we trimmed/ignored leading whitespace, we have to offset each Edit to account for that.
					foreach (Edit edit in this.changeEditScript)
					{
						edit.Offset(trimCountA, trimCountB);
					}
				}
				else if (this.Counterpart.FromA && this.Counterpart.Counterpart == this)
				{
					// Defer to the A line because its edit script changes A into B.
					this.changeEditScript = this.Counterpart.GetChangeEditScript(options);
				}
			}

			return this.changeEditScript;
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

			private string text;

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
