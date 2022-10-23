using Avalonia.Data.Converters;
using ImageManipulator.Application.Common.Helpers;
using System;
using System.Globalization;

namespace ImageManipulator.Application.Common.Converters
{
    public class BitmapConverter : IValueConverter
    {
        public static BitmapConverter Instance = new BitmapConverter();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => ImageConverterHelper.ConvertFromSystemDrawingBitmap((System.Drawing.Bitmap)value);

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => ImageConverterHelper.ConvertFromAvaloniaUIBitmap((Avalonia.Media.Imaging.Bitmap)value);
    }
}
