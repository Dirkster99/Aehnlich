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
        private readonly DiffView _Editor;
        #endregion fields

        #region ctors
        /// <summary>
        /// Class Constructor from editor and SolidColorBrush definition
        /// </summary>
        /// <param name="editor"></param>
        public HighlightCurrentLineBackgroundRenderer(DiffView diffView)
            : this()
        {
            _Editor = diffView;
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
        #endregion properties

        #region methods
        /// <summary>
        /// Draw the background line highlighting of the current line.
        /// </summary>
        /// <param name="textView"></param>
        /// <param name="drawingContext"></param>
        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            if (this._Editor == null)
                return;

            if (this._Editor.Document == null)
                return;

            if (_Editor.EditorCurrentLineBorderThickness == 0 && _Editor.EditorCurrentLineBackground == null)
                return;

            Pen borderPen = null;

            if (_Editor.EditorCurrentLineBorder != null)
            {
                borderPen = new Pen(_Editor.EditorCurrentLineBorder, _Editor.EditorCurrentLineBorderThickness);
                borderPen.Freeze();
            }

            textView.EnsureVisualLines();
            var currentLine = _Editor.Document.GetLineByOffset(_Editor.CaretOffset);

            foreach (var rect in BackgroundGeometryBuilder.GetRectsForSegment(textView, currentLine))
            {
                drawingContext.DrawRectangle(_Editor.EditorCurrentLineBackground, borderPen,
                                             new Rect(rect.Location, new Size(textView.ActualWidth, rect.Height)));
            }
        }
        #endregion methods
    }
}
