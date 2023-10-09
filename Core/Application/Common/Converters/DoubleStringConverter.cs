using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace ImageManipulator.Application.Common.Converters;

public class DoubleStringConverter : IValueConverter
{
    public static DoubleStringConverter Instance = new DoubleStringConverter();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value == null ? null : value.ToString();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value != null && Double.TryParse(value.ToString(), out double result))
        {
            return result;
        }

        return null;
    }
}