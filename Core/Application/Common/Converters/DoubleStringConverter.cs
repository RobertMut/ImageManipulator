using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace ImageManipulator.Application.Common.Converters
{
    public class DoubleStringConverter : IValueConverter
    {
        public static DoubleStringConverter Instance = new DoubleStringConverter();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null) return null;

            if (value.GetType() == typeof(double))
            {
                return $"{(double)value}";
            }

            return null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null) return null;

            if(value.GetType() == typeof(string) && !string.IsNullOrEmpty((string)value))
            {
                    return double.TryParse((string)value, out double result) == true ? result : null;
            }

            return null;
        }
    }
}
