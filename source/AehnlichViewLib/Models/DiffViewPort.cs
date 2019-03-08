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
        public DiffViewPort(int firstLine, int lastLine, int lineCount)
        {
            FirstLine = firstLine;
            LastLine = lastLine;
            LineCount = lineCount;
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
    }
}
