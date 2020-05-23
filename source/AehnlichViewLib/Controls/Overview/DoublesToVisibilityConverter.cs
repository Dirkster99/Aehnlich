namespace AehnlichViewLib.Converters
{
	using System;
	using System.Globalization;
	using System.Windows;
	using System.Windows.Data;

	/// <summary>
	/// Implements a converter that controls whether a thumb is visible based on the bound
	/// - (one based) ViewPortSize and the
	/// - (zero based) DocumentLength.
	/// 
	/// A thumb should be shown if we have: ViewPortSize > (DocumentLength + 1)
	/// and be hidden in all other cases.
	/// </summary>
	[ValueConversion(typeof(double), typeof(Visibility))]
	public class DoublesToVisibilityConverter : DependencyObject, IMultiValueConverter
	{
		/// <summary>
		/// Converts the multibound properties ViewPortSize and DocumentLength into a
		/// <see cref="Visibility"/> value.
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

			if (values.Length != 2)
				return Binding.DoNothing;

			if ((values[0] is double && values[1] is double) == false)
				return Visibility.Collapsed;

			double firstValue = (double)values[0];
			double secondValue = (double)values[1];

			if (((int)secondValue) > (int)(firstValue + 1))
				return Visibility.Visible;

			// Entries appear to be equal
			return Visibility.Collapsed;
		}

		/// <summary>
		/// Not Implemented.
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
