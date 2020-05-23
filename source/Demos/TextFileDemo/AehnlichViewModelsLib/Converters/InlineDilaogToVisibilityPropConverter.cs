namespace AehnlichViewModelsLib.Converters
{
	using AehnlichViewModelsLib.Enums;
	using System;
	using System.Globalization;
	using System.Windows;
	using System.Windows.Data;

	/// <summary>
	/// Converts a <see cref="InlineDialogMode"/> value into a configurable
	/// value of type <seealso cref="Visibility"/>.
	/// </summary>
	[ValueConversion(typeof(InlineDialogMode), typeof(Visibility))]
	public sealed class InlineDilaogToVisibilityPropConverter : IValueConverter
	{
		#region constructor
		/// <summary>
		/// Class constructor
		/// </summary>
		public InlineDilaogToVisibilityPropConverter()
		{
			// set defaults
			NoneValue = Visibility.Collapsed;
		}
		#endregion constructor

		#region properties
		/// <summary>
		/// Gets/sets the <see cref="Visibility"/> value that is associated
		/// (converted into) with the None value.
		/// </summary>
		public Visibility NoneValue { get; set; }
		#endregion properties

		#region methods
		/// <summary>
		/// Converts a value from the <see cref="InlineDialogMode"/> enumeration into
		/// a <see cref="Visibility"/> value as configured in the properties of this converter.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <param name="parameter"></param>
		/// <param name="culture"></param>
		/// <returns></returns>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if ((value is InlineDialogMode) == false)
				return null;

			var val = (InlineDialogMode)value;

			if (val == InlineDialogMode.None)
				return NoneValue;

			return Visibility.Visible;
		}

		/// <summary>
		/// NotImplemented
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <param name="parameter"></param>
		/// <param name="culture"></param>
		/// <returns></returns>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
		#endregion methods
	}
}
