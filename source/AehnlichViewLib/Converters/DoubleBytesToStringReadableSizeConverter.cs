namespace AehnlichViewLib.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

	/// <summary>
	/// Converts a given byte value as double number into human readable string (eg.: "1,5 Gb").
	///	The actual value input is expected to be an integer but we use a double here to avoid
	/// overflow issues when analyzing large directories.
	/// </summary>
    [ValueConversion(typeof(double), typeof(string))]
    public class DoubleBytesToStringReadableSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double == false)
                return Binding.DoNothing;

            var param = (double)value;

            if (param < 1024)
                return string.Format("{0:f2} bytes", param);

            param = param / 1024;

            if (param < 1024)
                return string.Format("{0:f2} Kb", param);

            param = param / 1024;

            if (param < 1024)
                return string.Format("{0:f2} Mb", param);

            param = param / 1024;

            if (param < 1024)
                return string.Format("{0:f2} Gb", param);

            param = param / 1024;

            if (param < 1024)
                return string.Format("{0:f2} Tb", param);

            param = param / 1024;

            return string.Format("{0:f2} Pb", param);
        }

		/// <summary>
		/// Not implemented.
		/// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
