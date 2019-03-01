namespace AehnlichViewLib.Controls.AvalonEditEx
{
    // Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
    // 
    // Permission is hereby granted, free of charge, to any person obtaining a copy of this
    // software and associated documentation files (the "Software"), to deal in the Software
    // without restriction, including without limitation the rights to use, copy, modify, merge,
    // publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
    // to whom the Software is furnished to do so, subject to the following conditions:
    // 
    // The above copyright notice and this permission notice shall be included in all copies or
    // substantial portions of the Software.
    // 
    // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
    // INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
    // PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
    // FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
    // OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
    // DEALINGS IN THE SOFTWARE.

    using System.Collections.Generic;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using AehnlichViewLib.Interfaces;
    using ICSharpCode.AvalonEdit.Editing;
    using ICSharpCode.AvalonEdit.Rendering;
    using Utils;

    /// <summary>
    /// Margin showing line numbers.
    /// </summary>
    internal class CustomLineNumberMargin : LineNumberMargin, IWeakEventListener
    {
        #region fields
        private readonly DiffView _DiffView;
        #endregion fields

        #region ctors
        /// <summary>
        /// Static constructor
        /// </summary>
        static CustomLineNumberMargin()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomLineNumberMargin),
                                                     new FrameworkPropertyMetadata(typeof(CustomLineNumberMargin)));
        }

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="diffView"></param>
        public CustomLineNumberMargin(DiffView diffView)
            : this()
        {
            this._DiffView = diffView;
        }

        /// <summary>
        /// Hidden standard constructor
        /// </summary>
        protected CustomLineNumberMargin()
        {
        }
        #endregion ctors

        #region methods
        protected override void OnRender(DrawingContext drawingContext)
        {
            TextView textView = this.TextView;
            Size renderSize = this.RenderSize;

            if (_DiffView == null)
                return;

            var srcLineDiffs = _DiffView.ItemsSource as IReadOnlyList<IDiffLineInfo>;

            if (srcLineDiffs == null)
                return;

            if (textView != null && textView.VisualLinesValid)
            {
                var foreground = _DiffView.LineNumbersForeground;
                foreach (VisualLine line in textView.VisualLines)
                {
                    // AvalonEdit is 1 based but the collection is of course zero based :-(
                    int lineNumber = line.FirstDocumentLine.LineNumber - 1;

                    if (lineNumber >= srcLineDiffs.Count)
                        continue;

                    // Find a diff context for a given line
                    if (srcLineDiffs.Count < lineNumber)
                        continue;

                    var srcLineDiff = srcLineDiffs[lineNumber];

                    if (srcLineDiff.ImaginaryLineNumber.HasValue)
                    {
                        // Render displayed numbers in a 1-based format
                        int imaginaryLineNumber = (int)srcLineDiff.ImaginaryLineNumber + 1;
                        FormattedText text = TextFormatterFactory.CreateFormattedText(
                            this,
                            imaginaryLineNumber.ToString(CultureInfo.CurrentCulture),
                            typeface, emSize, foreground
                        );

                        double y = line.GetTextLineVisualYPosition(line.TextLines[0], VisualYPosition.TextTop);
                        drawingContext.DrawText(text, new Point(renderSize.Width - text.Width, y - textView.VerticalOffset));
                    }
                }
            }
        }
        #endregion methods
    }
}
