﻿using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Common.Extensions;
using ImageManipulator.Domain.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace ImageManipulator.Application.Common.Services
{
    public class ImagePointOperationsService : IImagePointOperationsService
    {
        private List<int> _histogramValues;

        public int CalculateLowerImageThresholdPoint(int[]? histogram = null)
        {
            if (histogram == null && _histogramValues == null)
            {
                throw new NullReferenceException("Histogram is null");
            }

            _histogramValues = histogram.ToList();

            int max = _histogramValues.Max();
            int indexOfMax = _histogramValues.IndexOf(max);
            int index = _histogramValues.FindIndex(x => x != 0);

            return (((index + indexOfMax) / 2) + index) / 2;
        }

        public int CalculateUpperImageThresholdPoint(int[]? histogram = null)
        {
            if (histogram == null && _histogramValues == null)
            {
                throw new NullReferenceException("Histogram is null");
            }

            _histogramValues = histogram.ToList();

            int max = _histogramValues.Max();
            int indexOfMax = _histogramValues.IndexOf(max);
            int index = _histogramValues.FindLastIndex(x => x != 0);

            return (((index + indexOfMax) / 2) + index) / 2;
        }

        public unsafe Bitmap? StretchContrast(Bitmap? bitmap, int lowest, int highest)
        {
            Bitmap? newSrc = new Bitmap(bitmap);

            var bitmapData = newSrc.LockBitmap(newSrc.PixelFormat, ImageLockMode.ReadWrite)
                .ExecuteOnPixels((x, _, _) =>
            {
                byte* data = (byte*)x.ToPointer();

                double brightness = CalculationHelper.LuminanceFromRGBValue(data[2],
                        data[1],
                        data[0]);

                    if (brightness >= lowest && brightness <= highest)
                        data[0] = data[1] = data[2] = (byte)(int)(255 * ((brightness - lowest) / (highest - lowest)));
            });

            newSrc.UnlockBits(bitmapData);

            return newSrc;
        }

        public unsafe Bitmap? NonLinearlyStretchContrast(Bitmap? bitmap, double gamma)
        {
            Bitmap? newSrc = new Bitmap(bitmap);

            var bitmapData = newSrc.LockBitmap(newSrc.PixelFormat, ImageLockMode.ReadWrite).ExecuteOnPixels((x, _, _) =>
            {
                byte* data = (byte*)x.ToPointer();

                data[0] = (byte)CalculationHelper.CalculateCorrectedGamma(data[0], gamma);
                data[1] = (byte)CalculationHelper.CalculateCorrectedGamma(data[1], gamma);
                data[2] = (byte)CalculationHelper.CalculateCorrectedGamma(data[2], gamma);
            });

            newSrc.UnlockBits(bitmapData);

            return newSrc;
        }

        public unsafe Bitmap? HistogramEqualization(Bitmap? bitmap, int[]?[] lut)
        {
            var probability = new double[3][];
            
            for(int i = 0; i < 3; i ++)
            {
                probability[i] = new double[256];
                for(int j = 0; j < 256; j++)
                {
                    probability[i][j] = lut[i][j];
                }
            }

            Bitmap? newSrc = new Bitmap(bitmap);
            double totalNum = bitmap.Height * bitmap.Width;
            var bitmapData = newSrc.LockBitmap(newSrc.PixelFormat, ImageLockMode.ReadWrite);
            var sourceBitmapData = bitmap.LockBitmap(bitmap.PixelFormat, ImageLockMode.ReadOnly);

            for (int k = 0; k < 256; k++)
            {
                probability[0][k] /= totalNum;
                probability[1][k] /= totalNum;
                probability[2][k] /= totalNum;
            }

            int[] red = probability[0].CumulativeSum().Select(x => CalculationHelper.FloorValue(x)).ToArray();
            int[] green = probability[1].CumulativeSum().Select(x => CalculationHelper.FloorValue(x)).ToArray();
            int[] blue = probability[2].CumulativeSum().Select(x => CalculationHelper.FloorValue(x)).ToArray();

            bitmapData.ExecuteOnPixels((x, _, _, i, j) =>
            {
                byte* pixelData = (byte*)x.ToPointer();
                byte* otherImagePixelData = (byte*)sourceBitmapData.GetPixel(i, j).ToPointer();

                pixelData[0] = (byte)red[otherImagePixelData[0]];
                pixelData[1] = (byte)green[otherImagePixelData[1]];
                pixelData[2] = (byte)blue[otherImagePixelData[2]];
            });

            bitmap.UnlockBits(sourceBitmapData);
            newSrc.UnlockBits(bitmapData);

            return newSrc;
        }

        public unsafe Bitmap? Negation(Bitmap? bitmap)
        {
            Bitmap? newSrc = new Bitmap(bitmap);
            var bitmapData = newSrc
                .LockBitmap(newSrc.PixelFormat, ImageLockMode.ReadWrite)
                .ExecuteOnPixels((x, _, _) =>
                {
                    byte* data = (byte*)x.ToPointer();

                    data[0] = (byte)(255 - data[0]);
                    data[1] = (byte)(255 - data[1]);
                    data[2] = (byte)(255 - data[2]);
                });

            newSrc.UnlockBits(bitmapData);

            return newSrc;
        }

        public unsafe Bitmap? Thresholding(Bitmap? bitmap, int threshold, bool replace = true)
        {
            Bitmap? newSrc = new Bitmap(bitmap);
            newSrc.UnlockBits(newSrc
                .LockBitmap(newSrc.PixelFormat, ImageLockMode.ReadWrite)
                .ExecuteOnPixels((x, _, _) =>
                {
                    byte* data = (byte*)x.ToPointer();
                    double rgb = data[0];

                    if (rgb < threshold)
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
                }));

            return newSrc;
        }

        public unsafe Bitmap? MultiThresholding(Bitmap? bitmap, int lowerThreshold, int upperThreshold, bool replace = true)
        {
            Bitmap? newSrc = new Bitmap(bitmap);
            newSrc.UnlockBits(newSrc
                .LockBitmap(newSrc.PixelFormat, ImageLockMode.ReadWrite)
                .ExecuteOnPixels((x, _, _) =>
                {
                    byte* data = (byte*)x.ToPointer();
                    double rgb = data[0];
                    if (rgb < lowerThreshold || rgb > upperThreshold)
                    {
                        data[0] = data[1] = data[2] = 0;
                    }

                    if (rgb < upperThreshold && rgb > lowerThreshold && replace)
                    {
                        data[0] = 255;
                        data[1] = 255;
                        data[2] = 255;
                    }
                }));

            return newSrc;
        }
    }
}