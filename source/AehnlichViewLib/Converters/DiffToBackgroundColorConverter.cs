namespace AehnlichViewLib.Converters
{
    using AehnlichViewLib.Interfaces;
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// Considers all arguments of difference between two compared items and
    /// converts it into an associated background color key.
    /// </summary>
    public class DiffToBackgroundColorConverter : DependencyObject, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
                return Binding.DoNothing;

            if (values.Length != 4)
                return Binding.DoNothing;

            bool IsFromA = true;
            if ((string)parameter == "fromB")
                IsFromA = false;
            else
                IsFromA = true;

            bool IsItemInA = (bool)values[0];
            bool IsItemInB = (bool)values[1];
            bool IsItemDifferent = (bool)values[2];
            var colorDefs = values[3] as IDiffColorDefinitions;

            if (colorDefs == null)
                return Binding.DoNothing;

            if (IsItemDifferent && IsItemInA == true && IsItemInB == true)
                return colorDefs.ColorBackgroundContext;

            if (IsItemInA == false && IsItemInB == true)
            {
                if (IsFromA)
                    return colorDefs.ColorBackgroundImaginaryLineAdded;
                else
                    return colorDefs.ColorBackgroundAdded;
            }
            else
            {
                if (IsItemInA == true && IsItemInB == false)
                {
                    if (IsFromA)
                        return colorDefs.ColorBackgroundDeleted;
                    else
                        return colorDefs.ColorBackgroundImaginaryLineDeleted;
                }
            }

            // Entries appear to be equal
            return Binding.DoNothing;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
