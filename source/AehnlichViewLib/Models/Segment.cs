namespace AehnlichLibViewModels.Models
{
    using ICSharpCode.AvalonEdit.Document;
    using System;
    using System.Diagnostics;

    [DebuggerDisplay("Start = {Start}, Length = {Length}")]
    public struct Segment : ISegment
    {
        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="endOffset"></param>
        public Segment(int start, int length, int endOffset)
            : this()
        {
            this.Offset = start;
            this.Length = length;
            this.EndOffset = endOffset;
        }

        #endregion

        #region Public Properties

        public int Length { get; }

        public int Offset { get; }

        public int EndOffset { get; }
        #endregion
    }
}
