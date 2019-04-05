namespace AehnlichViewModelsLib.Converters
{
    using AehnlichViewLib.Enums;
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;

    /// <summary>
    /// Converts a <see cref="DiffContext"/> value into a configurable value of type
    /// <seealso cref="SolidColorBrush"/>.
    /// 
    /// Source: http://stackoverflow.com/questions/3128023/wpf-booleantovisibilityconverter-that-converts-to-hidden-instead-of-collapsed-wh
    /// </summary>
    [ValueConversion(typeof(DiffContext), typeof(SolidColorBrush))]
    public sealed class DiffContextToColorPropConverter : DependencyObject, IValueConverter
    {
        #region fields
        public static readonly DependencyProperty BlankValueProperty =
            DependencyProperty.Register("BlankValue", typeof(SolidColorBrush),
                typeof(DiffContextToColorPropConverter), new PropertyMetadata(null));

        public static readonly DependencyProperty AddedValueProperty =
            DependencyProperty.Register("AddedValue", typeof(SolidColorBrush),
                typeof(DiffContextToColorPropConverter), new PropertyMetadata(null));

        public static readonly DependencyProperty DeletedValueProperty =
            DependencyProperty.Register("DeletedValue", typeof(SolidColorBrush),
                typeof(DiffContextToColorPropConverter), new PropertyMetadata(null));

        public static readonly DependencyProperty ContextValueProperty =
            DependencyProperty.Register("ContextValue", typeof(SolidColorBrush),
                typeof(DiffContextToColorPropConverter), new PropertyMetadata(null));
        #endregion fields

        #region constructor
        /// <summary>
        /// Class constructor
        /// </summary>
        public DiffContextToColorPropConverter()
        {
        }
        #endregion constructor

        #region properties
        public SolidColorBrush BlankValue
        {
            get { return (SolidColorBrush)GetValue(BlankValueProperty); }
            set { SetValue(BlankValueProperty, value); }
        }

        public SolidColorBrush AddedValue
        {
            get { return (SolidColorBrush)GetValue(AddedValueProperty); }
            set { SetValue(AddedValueProperty, value); }
        }

        public SolidColorBrush DeletedValue
        {
            get { return (SolidColorBrush)GetValue(DeletedValueProperty); }
            set { SetValue(DeletedValueProperty, value); }
        }

        public SolidColorBrush ContextValue
        {
            get { return (SolidColorBrush)GetValue(ContextValueProperty); }
            set { SetValue(ContextValueProperty, value); }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Converts a bool value into <see cref="Visibility"/> as configured in the
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
            Brush brush = null;

            switch (val)
            {
                case DiffContext.Blank:
                    brush = this.BlankValue;
                    break;

                case DiffContext.Added:
                    brush = this.AddedValue;
                    break;

                case DiffContext.Deleted:
                    brush = this.DeletedValue;
                    break;

                case DiffContext.Context:
                    brush = this.ContextValue;
                    break;

                default:
                    throw new NotImplementedException(val.ToString());
            }

            if (brush == null)
                return Binding.DoNothing;

            return brush;
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
