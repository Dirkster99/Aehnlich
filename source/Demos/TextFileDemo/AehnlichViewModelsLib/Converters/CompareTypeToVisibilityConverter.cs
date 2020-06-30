namespace AehnlichViewModelsLib.Converters
{
	using AehnlichLib.Enums;
	using System;
	using System.Globalization;
	using System.Windows;
	using System.Windows.Data;


	/// <summary>
	/// Converts a <see cref="CompareType"/> value into a specific bool value
	/// and all other values into the other bool value as configured in the properties
	/// of this class.
	/// </summary>
	[ValueConversion(typeof(CompareType), typeof(Visibility))]
	public class CompareTypeToVisibilityConverter : IValueConverter
	{
		/// <summary>
		/// Converts a <see cref="CompareType"/> value into a <see cref="bool"/> value
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <param name="parameter"></param>
		/// <param name="culture"></param>
		/// <returns></returns>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
				return Binding.DoNothing;

			if (value is CompareType)
			{
				CompareType val = (CompareType)value;
				if (val == CompareType.Binary)
					return Visibility.Collapsed;

				return Visibility.Visible;
			}

			return Binding.DoNothing;
		}

		/// <summary>
		/// Not Implemented.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <param name="parameter"></param>
		/// <param name="culture"></param>
		/// <returns></returns>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Binding.DoNothing;
		}
	}
}
