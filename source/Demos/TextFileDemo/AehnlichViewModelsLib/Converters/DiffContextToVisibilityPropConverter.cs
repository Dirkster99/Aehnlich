namespace AehnlichViewModelsLib.Converters
{
	using AehnlichViewLib.Enums;
	using System;
	using System.Globalization;
	using System.Windows;
	using System.Windows.Data;

	/// <summary>
	/// Converts a <see cref="DiffContext"/> value into a configurable value of type
	/// <seealso cref="Visibility"/>.
	/// 
	/// Source: http://stackoverflow.com/questions/3128023/wpf-booleantovisibilityconverter-that-converts-to-hidden-instead-of-collapsed-wh
	/// </summary>
	[ValueConversion(typeof(DiffContext), typeof(Visibility))]
	public sealed class DiffContextToVisibilityPropConverter : IValueConverter
	{
		#region constructor
		/// <summary>
		/// Class constructor
		/// </summary>
		public DiffContextToVisibilityPropConverter()
		{
			// set defaults
			BlankValue = Visibility.Collapsed;
			AddedValue = Visibility.Visible;
			DeleteValue = Visibility.Visible;
			ContextValue = Visibility.Visible;
		}
		#endregion constructor

		#region properties
		/// <summary>
		/// Gets/sets the <see cref="Visibility"/> value that is associated
		/// (converted into) with the <see cref="BlankValue"/> value.
		/// </summary>
		public Visibility BlankValue { get; set; }

		/// <summary>
		/// Gets/sets the <see cref="Visibility"/> value that is associated
		/// (converted into) with the <see cref="AddedValue"/> value.
		/// </summary>
		public Visibility AddedValue { get; set; }

		/// <summary>
		/// Gets/sets the <see cref="Visibility"/> value that is associated
		/// (converted into) with the <see cref="DeleteValue"/> value.
		/// </summary>
		public Visibility DeleteValue { get; set; }

		/// <summary>
		/// Gets/sets the <see cref="Visibility"/> value that is associated
		/// (converted into) with the <see cref="ContextValue"/> value.
		/// </summary>
		public Visibility ContextValue { get; set; }
		#endregion properties

		#region methods
		/// <summary>
		/// Convertzs a bool value into <see cref="Visibility"/> as configured in the
		/// <see cref="BlankValue"/> and <see cref="AddedValue"/> properties.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <param name="parameter"></param>
		/// <param name="culture"></param>
		/// <returns></returns>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is DiffContext))
				return null;

			var val = (DiffContext)value;

			switch (val)
			{
				case DiffContext.Blank:
					return this.BlankValue;

				case DiffContext.Added:
					return this.AddedValue;

				case DiffContext.Deleted:
					return this.DeleteValue;

				case DiffContext.Context:
					return this.ContextValue;

				default:
					throw new NotImplementedException(val.ToString());
			}
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
		#endregion methods
	}
}
