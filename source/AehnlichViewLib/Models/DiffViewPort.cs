namespace AehnlichViewLib.Models
{
    public class DiffViewPort
    {
        public DiffViewPort(int firstLine, int lastLine)
        {
            FirstLine = firstLine;
            LastLine = lastLine;
        }

        public int FirstLine { get; }
        public int LastLine { get; }
    }
}
