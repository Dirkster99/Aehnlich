namespace DiffViewLib.Controls
{
    using System;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using DiffViewLib.Enums;
    using System.Windows.Media.Imaging;
    using System.Windows.Media;

    /// <summary>
    ///
    /// </summary>
    [TemplatePart(Name = PART_ViewPortContainer, Type = typeof(Grid))]
    [TemplatePart(Name = PART_ImageViewport, Type = typeof(Image))]
    public class Overview : Slider
    {
        #region fields
        private const string PART_ViewPortContainer = "PART_ViewPortContainer";
        private const string PART_ImageViewport = "PART_ImageViewport";

        private Grid _PART_ViewPortContainer;
        private Image _PART_ImageViewport;
        private WriteableBitmap writeableBmp;

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable),
                typeof(Overview), new PropertyMetadata(new PropertyChangedCallback(ItemsSourceChanged)));
        #endregion fields

        private static void ItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Overview)d).ItemsSourceChanged(e.NewValue);
        }

        private void ItemsSourceChanged(object newValue)
        {
            IList<DiffContext> newList = newValue as IList<DiffContext>;

            if (newList != null)
            {
                System.Diagnostics.Debug.WriteLine("Overview items list changed {0}", newList.Count());
                CreateBitmap(newList);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Overview items list changed to null");
                this.Minimum = this.Maximum = 0;
            }
        }

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

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

            _PART_ViewPortContainer = GetTemplateChild(PART_ViewPortContainer) as Grid;
            _PART_ImageViewport = GetTemplateChild(PART_ImageViewport) as Image;

            CreateBitmap(ItemsSource as IList<DiffContext>);
        }
        #endregion //Base Class Overrides

        #region methods

        private void CreateBitmap(IEnumerable<DiffContext> newList)
        {
            if (_PART_ViewPortContainer == null || _PART_ImageViewport == null || newList == null)
            {
                this.Minimum = this.Maximum = 0;
                return;
            }

            if (_PART_ViewPortContainer.IsVisible == false)
                return;

            this.Minimum = 0;
            this.Maximum = newList.Count() - 1;

            int width = (int)this._PART_ViewPortContainer.ActualWidth;
			int height = (int)this._PART_ViewPortContainer.ActualHeight;
            var thickness = new Thickness(0);

            if (width <= 0 || height <= 0)
                return;

            // Init WriteableBitmap
            writeableBmp = BitmapFactory.New(width, height);

            _PART_ImageViewport.Source = writeableBmp;

            Color controlBackgroundColor = Colors.White;

            // Clear Bitmap here
            using (writeableBmp.GetBitmapContext())
            {
                // Clear the WriteableBitmap with control background color
                writeableBmp.Clear(controlBackgroundColor);
            }

            int numLines = newList.Count();

            if (numLines == 0)
                return;

            const float GutterWidth = 1.0F;

            // Make sure each line is at least 1 pixel high
            float lineHeight = (float)Math.Max(1.0, this.GetPixelLineHeightF(1, numLines, height));

            using (writeableBmp.GetBitmapContext())
            {
                int i = 0;
                foreach (var line in newList)
                {
                    if (line != DiffContext.Blank)
                    {
                        float y = this.GetPixelLineHeightF(i, numLines, height);
                        float fullFillWidth = width - (2 * GutterWidth);

                        var color = default(Color);
                        switch (line)
                        {
                            case DiffContext.Context:
                                color = Color.FromArgb(0x40, 0xFF, 0, 0);
                                break;

                            case DiffContext.Deleted:
                                color = Color.FromArgb(0xFF, 0xFF, 0, 0);
                                break;

                            case DiffContext.Added:
                                color = Color.FromArgb(0xFF, 0, 0xFF, 0);
                                break;

                            case DiffContext.Blank:
                            default:
                                break;
                        }

                        if (color != default(Color))
                        {
                            writeableBmp.FillRectangle((int)GutterWidth, (int)y,
                                                        (int)GutterWidth + (int)fullFillWidth,
                                                        (int)y + (int)lineHeight,
                                                        color);
                        }
                    }

                    i++;
                }
            }
        }

		private float GetPixelLineHeightF(int lines, int lineCount, int clientSize_Height)
		{
			float result = 0;

			if (lineCount > 0)
			{
				result = clientSize_Height * (lines / (float)lineCount);
			}

			return result;
		}
        #endregion methods
    }
}
