using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Common.Common.Helpers;
using ImageManipulator.Domain.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImageManipulator.Application.Common.Services
{
    public class ImagePointOperationsService : IImagePointOperationsService
    {
        private List<double> _histogramValues;

        public int CalculateLowerImageThresholdPoint(double[] histogram = null)
        {
            if (histogram == null && _histogramValues == null)
            {
                throw new NullReferenceException("Histogram is null");
            }

            _histogramValues = histogram.ToList();

            double max = _histogramValues.Max();
            int indexOfMax = _histogramValues.IndexOf(max);
            int index = _histogramValues.FindIndex(x => x != 0);

            return (((index + indexOfMax) / 2) + index) / 2;
        }

        public int CalculateUpperImageThresholdPoint(double[] histogram = null)
        {
            if (histogram == null && _histogramValues == null)
            {
                throw new NullReferenceException("Histogram is null");
            }

            _histogramValues = histogram.ToList();

            double max = _histogramValues.Max();
            int indexOfMax = _histogramValues.IndexOf(max);
            int index = _histogramValues.FindLastIndex(x => x != 0);

            return (((index + indexOfMax) / 2) + index) / 2;
        }

        public unsafe System.Drawing.Bitmap StretchContrast(System.Drawing.Bitmap bitmap, int lowest, int highest)
        {
            System.Drawing.Bitmap newSrc = new System.Drawing.Bitmap(bitmap);

            var bitmapData = newSrc.LockBitmap(newSrc.PixelFormat);
            int scanLine = bitmapData.Stride;
            IntPtr bitmapScan0 = bitmapData.Scan0;
            byte bitsPerPixel = (byte)System.Drawing.Bitmap.GetPixelFormatSize(newSrc.PixelFormat);

            byte* pixel = (byte*)bitmapScan0.ToPointer();
            for (int i = 0; i < newSrc.Height; i++)
            {
                for (int j = 0; j < newSrc.Width; j++)
                {
                    byte* data = pixel + i * bitmapData.Stride + j * bitsPerPixel / 8;

                    double brightness = CalculationHelper.LuminanceFromRGBValue(data[2],
                        data[1],
                        data[0]);

                    if (brightness >= lowest && brightness <= highest)
                        data[0] = data[1] = data[2] = (byte)(int)(255 * ((brightness - lowest) / (highest - lowest)));
                }
            }

            newSrc.UnlockBits(bitmapData);

            return newSrc;
        }
    }
}
