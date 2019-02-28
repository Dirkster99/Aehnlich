namespace AehnlichViewLib.Controls.AvalonEditEx
{
    using ICSharpCode.AvalonEdit;
    using ICSharpCode.AvalonEdit.Rendering;
    using System.Windows.Media;
    using System.Windows;

    /// <summary>
    /// AvalonEdit: highlight current line even when not focused
    /// 
    /// Source: http://stackoverflow.com/questions/5072761/avalonedit-highlight-current-line-even-when-not-focused
    /// </summary>
    internal class HighlightCurrentLineBackgroundRenderer : IBackgroundRenderer
    {
        #region fields
        private readonly TextEditor _Editor;
        #endregion fields

        #region ctors
        /// <summary>
        /// Class Constructor from editor
        /// </summary>
        /// <param name="editor"></param>
        public HighlightCurrentLineBackgroundRenderer(TextEditor editor)
            : this()
        {
            this._Editor = editor;

            // Light Blue 0x100000FF
            this.BackgroundColorBrush = new SolidColorBrush(Color.FromArgb(0x10, 0x80, 0x80, 0x80));
        }

        /// <summary>
        /// Class Constructor from editor and SolidColorBrush definition
        /// </summary>
        /// <param name="editor"></param>
        /// <param name="highlightBackgroundColorBrush"></param>
        public HighlightCurrentLineBackgroundRenderer(TextEditor editor,
                                                      SolidColorBrush highlightBackgroundColorBrush)
            : this()
        {
            this._Editor = editor;

            // Light Blue 0x100000FF
            this.BackgroundColorBrush = new SolidColorBrush(highlightBackgroundColorBrush.Color);
        }

        /// <summary>
        /// Class Constructor from editor and color definition
        /// </summary>
        /// <param name="editor"></param>
        /// <param name="highlightBackgroundColorBrush"></param>
        public HighlightCurrentLineBackgroundRenderer(TextEditor editor,
                                                      Color highlightBackgroundColor)
            : this()
        {
            this._Editor = editor;

            // Light Blue 0x100000FF
            this.BackgroundColorBrush = new SolidColorBrush(highlightBackgroundColor);
        }

        /// <summary>
        /// Hidden class standard constructor
        /// </summary>
        protected HighlightCurrentLineBackgroundRenderer()
        {
            // Nothing to initialize here...
        }
        #endregion ctors

        #region properties
        /// <summary>
        /// Get the <seealso cref="KnownLayer"/> of the <seealso cref="TextEditor"/> control.
        /// </summary>
        public KnownLayer Layer
        {
            get { return KnownLayer.Background; }
        }

        /// <summary>
        /// Get/Set color brush to show for highlighting current line
        /// </summary>
        public SolidColorBrush BackgroundColorBrush { get; set; }
        #endregion properties

        #region methods
        /// <summary>
        /// Draw the background line highlighting of the current line.
        /// </summary>
        /// <param name="textView"></param>
        /// <param name="drawingContext"></param>
        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            if (this._Editor.Document == null)
                return;

            textView.EnsureVisualLines();
            var currentLine = _Editor.Document.GetLineByOffset(_Editor.CaretOffset);

            foreach (var rect in BackgroundGeometryBuilder.GetRectsForSegment(textView, currentLine))
            {
                drawingContext.DrawRectangle(new SolidColorBrush(this.BackgroundColorBrush.Color), null,
                                             new Rect(rect.Location, new Size(textView.ActualWidth, rect.Height)));
            }
        }
        #endregion methods
    }
}
