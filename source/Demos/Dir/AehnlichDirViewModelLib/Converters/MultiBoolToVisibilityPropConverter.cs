namespace AehnlichDirViewModelLib.Converters
{
	using System;
	using System.Globalization;
	using System.Windows;
	using System.Windows.Data;

	/// <summary>
	/// Converts a boolean values into a configurable
	/// value of type <seealso cref="Visibility"/>.
	/// 
	/// Source: http://stackoverflow.com/questions/3128023/wpf-booleantovisibilityconverter-that-converts-to-hidden-instead-of-collapsed-wh
	/// </summary>
	[ValueConversion(typeof(bool), typeof(Visibility))]
	public sealed class MultiBoolToVisibilityPropConverter : IMultiValueConverter
	{
		#region constructor
		/// <summary>
		/// Class constructor
		/// </summary>
		public MultiBoolToVisibilityPropConverter()
		{
			// set defaults
			TrueTrueValue = Visibility.Collapsed;
			TrueFalseValue = Visibility.Visible;

			FalseFalseValue = Visibility.Collapsed;
			FalseTrueValue = Visibility.Collapsed;
		}
		#endregion constructor

		#region properties
		/// <summary>
		/// Gets/sets the <see cref="Visibility"/> value that is associated
		/// (converted into) with the boolean true value.
		/// </summary>
		public Visibility TrueTrueValue { get; set; }

		/// <summary>
		/// Gets/sets the <see cref="Visibility"/> value that is associated
		/// (converted into) with the boolean false value.
		/// </summary>
		public Visibility TrueFalseValue { get; set; }

		/// <summary>
		/// Gets/sets the <see cref="Visibility"/> value that is associated
		/// (converted into) with the boolean true value.
		/// </summary>
		public Visibility FalseFalseValue { get; set; }

		/// <summary>
		/// Gets/sets the <see cref="Visibility"/> value that is associated
		/// (converted into) with the boolean false value.
		/// </summary>
		public Visibility FalseTrueValue { get; set; }
		#endregion properties

		#region methods
		/// <summary>
		/// Converts a bool value into <see cref="Visibility"/> as configured in the
		/// <see cref="TrueTrueValue"/>
		/// <see cref="TrueFalseValue"/>
		/// <see cref="FalseFalseValue"/>
		/// <see cref="FalseTrueValue"/>
		/// properties.
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

			if ((values[0] is bool) == false || (values[1] is bool) == false)
				return Binding.DoNothing;

			bool value0 = (bool)values[0];
			bool value1 = (bool)values[1];

			if (value0 && value1)
				return TrueTrueValue;

			if (value0 && value1 == false)
				return TrueFalseValue;

			if (value0 == false && value1 == false)
				return FalseFalseValue;

			return FalseTrueValue;
		}

		/// <summary>
		/// Not Implemented
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
		#endregion methods
	}
}
