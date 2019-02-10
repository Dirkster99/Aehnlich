namespace DiffViewLib
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;
    using DiffViewLib.Enums;
    using ICSharpCode.AvalonEdit.Rendering;

    public class DiffLineBackgroundRenderer2 : IBackgroundRenderer
    {
        #region fields
        static readonly Brush AddedBackground;
        static readonly Brush DeletedBackground;
        static readonly Brush BlankBackground;

        static readonly Pen BorderlessPen;
        private Dictionary<int, DiffContext> _LineDiffs;
        #endregion fields

        static DiffLineBackgroundRenderer2()
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

        public DiffLineBackgroundRenderer2(Dictionary<int, DiffContext> lineDiffs)
        {
            this._LineDiffs = lineDiffs;
        }

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            if (_LineDiffs == null)
                return;

            foreach (var v in textView.VisualLines)
            {
                var linenum = v.FirstDocumentLine.LineNumber - 1;
                if (linenum >= _LineDiffs.Count)
                    continue;

                // Find a diff context for a given line
                DiffContext context;
                if (_LineDiffs.TryGetValue(linenum, out context) == false)
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

        internal void ResetListValues(Dictionary<int, DiffContext> lineDiffs)
        {
            _LineDiffs = lineDiffs;
        }
    }
}
