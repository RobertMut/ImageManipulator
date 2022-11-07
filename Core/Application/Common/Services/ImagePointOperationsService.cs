﻿using ImageManipulator.Application.Common.Interfaces;
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

        public unsafe System.Drawing.Bitmap HistogramEqualization(System.Drawing.Bitmap bitmap, double[][] lut)
        {
            var bitmapData = bitmap.LockBitmap(bitmap.PixelFormat);
            int bytes = Math.Abs(bitmapData.Stride) * bitmap.Height;
            byte[] buffer = new byte[bytes];
            byte[] result = new byte[bytes];

            Marshal.Copy(bitmapData.Scan0, buffer, 0, bytes);

            bitmap.UnlockBits(bitmapData);

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    int data = (y * bitmapData.Stride) + (x * 4);
                    double sum = 0;

                    for (int i = 0; i < buffer[data]; i++)
                    {
                        sum += lut[0][i];
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
            var bitmapData = newSrc
                .LockBitmap(newSrc.PixelFormat)
                .ExecuteOnPixel((x, scan0, stride) =>
                {
                    byte* data = (byte*)x.ToPointer();

                    data[0] = (byte)(255 - data[0]);
                    data[1] = (byte)(255 - data[1]);
                    data[2] = (byte)(255 - data[2]);

                    return new IntPtr(data);
                });

            newSrc.UnlockBits(bitmapData);

            return newSrc;
        }

        public unsafe Bitmap Thresholding(Bitmap bitmap, double[][] lut, int threshold, bool replace = true)
        {
            double value = lut[0][threshold];
            int rgb = 0;
            System.Drawing.Bitmap newSrc = new System.Drawing.Bitmap(bitmap);
            newSrc.UnlockBits(newSrc
                .LockBitmap(newSrc.PixelFormat)
                .ExecuteOnPixel((x, scan0, stride) =>
                {
                    byte* data = (byte*)x.ToPointer();
                    rgb = data[0] + data[1] + data[2];

                    if (rgb < value)
                    {
                        data[0] = 0;
                        data[1] = 0;
                        data[2] = 0;
                    } else if (replace)
                    {
                        data[0] = 255;
                        data[1] = 255;
                        data[2] = 255;
                    }

                    return new IntPtr(data);
            }));

            return newSrc;
        }

        public unsafe Bitmap MultiThresholding(Bitmap bitmap, double[][] lut, int lowerThreshold, int upperThreshold, bool replace = true)
        {
            System.Drawing.Bitmap newSrc = new System.Drawing.Bitmap(bitmap);
            double lowerValue = lut[0][lowerThreshold];
            double upperValue = lut[0][upperThreshold];
            int rgb = 0;
            newSrc.UnlockBits(newSrc
                .LockBitmap(newSrc.PixelFormat)
                .ExecuteOnPixel((x, scan0, stride) =>
                {
                    byte* data = (byte*)x.ToPointer();
                    rgb = data[0] + data[1] + data[2];

                    if (lowerValue > rgb && rgb <= upperValue)
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

                    return new IntPtr(data);
                }));

            return newSrc;
        }
    }
}