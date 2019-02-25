namespace AehnlichViewLib
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;
    using AehnlichViewLib.Controls;
    using AehnlichViewLib.Enums;
    using ICSharpCode.AvalonEdit.Rendering;

    public class DiffLineBackgroundRenderer : IBackgroundRenderer
    {
        #region fields
        static Brush AddedBackground;
        static Brush BlankBackground;
        static Brush DeletedBackground;
        static Brush ChangedBackground;

        static readonly Pen BorderlessPen;
        private readonly DiffView _DiffView;
        #endregion fields

        static DiffLineBackgroundRenderer()
        {
////            AddedBackground = new SolidColorBrush(Color.FromArgb(0x40, 0x00, 0x7a, 0xcc));
////            AddedBackground.Freeze();
////
////            ChangedBackground = new SolidColorBrush(Color.FromArgb(0x40, 0x20, 0xFF, 0x20));
////            AddedBackground.Freeze();
////
////            DeletedBackground = new SolidColorBrush(Color.FromArgb(0x40, 0xFF, 0x20, 0x20));
////            DeletedBackground.Freeze();
////
////            BlankBackground = default(Brush);
////            BlankBackground.Freeze();

            var transparentBrush = new SolidColorBrush(Colors.Transparent);
            transparentBrush.Freeze();

            BorderlessPen = new Pen(transparentBrush, 0.0);
            BorderlessPen.Freeze();
        }

        public DiffLineBackgroundRenderer(DiffView diffView)
        {
            this._DiffView = diffView;

            AddedBackground = _DiffView.ColorBackgroundAdded;
            DeletedBackground = _DiffView.ColorBackgroundDeleted;
            ChangedBackground = _DiffView.ColorBackgroundContext;
            BlankBackground = _DiffView.ColorBackgroundBlank;
        }

        public KnownLayer Layer { get { return KnownLayer.Background; } }

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            if (_DiffView == null)
                return;

            var srcLineDiffs = _DiffView.ItemsSource as IReadOnlyList<DiffContext>;

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

                DiffContext context = srcLineDiffs[linenum];

                var brush = default(Brush);
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

                if (brush != default(Brush))
                {
                    foreach (var rc in BackgroundGeometryBuilder.GetRectsFromVisualSegment(textView, v, 0, 1000))
                    {
                        drawingContext.DrawRectangle(brush, BorderlessPen,
                            new Rect(0, rc.Top, textView.ActualWidth, rc.Height));
                    }
                }
            }
        }

        private Brush GetBrush4Color(Color thisColor)
        {
            if (thisColor == default(Color))
                return default(Brush);

            var brush = new SolidColorBrush(thisColor);
            brush.Freeze();

            return brush;
        }
    }
}
