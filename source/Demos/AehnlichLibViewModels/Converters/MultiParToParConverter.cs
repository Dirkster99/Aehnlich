namespace AehnlichLibViewModels.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class MultiParToParConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
                return Binding.DoNothing;

            if (values.Length < 1)
                return Binding.DoNothing;

            object[] ret = new object[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                ret[i] = values[i];
            }

            return ret;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
