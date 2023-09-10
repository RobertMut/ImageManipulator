using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace ImageManipulator.Application.Common.Converters
{
    public class HexColorConverter : IValueConverter
    {
        public static HexColorConverter Instance = new HexColorConverter();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null) return null;

            if (value is string)
            {
                //List<byte> colors = new List<byte>(3);
                //IEnumerable<char[]> colorHex = ((string)value).Skip(1).ToArray().GetSlices(2);

                //foreach(char[] color in colorHex)
                //{
                //    colors.Add(System.Convert.ToByte(string.Join("", color), 16));
                //}

                return Color.Parse(value.ToString());
            }

            return null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null) return null;

            if (value is Color)
            {
                Color color = (Color)value;

                return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
            }

            return null;
        }
    }
}
