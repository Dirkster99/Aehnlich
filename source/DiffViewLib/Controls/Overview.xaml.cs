namespace DiffViewLib.Controls
{
    using System;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using DiffViewLib.Enums;

    /// <summary>
    ///
    ///
    /// </summary>
    [TemplatePart(Name = PART_SpectrumDisplay, Type = typeof(Rectangle))]
    public class Overview : Slider
    {
        private const string PART_SpectrumDisplay = "PART_SpectrumDisplay";

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable),
                typeof(Overview), new PropertyMetadata(new PropertyChangedCallback(ItemsSourceChanged)));

        private static void ItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Overview)d).ItemsSourceChanged(e.NewValue);
        }

        private void ItemsSourceChanged(object newValue)
        {
            IEnumerable<DiffContext> newList = newValue as IEnumerable<DiffContext>;

            System.Diagnostics.Debug.WriteLine("matrix: items list changed " + newList.Count());
            if (newList != null)
            {
                CreateSpectrum(newList);
                System.Diagnostics.Debug.WriteLine("got " + string.Join(",", newList.Count()));
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("got null");
            }
        }

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        #region Private Members

        private Rectangle _Part_SpectrumDisplay;
        private LinearGradientBrush _pickerBrush;

        #endregion //Private Members

        #region Constructors
        /// <summary>
        /// Class constructor
        /// </summary>
        static Overview()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(Overview), new FrameworkPropertyMetadata(typeof(Overview)));
        }
        #endregion Constructors

        #region Dependency Properties

////        public static readonly DependencyProperty SelectedColorProperty =
////            DependencyProperty.Register("SelectedColor", typeof(Color),
////                typeof(ColorSpectrumSlider), new PropertyMetadata(System.Windows.Media.Colors.Transparent));
////
////        public Color SelectedColor
////        {
////            get
////            {
////                return (Color)GetValue(SelectedColorProperty);
////            }
////            set
////            {
////                SetValue(SelectedColorProperty, value);
////            }
////        }

        #endregion //Dependency Properties

        #region Base Class Overrides

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _Part_SpectrumDisplay = (Rectangle)GetTemplateChild(PART_SpectrumDisplay);

            if (_Part_SpectrumDisplay == null)
                return;

            CreateSpectrum(ItemsSource as IEnumerable<DiffContext>);
            OnValueChanged(Double.NaN, Value);
        }

        protected override void OnValueChanged(double oldValue, double newValue)
        {
////            base.OnValueChanged(oldValue, newValue);
////
////            //            Color color = ColorUtilities.ConvertHsvToRgb(360 - newValue, 1, 1);
////            Color color = HsvColor.RGBFromHSV(new HsvColor(360 - newValue, 1, 1));
////            SelectedColor = color;
        }

        #endregion //Base Class Overrides

        #region methods

        private void CreateSpectrum(IEnumerable<DiffContext> newList)
        {
            if (_Part_SpectrumDisplay == null || newList == null)
                return;

            double width = this._Part_SpectrumDisplay.Width;
			double height = this._Part_SpectrumDisplay.Height;

            if (width > 0 && height > 0)
            {
/*** Original rendering code from Diff.Net -> DiffOverview.RenderImage()
                // Draw a bitmap in memory that we can render from
                this.image = new Bitmap(width, height);
                using (Graphics g = Graphics.FromImage(this.image))
                using (SolidBrush backBrush = new SolidBrush(this.BackColor))
                {
                    g.FillRectangle(backBrush, 0, 0, width, height);

                    const float GutterWidth = 2.0F;

                    // Make sure each line is at least 1 pixel high
                    float lineHeight = (float)Math.Max(1.0, this.GetPixelLineHeightF(1));
                    DiffViewLines lines = this.view.Lines;
                    int numLines = lines.Count;
                    for (int i = 0; i < numLines; i++)
                    {
                        DiffViewLine line = lines[i];
                        if (line.Edited)
                        {
                            backBrush.Color = DiffOptions.GetColorForEditType(line.EditType);
                            float y = this.GetPixelLineHeightF(i);
                            float fullFillWidth = width - (2 * GutterWidth);

                            switch (line.EditType)
                            {
                                case EditType.Change:

                                    // Draw all the way across
                                    g.FillRectangle(backBrush, GutterWidth, y, fullFillWidth, lineHeight);
                                    break;

                                case EditType.Delete:

                                    // Draw delete on the left and dead space on the right.
                                    g.FillRectangle(backBrush, GutterWidth, y, fullFillWidth / 2, lineHeight);
                                    using (Brush deadBrush = DiffOptions.TryCreateDeadSpaceBrush(backBrush.Color))
                                    {
                                        g.FillRectangle(deadBrush ?? backBrush, GutterWidth + (fullFillWidth / 2), y, fullFillWidth / 2, lineHeight);
                                    }

                                    break;

                                case EditType.Insert:

                                    // Draw dead space on the left and insert on the right.
                                    using (Brush deadBrush = DiffOptions.TryCreateDeadSpaceBrush(backBrush.Color))
                                    {
                                        g.FillRectangle(deadBrush ?? backBrush, GutterWidth, y, fullFillWidth / 2, lineHeight);
                                    }

                                    g.FillRectangle(backBrush, GutterWidth + (fullFillWidth / 2), y, fullFillWidth / 2, lineHeight);
                                    break;
                            }
                        }
                    }
                }
***/
            }

////            _pickerBrush = new LinearGradientBrush();
////            _pickerBrush.StartPoint = new Point(0.5, 0);
////            _pickerBrush.EndPoint = new Point(0.5, 1);
////            _pickerBrush.ColorInterpolationMode = ColorInterpolationMode.SRgbLinearInterpolation;
////
////            List<Color> colorsList = ColorUtilities.GenerateHsvSpectrum();
////
////            double stopIncrement = (double)1 / colorsList.Count;
////
////            int i;
////            for (i = 0; i < colorsList.Count; i++)
////            {
////                _pickerBrush.GradientStops.Add(new GradientStop(colorsList[i], i * stopIncrement));
////            }
////
////            _pickerBrush.GradientStops[i - 1].Offset = 1.0;
////            _Part_SpectrumDisplay.Fill = _pickerBrush;
        }
        #endregion methods
    }
}
