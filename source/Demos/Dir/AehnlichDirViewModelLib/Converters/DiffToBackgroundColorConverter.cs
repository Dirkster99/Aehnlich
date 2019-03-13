﻿namespace AehnlichDirViewModelLib.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

    public class DiffToBackgroundColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
                return Binding.DoNothing;

            if (values.Length != 3)
                return Binding.DoNothing;

            bool IsItemInA = (bool)values[0];
            bool IsItemInB = (bool)values[1];
            bool IsItemDifferent = (bool)values[2];

            if (IsItemDifferent)
                return new SolidColorBrush(Colors.Green);

            if (IsItemInA == true && IsItemInB == true)
                return Binding.DoNothing;

            if (IsItemInA == true && IsItemInB == false)
                return new SolidColorBrush(Colors.Blue);
            else
                return new SolidColorBrush(Colors.Red);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}