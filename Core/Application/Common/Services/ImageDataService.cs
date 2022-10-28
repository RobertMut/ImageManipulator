using Avalonia.Media.Imaging;
using ImageManipulator.Application.Common.Helpers;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Common.Common.Helpers;
using ImageManipulator.Domain.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace ImageManipulator.Application.Common.Services
{
    public class ImageDataService : IImageDataService
    {
        public unsafe Dictionary<string, double[]> CalculateHistogramForImage(Avalonia.Media.Imaging.Bitmap bitmap)
        {
            double[] red = new double[256];
            double[] green = new double[256];
            double[] blue = new double[256];

            System.Drawing.Bitmap newBitmap = ImageConverterHelper.ConvertFromAvaloniaUIBitmap(bitmap);
            var bitmapData = newBitmap.LockBitmap(newBitmap.PixelFormat);
            int scanLine = bitmapData.Stride;
            IntPtr bitmapScan0 = bitmapData.Scan0;
            byte bitsPerPixel = (byte)System.Drawing.Bitmap.GetPixelFormatSize(newBitmap.PixelFormat);

            byte* pixel = (byte*)bitmapScan0.ToPointer();
            for (int i = 0; i < newBitmap.Height; i++)
            {
                for (int j = 0; j < newBitmap.Width; j++)
                {
                    byte* data = pixel + i * bitmapData.Stride + j * bitsPerPixel / 8;

                    red[data[2]]++;
                    green[data[1]]++;
                    blue[data[0]]++;
                }
            }

            newBitmap.UnlockBits(bitmapData);

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

        private unsafe Dictionary<string, double[]> StretchHistogram(Dictionary<string, double[]> existingHistogram, System.Drawing.Bitmap existingImage)
        {
            double[] red = new double[256];
            double[] blue = new double[256];
            double[] green = new double[256];

            existingHistogram["red"] = CalculateLUT(existingHistogram["red"]);
            existingHistogram["green"] = CalculateLUT(existingHistogram["green"]);
            existingHistogram["blue"] = CalculateLUT(existingHistogram["blue"]);

            System.Drawing.Bitmap newImage = new System.Drawing.Bitmap(existingImage.Width, existingImage.Height, existingImage.PixelFormat);
            var bitmapData = newImage.LockBitmap(newImage.PixelFormat);
            int scanLine = bitmapData.Stride;
            IntPtr bitmapScan0 = bitmapData.Scan0;
            byte bitsPerPixel = (byte)System.Drawing.Bitmap.GetPixelFormatSize(newImage.PixelFormat);

            byte* pixel = (byte*)bitmapScan0.ToPointer();
            for (int i = 0; i < newImage.Height; i++)
            {
                for (int j = 0; j < newImage.Width; j++)
                {
                    byte* data = pixel + i * bitmapData.Stride + j * bitsPerPixel / 8;
                    var pixelFromExistingImage = existingImage.GetPixel(j, i);

                    var newPixel = System.Drawing.Color.FromArgb((int)existingHistogram["red"][pixelFromExistingImage.R],
                        (int)existingHistogram["green"][pixelFromExistingImage.G],
                        (int)existingHistogram["blue"][pixelFromExistingImage.B]);

                    data[2] = newPixel.R;
                    data[1] = newPixel.G;
                    data[0] = newPixel.B;

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