using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Domain.Common.Helpers;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ImageManipulator.Application.Common.Services
{
    public class ImageDataService : IImageDataService
    {
        public int[]?[] CalculateLevels(Bitmap? bitmap)
        {
            var bitmapData = bitmap.LockBitmap(bitmap.PixelFormat, ImageLockMode.ReadOnly);
            int bytes = Math.Abs(bitmapData.Stride) * bitmap.Height;

            byte[] buffer = new byte[bytes];
            Marshal.Copy(bitmapData.Scan0, buffer, 0, bytes);

            bitmap.UnlockBits(bitmapData);

            return GetLevels(ref buffer);
        }

        public int[]? CalculateAverageForGrayGraph(int[]?[] levels)
        {
            int[]? result = new int[256];
            for(int i = 0; i < 256; i++)
            {
                result[i] = CalculationHelper.AverageFromRGB(levels[0][i], levels[1][i], levels[2][i]);
            }

            return result;
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