namespace AehnlichViewLib.Controls
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;
    using AehnlichViewLib.Enums;
    using ICSharpCode.AvalonEdit.Rendering;
    using Interfaces;

    internal class DiffLineBackgroundRenderer : IBackgroundRenderer
    {
        #region fields
        static readonly Pen BorderlessPen;
        private readonly DiffView _DiffView;
        #endregion fields

        #region ctors
        /// <summary>
        /// Static class constructor
        /// </summary>
        static DiffLineBackgroundRenderer()
        {
            var transparentBrush = new SolidColorBrush(Colors.Transparent);
            transparentBrush.Freeze();

            BorderlessPen = new Pen(transparentBrush, 0.0);
            BorderlessPen.Freeze();
        }

        /// <summary>
        /// Parameterized class constructor
        /// </summary>
        /// <param name="diffView"></param>
        public DiffLineBackgroundRenderer(DiffView diffView)
            : this()
        {
            this._DiffView = diffView;
        }

        /// <summary>
        /// Hidden standard constructor
        /// </summary>
        protected DiffLineBackgroundRenderer()
        {
        }
        #endregion ctors

        #region properties
        public KnownLayer Layer { get { return KnownLayer.Background; } }
        #endregion properties

        #region methods
        /// <summary>
        /// Draws the background of line based on its <see cref="DiffContext"/>
        /// and whether it is imaginary or not.
        /// </summary>
        /// <param name="textView"></param>
        /// <param name="drawingContext"></param>
        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            if (_DiffView == null)
                return;

            var srcLineDiffs = _DiffView.ItemsSource as IReadOnlyList<IDiffLineInfo>;

            if (srcLineDiffs == null)
                return;

            foreach (var v in textView.VisualLines)
            {
                var linenum = v.FirstDocumentLine.LineNumber - 1;
                if (linenum >= srcLineDiffs.Count)
                    continue;

                // Find a diff context for a given line
                if (srcLineDiffs.Count <= linenum)
                    continue;

                var srcLineDiff = srcLineDiffs[linenum];
                DiffContext context = srcLineDiff.Context;

                var brush = default(SolidColorBrush);
                switch (context)
                {
                    case DiffContext.Added:
                        brush = _DiffView.ColorBackgroundAdded;
                        break;

                    case DiffContext.Deleted:
                        brush = _DiffView.ColorBackgroundDeleted;
                        break;

                    case DiffContext.Context:
                        brush = _DiffView.ColorBackgroundContext;
                        break;

                    case DiffContext.Blank:
                        brush = _DiffView.ColorBackgroundBlank;
                        break;
                    default:
                        throw new System.ArgumentException(context.ToString());
                }

                if (brush != default(SolidColorBrush))
                {
                    if (srcLineDiff.ImaginaryLineNumber.HasValue == false)
                    {
                        brush = new SolidColorBrush(Color.FromArgb(0x60,
                            brush.Color.R, brush.Color.G, brush.Color.B));
                    }

                    foreach (var rc in BackgroundGeometryBuilder.GetRectsFromVisualSegment(textView, v, 0, 1000))
                    {
                        drawingContext.DrawRectangle(brush, BorderlessPen,
                            new Rect(0, rc.Top, textView.ActualWidth, rc.Height));
                    }
                }
            }
        }
        #endregion methods
    }
}
