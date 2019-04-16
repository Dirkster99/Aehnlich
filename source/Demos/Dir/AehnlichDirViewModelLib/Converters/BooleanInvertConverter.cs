namespace AehnlichDirViewModelLib.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class BooleanInvertConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value is bool) == false)
                return Binding.DoNothing;

            return !((bool)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value is bool) == false)
                return Binding.DoNothing;

            return !((bool)value);
        }
    }
}
