namespace AehnlichDirViewModelLib.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// Converts a boolean value into a configurable
    /// value of type <seealso cref="Visibility"/>.
    /// 
    /// Source: http://stackoverflow.com/questions/3128023/wpf-booleantovisibilityconverter-that-converts-to-hidden-instead-of-collapsed-wh
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public sealed class BoolToVisibilityPropConverter : IValueConverter
    {
        #region constructor
        /// <summary>
        /// Class constructor
        /// </summary>
        public BoolToVisibilityPropConverter()
        {
            // set defaults
            TrueValue = Visibility.Visible;
            FalseValue = Visibility.Collapsed;
        }
        #endregion constructor

        #region properties
        /// <summary>
        /// Gets/sets the <see cref="Visibility"/> value that is associated
        /// (converted into) with the boolean true value.
        /// </summary>
        public Visibility TrueValue { get; set; }

        /// <summary>
        /// Gets/sets the <see cref="Visibility"/> value that is associated
        /// (converted into) with the boolean false value.
        /// </summary>
        public Visibility FalseValue { get; set; }
        #endregion properties

        #region methods
        /// <summary>
        /// Convertzs a bool value into <see cref="Visibility"/> as configured in the
        /// <see cref="TrueValue"/> and <see cref="FalseValue"/> properties.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
                return null;

            return (bool)value ? TrueValue : FalseValue;
        }

        /// <summary>
        /// Convertzs a <see cref="Visibility"/> value into bool as configured in the
        /// <see cref="TrueValue"/> and <see cref="FalseValue"/> properties.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Equals(value, TrueValue))
                return true;

            if (Equals(value, FalseValue))
                return false;

            return null;
        }
        #endregion methods
    }
}
