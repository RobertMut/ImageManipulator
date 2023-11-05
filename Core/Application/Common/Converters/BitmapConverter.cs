using Avalonia.Data.Converters;
using System;
using System.Drawing;
using System.Globalization;
using ImageManipulator.Domain.Common.Helpers;

namespace ImageManipulator.Application.Common.Converters
{
    public class BitmapConverter : IValueConverter
    {
        public static BitmapConverter Instance = new BitmapConverter();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null) return null;

            if (value is Bitmap)
            {
                return ImageConverterHelper.ConvertFromSystemDrawingBitmap((Bitmap)value);
            }

            return null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null) return null;

            if (value.GetType() == typeof(Avalonia.Media.Imaging.Bitmap))
            {
                return ImageConverterHelper.ConvertFromAvaloniaUIBitmap((Avalonia.Media.Imaging.Bitmap)value);
            }

            return null;
        }
    }
}
