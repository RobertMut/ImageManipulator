using System;
using System.Linq;

namespace ImageManipulator.Domain.Common.Helpers
{
    public class CalculationHelper
    {
        [Obsolete("No need to use for now. Maybe in future")]
        public static Func<int[], int[]> CalculateRGBLinear => (doubleArray) =>
            doubleArray.Select(x => x / 255)
            .Select(sR =>
                Convert.ToInt32(sR <= 0.04045 ? sR / 12.92 : Math.Pow((sR + 0.055) / 1.055, 2.4))
            ).ToArray();

        [Obsolete("No need to use for now. Maybe in future")]
        public static Func<int[], int[], int[], int[]> CalculateLuminanceFromRGBLinear => (red, green, blue) =>
            red.Zip(green, (r, g) => new { r, g })
                .Zip(blue, (rg, b) => new { rg.r, rg.g, b })
                .Select(rgb => Convert.ToInt32(0.2126 * rgb.r + 0.7152 * rgb.g + 0.0722 * rgb.b)).ToArray();

        [Obsolete("No need to use for now. Maybe in future")]
        public static Func<int[], int[]> CalculatePerceivedLightness => (luminance) =>
            luminance.Select(x => Convert.ToInt32(x <= 216 / 24389 ? x * (24389 / 27) : Math.Pow(x, 1 / 3) * 116 - 16)).ToArray();

        public static Func<double, double, double, double> LuminanceFromRGBValue => (red, green, blue) => 0.2126 * red + 0.7152 * green + 0.0722 * blue;

        public static Func<int, int, int, int> AverageFromRGB => (red, green, blue) => (red + green + blue) / 3;

        public static Func<double, double, double> CalculateCorrectedGamma => (value, gammaCorrection) => 255 * Math.Pow(value / 255, gammaCorrection);

        public static Func<double, int> FloorValue => (value) => (int)Math.Floor(value * 255);
        
        public static byte HandleValueOutsideBounds(int value) =>
            value switch
            {
                < 0 => 0,
                > 255 => 255,
                _ => (byte)value
            };
        
        public static byte HandleValueOutsideBounds(double value) =>
            value switch
            {
                < 0 => 0,
                > 255 => 255,
                _ => (byte)value
            };
    }
}