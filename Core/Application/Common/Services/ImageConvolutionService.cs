using Avalonia.Media;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Application.Common.Models;
using ImageManipulator.Domain.Common.Dictionaries;
using ImageManipulator.Domain.Common.Helpers;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ImageManipulator.Application.Common.Services
{
    public class ImageConvolutionService : IImageConvolutionService
    {
        public unsafe Bitmap Execute(Bitmap bitmap, double[,] matrix, double factor, int bias = 0)
        {
            var newBitmap = new Bitmap(bitmap);
            var byteOffsetFunc = ImageXYCoordinatesDictionary.ByteOffset[bitmap.PixelFormat];
            var calcOffsetFunc = ImageXYCoordinatesDictionary.CalculationOffset[bitmap.PixelFormat];

            int filterWidth = matrix.GetLength(1);
            int filterHeight = matrix.GetLength(0);

            int filterOffset = (filterWidth - 1) / 2;

            BitmapData sourceData = bitmap.LockBitmapReadOnly(bitmap.PixelFormat);

            byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];
            byte[] resultBuffer = new byte[sourceData.Stride * sourceData.Height];

            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
            bitmap.UnlockBits(sourceData);
            for (int offsetY = filterOffset; offsetY <
            sourceData.Height - filterOffset; offsetY++)
            {
                for (int offsetX = filterOffset; offsetX <
                    sourceData.Width - filterOffset; offsetX++)
                {
                    var color = new ColorDouble(0, 0, 0);

                    var byteOffset = byteOffsetFunc(sourceData.Stride, offsetX, offsetY);

                    CalculateValuesForColours(ref pixelBuffer, ref matrix, ref color, filterOffset, byteOffset, sourceData.Stride, ValueByOffsetAndMatrix, calcOffsetFunc);

                    color.Blue = factor * color.Blue + bias;
                    color.Green = factor * color.Green + bias;
                    color.Red = factor * color.Red + bias;

                    resultBuffer[byteOffset] = (byte)CutValue(color.Blue);
                    resultBuffer[byteOffset+1] = (byte)CutValue(color.Green);
                    resultBuffer[byteOffset+2] = (byte)CutValue(color.Red);
                    resultBuffer[byteOffset+3] = 255;
                }
            }

            var resultData = newBitmap.LockBitmapWriteOnly(bitmap.PixelFormat);
            Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length);
            newBitmap.UnlockBits(resultData);

            return newBitmap;
        }

        private double CutValue(double color)
        {
            if (color > 255)
                color = 255;
            else if (color < 0)
                color = 0;

            return color;
        }

        private void CalculateValuesForColours(ref byte[] pixelBuffer,
            ref double[,] matrix,
            ref ColorDouble color,
            int filterOffset,
            int byteOffset,
            int stride,
            Func<byte[], double[,], int, int, int, int, int, double> func,
            Func<int, int, int, int, int> calcOffsetFunc
            )
        {
            for (int filterY = -filterOffset;
                filterY <= filterOffset; filterY++)
            {
                for (int filterX = -filterOffset, calcOffset = 0;
                    filterX <= filterOffset; filterX++)
                {
                    calcOffset = calcOffsetFunc(byteOffset, stride, filterX, filterY);

                    color.Red += func(pixelBuffer, matrix, filterX, filterY, filterOffset, calcOffset, 0);

                    color.Blue += func(pixelBuffer, matrix, filterX, filterY, filterOffset, calcOffset, 1);

                    color.Green += func(pixelBuffer, matrix, filterX, filterY, filterOffset, calcOffset, 2);

                }
            }
        }

        private Func<byte[], double[,], int, int, int, int, int, double> ValueByOffsetAndMatrix => (pixelBuffer, matrix, filterX, filterY, filterOffset, calcOffset, color) =>
            (pixelBuffer[calcOffset + color]) * matrix[filterY + filterOffset, filterX + filterOffset];
    }
}