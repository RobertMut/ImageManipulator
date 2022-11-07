using ImageManipulator.Application.Common.Helpers;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Common.Common.Helpers;
using ImageManipulator.Domain.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

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

        private double[] CalculateLUT(ref double[] values)
        {
            double[] result = new double[256];
            double minValue = GetLUTMinValue(ref values);
            double maxValue = GetLUTMaxValue(ref values);
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

        public double[] CalculateAverageForGrayGraph(double[][] levels)
        {
            double[] result = new double[256];
            for(int i = 0; i < 256; i++)
            {
                result[i] = CalculationHelper.AverageFromRGB(levels[0][i], levels[1][i], levels[2][i]);
            }

            return result;
        }

        public double[] CalculateBrightnessFromLuminance(double[] luminanceArray) => CalculationHelper.CalculatePerceivedLightness(luminanceArray);

        public unsafe double[][] StretchHistogram(double[][] values, System.Drawing.Bitmap existingImage)
        {
            double[] red = new double[256];
            double[] blue = new double[256];
            double[] green = new double[256];
            Color newPixel;

            if (values.Length == 3)
            {
                values[0] = CalculateLUT(ref values[0]);
                values[1] = CalculateLUT(ref values[1]);
                values[2] = CalculateLUT(ref values[2]);
            } else
            {
                values[0] = CalculateLUT(ref values[0]);
            }


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

                    if (values.Length == 3)
                    {
                        newPixel = System.Drawing.Color.FromArgb((byte)values[0][pixelFromExistingImage.R],
                            (byte)values[1][pixelFromExistingImage.G],
                            (byte)values[2][pixelFromExistingImage.B]);
                    } else
                    {
                        newPixel = System.Drawing.Color.FromArgb((byte)values[0][pixelFromExistingImage.R],
                            (byte)values[0][pixelFromExistingImage.G],
                            (byte)values[0][pixelFromExistingImage.B]);
                    }

                    data[2] = newPixel.R;
                    data[1] = newPixel.G;
                    data[0] = newPixel.B;

                    red[newPixel.R]++;
                    green[newPixel.G]++;
                    blue[newPixel.B]++;
                }
            }

            if (values.Length == 3)
            {
                values[0] = red;
                values[1] = green;
                values[2] = blue;
            }
            else
            {
                values[0] = red;
            }

            return values;
        }

        private double GetLUTMinValue(ref double[] values)
        {
            for (int i = 0; i < 256; i++)
            {
                if (values[i] != 0)
                    return i;
            }

            return 0;
        }

        private double GetLUTMaxValue(ref double[] values)
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
        }
    }
}