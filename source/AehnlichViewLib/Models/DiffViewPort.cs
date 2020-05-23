namespace AehnlichViewLib.Models
{
	/// <summary>
	/// Describes the currently available text view by giving its first and
	/// last line (number of lines visible in display) and the total numer
	/// of line in the bound text document.
	/// </summary>
	public class DiffViewPort
	{
		/// <summary>
		/// Class constructor
		/// </summary>
		/// <param name="firstLine"></param>
		/// <param name="lastLine"></param>
		/// <param name="lineCount"></param>
		/// <param name="caretLine"></param>
		/// <param name="caretColumn"></param>
		public DiffViewPort(int firstLine, int lastLine, int lineCount,
							int caretLine, int caretColumn)
		{
			this.FirstLine = firstLine;
			this.LastLine = lastLine;
			this.LineCount = lineCount;

			this.CaretLine = caretLine;
			this.CaretColumn = caretColumn;
		}

		/// <summary>
		/// Gets the line number of the first visible line in the text view.
		/// </summary>
		public int FirstLine { get; }

		/// <summary>
		/// Gets the line number of the last visible line in the text view.
		/// </summary>
		public int LastLine { get; }

		/// <summary>
		/// Gets the total number of all text lines in the text document.
		/// </summary>
		public int LineCount { get; }

		/// <summary>
		/// Gets the 1 based line number in which the cursor is currently positioned.
		/// </summary>
		public int CaretLine { get; }

		/// <summary>
		/// Gets the 1 based column number in which the cursor is currently positioned.
		/// </summary>
		public int CaretColumn { get; }
	}
}
