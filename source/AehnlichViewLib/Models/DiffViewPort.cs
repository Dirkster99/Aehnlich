namespace AehnlichViewLib.Models
{
    public class DiffViewPort
    {
        public DiffViewPort(int firstLine, int lastLine, int lineCount)
        {
            FirstLine = firstLine;
            LastLine = lastLine;
            LineCount = lineCount;
        }

        public int FirstLine { get; }
        public int LastLine { get; }

        /// <summary>
        /// Gets the total number of lines in the document
        /// </summary>
        public int LineCount { get; }
    }
}
