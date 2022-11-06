using ImageManipulator.Application.Common.Helpers;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Common.Common.Helpers;
using ImageManipulator.Domain.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ImageManipulator.Application.Common.Services
{
    public class ImageDataService : IImageDataService
    {
        private double[][] _imageLevels;

        public ImageDataService()
        {
            _imageLevels = new double[3][];
            _imageLevels[0] = new double[256];
            _imageLevels[1] = new double[256];
            _imageLevels[2] = new double[256];
        }

        public unsafe double[][] CalculateLevels(Avalonia.Media.Imaging.Bitmap bitmap)
        {
            System.Drawing.Bitmap newBitmap = ImageConverterHelper.ConvertFromAvaloniaUIBitmap(bitmap);
            var bitmapData = newBitmap.LockBitmap(newBitmap.PixelFormat);
            int bytes = Math.Abs(bitmapData.Stride) * newBitmap.Height;

            byte[] buffer = new byte[bytes];
            Marshal.Copy(bitmapData.Scan0, buffer, 0, bytes);

            GetLevels(ref buffer, newBitmap.Width, newBitmap.Height);

            newBitmap.UnlockBits(bitmapData);

            return _imageLevels;
        }

        public double[] CalculateLUT(double[] values)
        {
            double[] result = new double[256];
            double minValue = GetLUTMinValue(values);
            double maxValue = GetLUTMaxValue(values);
            for (int i = 0; i < 256; i++)
            {
                result[i] = ((255 / (maxValue - minValue)) * (i - minValue));
            }

            return result;
        }

        public double[] CalculateLuminanceFromRGB(double[][] levels)
        {
            double[] eightBitRed = CalculationHelper.CalculateRGBLinear(levels[0]);
            double[] eightBitGreen = CalculationHelper.CalculateRGBLinear(levels[1]);
            double[] eightBitBlue = CalculationHelper.CalculateRGBLinear(levels[2]);

            return CalculationHelper.CalculateLuminanceFromRGBLinear(eightBitRed, eightBitGreen, eightBitBlue);
        }

        public double[] CalculateBrightnessFromLuminance(double[] luminanceArray) => CalculationHelper.CalculatePerceivedLightness(luminanceArray);

        public unsafe Dictionary<string, double[]> StretchHistogram(Dictionary<string, double[]> stringImageValues, System.Drawing.Bitmap existingImage)
        {
            double[] red = new double[256];
            double[] blue = new double[256];
            double[] green = new double[256];

            stringImageValues["red"] = CalculateLUT(stringImageValues["red"]);
            stringImageValues["green"] = CalculateLUT(stringImageValues["green"]);
            stringImageValues["blue"] = CalculateLUT(stringImageValues["blue"]);

            System.Drawing.Bitmap newImage = new System.Drawing.Bitmap(existingImage.Width, existingImage.Height, existingImage.PixelFormat);
            var bitmapData = newImage.LockBitmap(newImage.PixelFormat);
            int scanLine = bitmapData.Stride;
            IntPtr bitmapScan0 = bitmapData.Scan0;
            byte bitsPerPixel = (byte)Image.GetPixelFormatSize(newImage.PixelFormat);

            byte* pixel = (byte*)bitmapScan0.ToPointer();
            for (int i = 0; i < newImage.Height; i++)
            {
                for (int j = 0; j < newImage.Width; j++)
                {
                    byte* data = pixel + i * bitmapData.Stride + j * bitsPerPixel / 8;
                    var pixelFromExistingImage = existingImage.GetPixel(j, i);

                    var newPixel = System.Drawing.Color.FromArgb((byte)stringImageValues["red"][pixelFromExistingImage.R],
                        (byte)stringImageValues["green"][pixelFromExistingImage.G],
                        (byte)stringImageValues["blue"][pixelFromExistingImage.B]);

                    data[2] = newPixel.R;
                    data[1] = newPixel.G;
                    data[0] = newPixel.B;

                    red[newPixel.R]++;
                    green[newPixel.G]++;
                    blue[newPixel.B]++;
                }
            }

            stringImageValues["red"] = red;
            stringImageValues["green"] = green;
            stringImageValues["blue"] = blue;

            return stringImageValues;
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

        private void GetLevels(ref byte[] buffer, int width, int height)
        {
            for (int p = 0; p < buffer.Length; p += 4)
            {
                _imageLevels[0][buffer[p]]++;
                _imageLevels[1][buffer[p + 1]]++;
                _imageLevels[2][buffer[p + 2]]++;
            }

            int pixels = width * height;
            for (int prob = 0; prob < 256; prob++)
            {
                _imageLevels[0][prob] /= pixels;
                _imageLevels[1][prob] /= pixels;
                _imageLevels[2][prob] /= pixels;
            }
        }
    }
}