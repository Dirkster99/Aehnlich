namespace Aehnlich.Converters
{
	using System;
	using System.Globalization;
	using System.Windows.Data;

	/// <summary>
	/// Converts a string null or string empty value into a specific bool value
	/// and all other values into the other bool value as configured in the properties
	/// of this class.
	/// </summary>
	[ValueConversion(typeof(string), typeof(bool))]
	public sealed class StringEmptyToBoolPropConverter : IValueConverter
	{
		#region constructor
		/// <summary>
		/// Class constructor
		/// </summary>
		public StringEmptyToBoolPropConverter()
		{
			// set defaults
			NullValue = false;
			NotNullValue = true;
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
		public bool NotNullValue { get; set; }
		#endregion properties

		#region methods
		/// <summary>
		/// Converts a <see cref="string"/> value into a <see cref="bool"/> value
		/// as configured in the <see cref="NullValue"/> and <see cref="NotNullValue"/> properties.
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

			var str = value as string;
			if (string.IsNullOrEmpty(str))
				return NullValue;

			return NotNullValue;
		}

		/// <summary>
		/// Not Implemented
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
		#endregion methods
	}
}
