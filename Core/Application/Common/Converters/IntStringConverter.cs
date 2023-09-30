using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace ImageManipulator.Application.Common.Converters
{
    public class IntStringConverter : IValueConverter
    {
        public static IntStringConverter Instance = new IntStringConverter();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null) return null;

            if (value is int)
            {
                return $"{(int)value}";
            }

            return null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null) return null;

            if(value is string && !string.IsNullOrEmpty((string)value))
            {
                return int.TryParse((string)value, out int result) ? result : null;
            }

            return null;
        }
    }
}
