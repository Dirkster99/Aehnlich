namespace DiffViewLib
{
    using System.Windows;
    using System.Windows.Media;
    using DiffViewLib.Controls;
    using DiffViewLib.Enums;
    using ICSharpCode.AvalonEdit.Rendering;

    public class DiffLineBackgroundRenderer : IBackgroundRenderer
    {
        #region fields
        static readonly Brush AddedBackground;
        static readonly Brush BlankBackground;
        static readonly Brush DeletedBackground;
        static readonly Brush ChangedBackground;

        static readonly Pen BorderlessPen;
        private readonly DiffView _DiffView;
        #endregion fields

        static DiffLineBackgroundRenderer()
        {
            AddedBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0xFF, 0x00));
            AddedBackground.Freeze();

            ChangedBackground = new SolidColorBrush(Color.FromArgb(0x40, 0xFF, 0x00, 0x00));
            AddedBackground.Freeze();

            DeletedBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0x00, 0x00));
            DeletedBackground.Freeze();

            BlankBackground = new SolidColorBrush(Color.FromRgb(0xfa, 0xfa, 0xfa));
            BlankBackground.Freeze();

            var transparentBrush = new SolidColorBrush(Colors.Transparent);
            transparentBrush.Freeze();

            BorderlessPen = new Pen(transparentBrush, 0.0);
            BorderlessPen.Freeze();
        }

        public DiffLineBackgroundRenderer(DiffView diffView)
        {
            this._DiffView = diffView;
        }

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            if (_DiffView == null)
                return;

            foreach (var v in textView.VisualLines)
            {
                var linenum = v.FirstDocumentLine.LineNumber - 1;
                if (linenum >= _DiffView.LineDiffs.Count)
                    continue;

                // Find a diff context for a given line
                DiffContext context;
                if (_DiffView.LineDiffs.TryGetValue(linenum, out context) == false)
                    continue;

                var brush = default(Brush);
                switch (context)
                {
                    case DiffContext.Added:
                        brush = AddedBackground;
                        break;

                    case DiffContext.Deleted:
                        brush = DeletedBackground;
                        break;

                    case DiffContext.Context:
                        brush = ChangedBackground;
                        break;

///                    case DiffContext.Blank:
///                        brush = BlankBackground;
///                        break;
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

        public KnownLayer Layer { get { return KnownLayer.Background; } }
    }
}
