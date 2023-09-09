using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Domain.Common.Helpers;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using ImageManipulator.Domain.Common.Dictionaries;

namespace ImageManipulator.Application.Common.Services
{
    public class ImageDataService : IImageDataService
    {
        public int[]?[] CalculateLevels(Avalonia.Media.Imaging.Bitmap? bitmap)
        {
            Bitmap? newBitmap = ImageConverterHelper.ConvertFromAvaloniaUIBitmap(bitmap);
            var bitmapData = newBitmap.LockBitmap(newBitmap.PixelFormat, ImageLockMode.ReadWrite);
            int bytes = Math.Abs(bitmapData.Stride) * newBitmap.Height;

            byte[] buffer = new byte[bytes];
            Marshal.Copy(bitmapData.Scan0, buffer, 0, bytes);

            newBitmap.UnlockBits(bitmapData);

            return GetLevels(ref buffer);
        }

        private static int[]? CalculateLut(ref int[]? values, int shift = 0) =>
            values
                .Select(n => Math.Abs(n << shift))
                .ToArray();

        public int[]? CalculateAverageForGrayGraph(int[]?[] levels)
        {
            int[]? result = new int[256];
            for(int i = 0; i < 256; i++)
            {
                result[i] = CalculationHelper.AverageFromRGB(levels[0][i], levels[1][i], levels[2][i]);
            }

            return result;
        }
        
        public int[]?[] GetHistogramValues(int[]?[] values, Bitmap? existingImage)
        {
            if (values.Length == 3)
            {
                values[0] = CalculateLut(ref values[0], 16);
                values[1] = CalculateLut(ref values[1], 8);
                values[2] = CalculateLut(ref values[2]);
            } else
            {
                values[0] = CalculateLut(ref values[0]);
            }
            
            return values;
        }

        private int[][] GetLevels(ref byte[] buffer)
        {
            int[][] levels = new int[3][]
            {
                new int[256],
                new int[256],
                new int[256]
            };
            
            for (int p = 0; p < buffer.Length; p += 4)
            {
                levels[0][buffer[p]]++;
                levels[1][buffer[p + 1]]++;
                levels[2][buffer[p + 2]]++;
            }

            return levels;
        }
    }
}