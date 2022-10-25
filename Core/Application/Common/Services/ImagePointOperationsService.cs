using Avalonia.Media;
using Avalonia.Media.Imaging;
using DynamicData;
using ImageManipulator.Application.Common.Helpers;
using ImageManipulator.Common.Common.Helpers;
using ImageManipulator.Domain.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageManipulator.Application.Common.Services
{
    public class ImagePointOperationsService
    {
        private List<double> histogramValues;

        public int CalculateLowerImageThresholdPoint(Dictionary<Color, double[]> histogram = null)
        {
            if (histogram == null && histogramValues == null)
            {
                throw new NullReferenceException("Histogram is null");
            }

            if (histogram.Keys.Count != 1)
            {
                throw new NotImplementedException();
            }

            histogramValues = histogram.Values.FirstOrDefault().ToList();

            double max = histogramValues.Max();
            int indexOfMax = histogramValues.IndexOf(max);
            int index = histogramValues.FindIndex(x => x != 0);

            return ((index + indexOfMax / 2) + index) / 2;
        }

        public int CalculateUpperImageThresholdPoint(Dictionary<Color, double[]> histogram = null)
        {
            if (histogram == null && histogramValues == null)
            {
                throw new NullReferenceException("Histogram is null");
            }

            if (histogram.Keys.Count != 1)
            {
                throw new NotImplementedException();
            }

            histogramValues = histogram.Values.FirstOrDefault().ToList();

            double max = histogramValues.Max();
            int indexOfMax = histogramValues.IndexOf(max);
            int index = histogramValues.FindLastIndex(x => x != 0); 

            return ((index + indexOfMax / 2) + index) / 2;
        }

        public unsafe System.Drawing.Bitmap StretchContrast(Bitmap bitmap, int lowest, int highest)
        {
            System.Drawing.Bitmap newSrc = ImageConverterHelper.ConvertFromAvaloniaUIBitmap(bitmap);
            
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

                    double brightness = CalculationHelper.LuminanceFromRGBValue(data[2]/255d,
                        data[1] / 225d,
                        data[0] / 255d);

                    if (brightness >= lowest && brightness <= highest)
                        data[0] = data[1] = data[2] = (byte)(int)(255 * ((brightness - lowest) / (highest - lowest)));
                }
            }

            newSrc.UnlockBits(bitmapData);

            return newSrc;
        }
    }
}
