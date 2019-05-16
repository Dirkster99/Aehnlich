namespace AehnlichViewLib.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;

    internal class InvertColorConverter : IValueConverter
    {
        #region fields

        #endregion fields

        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        public InvertColorConverter()
        {
        }
        #endregion ctors

        #region methods
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Color)
            {
                return InvertColor((Color)value);
            }
            else
            {
                var brush = value as SolidColorBrush;
                if (brush != null)
                {
                    SolidColorBrush convertedBrush = new SolidColorBrush(InvertColor(brush.Color));
                    convertedBrush.Freeze();

                    return convertedBrush;
                }
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

        private Color InvertColor(Color ColourToInvert)
        {
            return Color.FromArgb(ColourToInvert.A,
                                 (byte)~ColourToInvert.R, (byte)~ColourToInvert.G, (byte)~ColourToInvert.B);
        }
        #endregion methods
    }
}
