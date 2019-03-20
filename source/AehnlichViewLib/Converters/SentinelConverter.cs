namespace AehnlichViewLib.Converters
{
    using System;
    using System.Windows.Data;

    /// <summary>
	/// This converter ensures that new lines (with editing in DataGrid) are not bound
	/// to the viewmodel.
	///
    /// https://stackoverflow.com/questions/9109103/wpf-datagrid-selecteditem
    /// </summary>
    public sealed class SentinelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null && string.Equals("{NewItemPlaceholder}", value.ToString(), StringComparison.Ordinal))
            {
                return null;
            }

            return value;
        }
    }
}