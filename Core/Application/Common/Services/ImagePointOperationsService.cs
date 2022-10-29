using Avalonia.Controls;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Common.Common.Helpers;
using ImageManipulator.Domain.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;

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

        public unsafe System.Drawing.Bitmap NonLinearlyStretchContrast(System.Drawing.Bitmap bitmap, double gamma)
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

                    data[0] = (byte)CalculationHelper.CalculateCorrectedGamma(data[0], gamma);
                    data[1] = (byte)CalculationHelper.CalculateCorrectedGamma(data[1], gamma);
                    data[2] = (byte)CalculationHelper.CalculateCorrectedGamma(data[2], gamma);
                }
            }

            newSrc.UnlockBits(bitmapData);

            return newSrc;
        }

        public unsafe System.Drawing.Bitmap HistogramEqualization(System.Drawing.Bitmap bitmap)
        {
            var bitmapData = bitmap.LockBitmap(bitmap.PixelFormat);
            byte bitsPerPixel = (byte)System.Drawing.Bitmap.GetPixelFormatSize(bitmap.PixelFormat);
            int bytes = Math.Abs(bitmapData.Stride) * bitmap.Height;
            byte[] buffer = new byte[bytes];
            byte[] result = new byte[bytes];

            Marshal.Copy(bitmapData.Scan0, buffer, 0, bytes);

            bitmap.UnlockBits(bitmapData);

            double[] levels = CalculateLevels(buffer, bitmap.Width, bitmap.Height, 4);

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    int data = (y * bitmapData.Stride) + (x * 4);
                    double sum = 0;

                    for (int i = 0; i < buffer[data]; i++)
                    {
                        sum += levels[i];
                    }

                    for (int c = 0; c < 3; c++)
                    {
                        result[data + c] = (byte)Math.Floor(255 * sum);
                    }

                    result[data + 4] = 255;
                }
            }

            System.Drawing.Bitmap newSrc = new System.Drawing.Bitmap(bitmap.Width, bitmap.Height);
            var newSrcBitmapData = newSrc.LockBitmap(newSrc.PixelFormat);

            Marshal.Copy(result, 0, newSrcBitmapData.Scan0, bytes);
            newSrc.UnlockBits(bitmapData);

            return newSrc;
        }

        public unsafe Bitmap Negation(Bitmap bitmap)
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

                    data[0] = (byte)(255 - data[0]);
                    data[1] = (byte)(255 - data[1]);
                    data[2] = (byte)(255 - data[2]);
                }
            }

            newSrc.UnlockBits(bitmapData);

            return newSrc;
        }

        public unsafe Bitmap Thresholding(Bitmap bitmap, int threshold, bool replace = true)
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
                    int rgb = data[0] + data[1] + data[2];
                    
                    if (rgb <= threshold)
                    {
                        data[0] = 0;
                        data[1] = 0;
                        data[2] = 0;
                    } else if(replace)
                    {
                        data[0] = 255;
                        data[1] = 255;
                        data[2] = 255;
                    }

                }
            }

            newSrc.UnlockBits(bitmapData);

            return newSrc;
        }

        public unsafe Bitmap MultiThresholding(Bitmap bitmap, int lowerThreshold, int upperThreshold, bool replace = true)
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
                    int rgb = data[0] + data[1] + data[2];

                    if (lowerThreshold <= rgb && rgb <= upperThreshold)
                    {
                        data[0] = 0;
                        data[1] = 0;
                        data[2] = 0;
                    }
                    else if (replace)
                    {
                        data[0] = 255;
                        data[1] = 255;
                        data[2] = 255;
                    }

                }
            }

            newSrc.UnlockBits(bitmapData);

            return newSrc;
        }

        //TODO: it'll be new lut calculation
        private double[] CalculateLevels(byte[] buffer, int width, int height, byte bits)
        {
            double[] levels = new double[256];
            for (int p = 0; p < buffer.Length; p += bits)
            {
                levels[buffer[p]]++;
            }
            for (int prob = 0; prob < levels.Length; prob++)
            {
                levels[prob] /= (width * height);
            }

            return levels;
        }
    }
}