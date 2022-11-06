using System;
using System.Linq;

namespace ImageManipulator.Common.Common.Helpers
{
    public class CalculationHelper
    {
        public static Func<double[], double[]> CalculateRGBLinear => (doubleArray) =>
            doubleArray.Select(x => x / 255)
            .Select(sR =>
                sR <= 0.04045 ? sR / 12.92 : Math.Pow(((sR + 0.055) / 1.055), 2.4)
            ).ToArray();

        public static Func<double[], double[], double[], double[]> CalculateLuminanceFromRGBLinear => (red, green, blue) =>
            red.Zip(green, (r, g) => new { r, g })
                .Zip(blue, (rg, b) => new { rg.r, rg.g, b })
                .Select(rgb => (0.2126 * rgb.r + 0.7152 * rgb.g + 0.0722 * rgb.b)).ToArray();

        public static Func<double[], double[]> CalculatePerceivedLightness => (luminance) =>
            luminance.Select(x => x <= (216 / 24389) ? (x * (24389 / 27)) : (Math.Pow(x, (1 / 3)) * 116 - 16)).ToArray();
        
        public static Func<double, double, double, double> LuminanceFromRGBValue => (red, green, blue) => 0.2126 * red + 0.7152 * green + 0.0722 * blue;

        public static Func<double, double, double> CalculateCorrectedGamma => (value, gammaCorrection) => 255 * Math.Pow(value / 255, gammaCorrection);
    }
}