using Avalonia.Media;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Application.Common.Models;
using ImageManipulator.Domain.Common.Dictionaries;
using ImageManipulator.Domain.Common.Helpers;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ImageManipulator.Application.Common.Services
{
    public class ImageConvolutionService : IImageConvolutionService
    {
        public unsafe Bitmap Execute(Bitmap bitmap, double[,] kernel, double factor, bool soften = false)
        {
            var newBitmap = new Bitmap(bitmap);

            int kernelWidth = kernel.GetLength(1);
            int kernelHeight = kernel.GetLength(0);
            int radiusY = kernelHeight >> 1;
            int radiusX = kernelWidth >> 1;

            if (soften)
            {
                kernel = PrepareKernel(kernel);
            }
            

            int filterOffset = (kernelWidth - 1) / 2;

            BitmapData sourceData = bitmap
                .LockBitmapReadOnly(PixelFormat.Format32bppArgb);

            BitmapData affectedData = newBitmap
                .LockBitmapReadOnly(PixelFormat.Format32bppArgb);


            for (int y = 0; y < newBitmap.Height; y++)
            {
                for (int x = 0; x < newBitmap.Width; x++)
                {
                    var color = new ColorDouble(0, 0, 0);

                    for (int kernelY = 0; kernelY < kernelHeight; kernelY++)
                    {
                        int kernelYRadius = kernelY - radiusY;
                        int offsetY = Math.Clamp((y + kernelYRadius), 0, affectedData.Height - 1);

                        for (int kernelX = 0; kernelX < kernelWidth; kernelX++)
                        {
                            int kernelXRadius = kernelX - radiusX;
                            int offsetX = Math.Clamp((x + kernelXRadius), 0, affectedData.Width - 1);

                            byte* offsetedPixel = (byte*)sourceData.GetPixel(offsetX, offsetY).ToPointer();

                            color.Red += kernel[kernelY, kernelX] * offsetedPixel[0];
                            color.Green += kernel[kernelY, kernelX] * offsetedPixel[1];
                            color.Blue += kernel[kernelY, kernelX] * offsetedPixel[2];
                        }
                    }

                    byte* pixel = (byte*)affectedData.GetPixel(x, y).ToPointer();
                    byte* sourcePixel = (byte*)sourceData.GetPixel(x, y).ToPointer();
                    pixel[0] = (byte)((color.Red > 255) ? 255 : ((color.Red < 0) ? 0 : color.Red));
                    pixel[1] = (byte)((color.Green > 255) ? 255 : ((color.Green < 0) ? 0 : color.Green));
                    pixel[2] = (byte)((color.Blue > 255) ? 255 : ((color.Blue < 0) ? 0 : color.Blue));
                }
            }

            newBitmap.UnlockBits(affectedData);
            bitmap.UnlockBits(sourceData);

            return newBitmap;
        }

        private double[,] PrepareKernel(double[,] kernel)
        {
            double sum = SumKernel(kernel);
            for (int i = 0; i < kernel.GetLength(0); i++)
            {
                for (int j = 0; j < kernel.GetLength(1); j++)
                {
                    kernel[i,j] = kernel[i,j] / sum;
                }
            }

            return kernel;
        }

        private double SumKernel(double[,] kernel)
        {
            double sum = 0;

            for (int i = 0; i < kernel.GetLength(0); i++)
            {
                for (int j = 0; j < kernel.GetLength(1); j++)
                {
                    sum += kernel[i, j];
                }
            }

            return sum;
        }
    }
}