namespace AehnlichViewLib.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// Converts an object null or non-null value into a configurable
    /// value of type <seealso cref="bool"/>.
    /// </summary>
    [ValueConversion(typeof(object), typeof(bool))]
    public sealed class NullToEnabledPropConverter : IValueConverter
    {
        #region constructor
        /// <summary>
        /// Class constructor
        /// </summary>
        public NullToEnabledPropConverter()
        {
            // set defaults
            NullValue = false;
            NonNullValue = true;
        }
        #endregion constructor

        #region properties
        /// <summary>
        /// Gets/sets the <see cref="Visibility"/> value that is associated
        /// (converted into) with the boolean true value.
        /// </summary>
        public bool NullValue { get; set; }

        /// <summary>
        /// Gets/sets the <see cref="Visibility"/> value that is associated
        /// (converted into) with the boolean false value.
        /// </summary>
        public bool NonNullValue { get; set; }
        #endregion properties

        #region methods
        /// <summary>
        /// Convertzs a bool value into <see cref="Visibility"/> as configured in the
        /// <see cref="NullValue"/> and <see cref="NonNullValue"/> properties.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return NullValue;

            return NonNullValue;
        }

        /// <summary>
        /// Convertzs a <see cref="Visibility"/> value into bool as configured in the
        /// <see cref="NullValue"/> and <see cref="NonNullValue"/> properties.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Equals(value, NullValue))
                return true;

            if (Equals(value, NonNullValue))
                return false;

            return null;
        }
        #endregion methods
    }
}
