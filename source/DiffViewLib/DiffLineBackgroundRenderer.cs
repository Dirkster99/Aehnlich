namespace DiffViewLib
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;
    using DiffViewLib.Enums;
    using ICSharpCode.AvalonEdit.Rendering;

    public class DiffLineBackgroundRenderer : IBackgroundRenderer
    {
        static readonly Brush AddedBackground;
        static readonly Brush DeletedBackground;
        static readonly Brush BlankBackground;

        static readonly Pen BorderlessPen;

        static DiffLineBackgroundRenderer()
        {
            AddedBackground = new SolidColorBrush(Color.FromRgb(0xdd, 0xff, 0xdd));
            AddedBackground.Freeze();

            DeletedBackground = new SolidColorBrush(Color.FromRgb(0xff, 0xdd, 0xdd));
            DeletedBackground.Freeze();

            BlankBackground = new SolidColorBrush(Color.FromRgb(0xfa, 0xfa, 0xfa));
            BlankBackground.Freeze();

            var transparentBrush = new SolidColorBrush(Colors.Transparent);
            transparentBrush.Freeze();

            BorderlessPen = new Pen(transparentBrush, 0.0);
            BorderlessPen.Freeze();
        }

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            if (Lines == null) return;

            foreach (var v in textView.VisualLines)
            {
                var linenum = v.FirstDocumentLine.LineNumber - 1;
                if (linenum >= Lines.Count) continue;

                DiffContext context;
                if (Lines.TryGetValue(linenum, out context) == false)
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

                    case DiffContext.Blank:
                        brush = BlankBackground;
                        break;
                }

                foreach (var rc in BackgroundGeometryBuilder.GetRectsFromVisualSegment(textView, v, 0, 1000))
                {
                    drawingContext.DrawRectangle(brush, BorderlessPen,
                        new Rect(0, rc.Top, textView.ActualWidth, rc.Height));
                }
            }
        }

        public KnownLayer Layer { get { return KnownLayer.Background; } }
        public Dictionary<int, DiffContext> Lines { get; set; }
    }
}
