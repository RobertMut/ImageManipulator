using Avalonia.Media.Imaging;
using ImageManipulator.Application.Common.Helpers;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Common.Common.Helpers;
using System.Collections.Generic;

namespace ImageManipulator.Application.Common.Services
{
    public class ImageDataService : IImageDataService
    {
        public Dictionary<string, double[]> CalculateHistogramForImage(Bitmap bitmap)
        {
            double[] red = new double[256];
            double[] green = new double[256];
            double[] blue = new double[256];

            System.Drawing.Bitmap newBitmap = ImageConverterHelper.ConvertFromAvaloniaUIBitmap(bitmap);

            for (int i = 0; i < newBitmap.Width; i++)
            {
                for (int j = 0; j < newBitmap.Height; j++)
                {
                    var pixel = newBitmap.GetPixel(i, j);

                    red[pixel.R]++;
                    green[pixel.G]++;
                    blue[pixel.B]++;
                }
            }

            return StretchHistogram(new Dictionary<string, double[]>
            {
                { "red", red },
                { "green", green },
                { "blue", blue }
            }, newBitmap);
        }

        public double[] CalculateLUT(double[] values)
        {
            double[] result = new double[256];

            double minValue = GetLUTMinValue(values);
            double maxValue = GetLUTMaxValue(values);
            double equation = 255.0 / (maxValue - minValue);
            for (int i = 0; i < 256; i++)
            {
                result[i] = (equation * (i - minValue));
            }

            return result;
        }

        public double[] CalculateLuminanceFromRGB(Dictionary<string, double[]> rgbDictionary)
        {
            double[] eightBitRed = CalculationHelper.CalculateRGBLinear(rgbDictionary["red"]);
            double[] eightBitGreen = CalculationHelper.CalculateRGBLinear(rgbDictionary["green"]);
            double[] eightBitBlue = CalculationHelper.CalculateRGBLinear(rgbDictionary["blue"]);

            return CalculationHelper.CalculateLuminanceFromRGBLinear(eightBitRed, eightBitGreen, eightBitBlue);
        }

        public double[] CalculateBrightnessFromLuminance(double[] luminanceArray) => CalculationHelper.CalculatePerceivedLightness(luminanceArray);

        private Dictionary<string, double[]> StretchHistogram(Dictionary<string, double[]> existingHistogram, System.Drawing.Bitmap existingImage)
        {
            double[] red = new double[256];
            double[] blue = new double[256];
            double[] green = new double[256];

            existingHistogram["red"] = CalculateLUT(existingHistogram["red"]);
            existingHistogram["green"] = CalculateLUT(existingHistogram["green"]);
            existingHistogram["blue"] = CalculateLUT(existingHistogram["blue"]);

            System.Drawing.Bitmap newImage = new System.Drawing.Bitmap(existingImage.Width, existingImage.Height, existingImage.PixelFormat);
            for (int i = 0; i < existingImage.Width; i++)
            {
                for (int j = 0; j < existingImage.Height; j++)
                {
                    var pixel = existingImage.GetPixel(i, j);
                    var newPixel = System.Drawing.Color.FromArgb((int)existingHistogram["red"][pixel.R],
                        (int)existingHistogram["green"][pixel.G],
                        (int)existingHistogram["blue"][pixel.B]);
                    newImage.SetPixel(i, j, newPixel);
                    red[newPixel.R]++;
                    green[newPixel.G]++;
                    blue[newPixel.B]++;
                }
            }
            existingHistogram["red"] = red;
            existingHistogram["green"] = green;
            existingHistogram["blue"] = blue;

            return existingHistogram;
        }

        private double GetLUTMinValue(double[] values)
        {
            for (int i = 0; i < 256; i++)
            {
                if (values[i] != 0)
                    return i;
            }

            return 0;
        }

        private double GetLUTMaxValue(double[] values)
        {
            for (int j = 255; j >= 0; j--)
            {
                if (values[j] != 0)
                    return j;
            }

            return 255;
        }
    }
}
