namespace AehnlichDirViewModelLib.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// Converts multiple bound objects into one array of <see cref="object"/>s and returns them.
    /// </summary>
    public class MultiParToParConverter : IMultiValueConverter
    {
        /// <summary>
        /// Converts multiple bound objects into one array of <see cref="object"/>s and returns them.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetTypes"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
