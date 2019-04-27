namespace AehnlichViewLib.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// Inverts a boolean value and returns it.
    /// </summary>
    public class BooleanInvertConverter : IValueConverter
    {
        /// <summary>
        /// Inverts a boolean value and returns it.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value is bool) == false)
                return Binding.DoNothing;

            return !((bool)value);
        }

        /// <summary>
        /// Inverts a boolean value and returns it.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value is bool) == false)
                return Binding.DoNothing;

            return !((bool)value);
        }
    }
}
