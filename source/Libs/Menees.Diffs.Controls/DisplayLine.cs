namespace Menees.Diffs.Controls
{
    using DiffLib.Enums;
    using DiffLib.Text;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Text;

	[DebuggerDisplay("DisplayText = {GetDisplayText()}")]
	internal struct DisplayLine
	{
		#region Private Data Members

		private DiffViewLine line;
		private bool showWhitespace;
		private int spacesPerTab;

		#endregion

		#region Constructors

		public DisplayLine(DiffViewLine line, bool showWhitespace, int spacesPerTab)
			: this()
		{
			this.line = line;
			this.showWhitespace = showWhitespace;
			this.spacesPerTab = spacesPerTab;
		}

		#endregion

		#region Public Properties

		public string OriginalText => this.line.Text;

		#endregion

		#region Public Methods

		public IList<Segment> GetChangeSegments(EditScript changeEditScript, bool useA)
		{
			List<Segment> result = new List<Segment>();

			int previousOriginalTextIndex = 0;
			int previousDisplayTextIndex = 0;

			foreach (Edit edit in changeEditScript)
			{
				if ((useA && edit.EditType == EditType.Delete) ||
					(!useA && edit.EditType == EditType.Insert))
				{
					int startOriginalTextIndex = useA ? edit.StartA : edit.StartB;
					int startDisplayTextIndex = this.GetDisplayTextIndex(startOriginalTextIndex, previousOriginalTextIndex, previousDisplayTextIndex);
					int endOriginalTextIndex = startOriginalTextIndex + edit.Length;
					int endDisplayTextIndex = this.GetDisplayTextIndex(endOriginalTextIndex, startOriginalTextIndex, startDisplayTextIndex);
					previousOriginalTextIndex = endOriginalTextIndex;
					previousDisplayTextIndex = endDisplayTextIndex;

					result.Add(new Segment(startDisplayTextIndex, endDisplayTextIndex - startDisplayTextIndex));
				}
			}

			return result;
		}

		public int GetColumnStart(int column)
		{
			// Convert to and from text index so the column's X position is always
			// shifted left to the start of the character that contains/crosses the column.
			// For example, if there are two leading tabs (of 4-spaces each), then a column
			// value of 6 should shift back to 4 because that's the column where the
			// overlapping tab character starts.
			int textIndex = this.GetTextIndexFromDisplayColumn(column);
			int result = this.GetDisplayColumnFromTextIndex(textIndex);
			return result;
		}

		public int GetColumnWidth(int column)
		{
			int textIndex = this.GetTextIndexFromDisplayColumn(column);

			// If they ask for a column past either end of the text, just tell them 1.
			int result = 1;
			if (textIndex >= 0 && textIndex < this.line.Text.Length)
			{
				int columnStart = this.GetDisplayColumnFromTextIndex(textIndex);
				result = this.GetCharWidth(this.line.Text[textIndex], columnStart);
			}

			return result;
		}

		public int GetDisplayColumnFromTextIndex(int index)
		{
			int result = 0;
			string originalText = this.OriginalText;
			int originalTextLength = originalText.Length;
			for (int i = 0; i < index && i < originalTextLength; i++)
			{
				char ch = originalText[i];
				result += this.GetCharWidth(ch, result);
			}

			return result;
		}

		public string GetDisplayText()
		{
			string originalText = this.OriginalText;
			StringBuilder sb = new StringBuilder(originalText.Length);
			foreach (char ch in originalText)
			{
				if (char.IsWhiteSpace(ch))
				{
					int charWidth = this.GetCharWidth(ch, sb.Length);
					char appendChar = ch;
					switch (ch)
					{
						case '\t':
							appendChar = this.showWhitespace ? '»' : ' ';
							break;

						case ' ':
							appendChar = this.showWhitespace ? '·' : ' ';
							break;

						case '\r':
							appendChar = '←';
							break;

						case '\n':
							appendChar = '↓';
							break;
					}

					sb.Append(appendChar);

					if (charWidth > 1)
					{
						// Subtract 1 because we've already appended a char.
						sb.Append(' ', charWidth - 1);
					}
				}
				else
				{
					sb.Append(ch);
				}
			}

			return sb.ToString();
		}

		public int GetDisplayTextLength()
		{
			int result = 0;
			foreach (char ch in this.OriginalText)
			{
				result += this.GetCharWidth(ch, result);
			}

			return result;
		}

		public string GetTextBetweenDisplayColumns(int startColumn, int afterEndColumn)
		{
			int startIndex = this.GetTextIndexFromDisplayColumn(startColumn);
			int afterEndIndex = this.GetTextIndexFromDisplayColumn(afterEndColumn);
			string result = this.line.Text.Substring(startIndex, afterEndIndex - startIndex);
			return result;
		}

		/// <summary>
		/// If column is past the end of the text, then Text.Length is returned.
		/// </summary>
		public int GetTextIndexFromDisplayColumn(int column)
		{
			int result = 0;
			if (column >= 0)
			{
				string originalText = this.OriginalText;
				int originalTextLength = originalText.Length;

				// Always set result, in case they pass a column that's past the end of the text.
				result = originalTextLength;

				int displayTextLength = 0;
				for (int i = 0; i < originalTextLength; i++)
				{
					char ch = originalText[i];
					displayTextLength += this.GetCharWidth(ch, displayTextLength);

					// Quit as soon as the display length is greater than the column because we've
					// already added the current char's width to the total.  A char that's N-spaces
					// wide may cross the column boundary.  For example, if column == 10 and there
					// are three leading tabs (of 4-spaces each), then the returned index should be
					// 2 because text[2] (i.e., the third char) is what contains/crosses column 10.
					if (displayTextLength > column)
					{
						result = i;
						break;
					}
				}
			}

			return result;
		}

		#endregion

		#region Private Methods

		private int GetCharWidth(char ch, int columnStart)
		{
			int result = 1;
			int position = columnStart + result;

			if (ch == '\t')
			{
				// We've already counted the tab as one character, but now we need to add on spaces
				// to expand to the next multiple of this.spacesPerTab.  The mod remainder tells us how
				// many spaces of the current tab that we've used so far.
				// Examples (with SpacesPerTab == 4):
				//  position = 5 --> mod == 1 --> need 3 spaces
				//  position = 16 --> mod == 0 --> need 0 spaces
				//  position = 18 --> mod == 2 --> need 2 spaces
				//  position = 23 --> mod == 1 --> need 1 space
				int tabSpacesUsed = position % this.spacesPerTab;
				if (tabSpacesUsed > 0)
				{
					int extraSpacesNeeded = this.spacesPerTab - tabSpacesUsed;
					result += extraSpacesNeeded;
				}
			}

			return result;
		}

		private int GetDisplayTextIndex(int originalTextIndex, int previousOriginalTextIndex, int previousDisplayTextIndex)
		{
			int result = previousDisplayTextIndex;

			string originalText = this.OriginalText;
			int maxLength = originalText.Length;
			for (int i = previousOriginalTextIndex; i < maxLength && i < originalTextIndex; i++)
			{
				result += this.GetCharWidth(originalText[i], result);
			}

			return result;
		}

		#endregion
	}
}
