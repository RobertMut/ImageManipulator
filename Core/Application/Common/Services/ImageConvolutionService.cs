using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Application.Common.Models;
using ImageManipulator.Domain.Common.Helpers;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace ImageManipulator.Application.Common.Services
{
    public class ImageConvolutionService : IImageConvolutionService
    {
        private static readonly int[,] _kernelX =
        {
            { -1, 0, 1 },
            { -2, 0, 2 },
            { -1, 0, 1 }
        };

        private static readonly int[,] _kernelY =
        {
            { 1, 2, 1 },
            { 0, 0, 0 },
            { -1, -2, -1 }
        };

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
                    pixel[0] = (byte)((color.Red > 255) ? 255 : ((color.Red < 0) ? 0 : color.Red));
                    pixel[1] = (byte)((color.Green > 255) ? 255 : ((color.Green < 0) ? 0 : color.Green));
                    pixel[2] = (byte)((color.Blue > 255) ? 255 : ((color.Blue < 0) ? 0 : color.Blue));
                }
            }

            newBitmap.UnlockBits(affectedData);
            bitmap.UnlockBits(sourceData);

            return newBitmap;
        }

        public unsafe double[,] ComputeGradient(Bitmap bitmap, Func<double, double, double> func)
        {
            double[,] gradient = new double[bitmap.Width, bitmap.Height];
            var bitmapData = bitmap.LockBitmapReadOnly(bitmap.PixelFormat);

            for (int y = 1; y < bitmap.Height - 1; y++)
            {
                for (int x = 1; x < bitmap.Width - 1; x++)
                {
                    double gx = 0;
                    double gy = 0;

                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            byte* pixelData = (byte*)bitmapData.GetPixel(x + j, y + i);
                            int intensity = pixelData[0];

                            gx += intensity * _kernelX[i + 1, j + 1];
                            gy += intensity * _kernelY[i + 1, j + 1];
                        }
                    }

                    gradient[x, y] = func(gx, gy);
                }
            }

            bitmap.UnlockBits(bitmapData);

            return gradient;
        }

        public unsafe Bitmap NonMaxSupression(double[,] gradientMagnitude, double[,] gradientDirection)
        {
            int width = gradientMagnitude.GetLength(0);
            int height = gradientMagnitude.GetLength(1);

            Bitmap nonMax = new Bitmap(width, height);

            var data = nonMax.LockBitmapWriteOnly(nonMax.PixelFormat).ExecuteOnPixels(
                1, nonMax.Width-1,
                1, nonMax.Height-1,
                (pixelPtr, scan0, stride, x, y) =>
                {
                    byte* pixel = (byte*)pixelPtr.ToPointer();
                    double direction = gradientDirection[x, y];
                    double magnitude = gradientMagnitude[x, y];

                    if (
                        ((direction > -Math.PI / 8 && direction <= Math.PI / 8) || (direction > 7 * Math.PI / 8 && direction <= 2 * Math.PI)) &&
                        (magnitude > gradientMagnitude[x + 1, y] || magnitude > gradientMagnitude[x - 1, y])
                    )
                    {
                        magnitude = 0;
                    }
                    else if ((direction > Math.PI / 8 && direction <= 3 * Math.PI / 8) &&
                             (magnitude > gradientMagnitude[x + 1, y - 1] ||
                              magnitude > gradientMagnitude[x - 1, y + 1]))

                    {
                        magnitude = 0;
                    }
                    else if ((direction > 3 * Math.PI / 8 && direction <= 5 * Math.PI / 8) &&
                             (magnitude > gradientMagnitude[x, y - 1] || magnitude > gradientMagnitude[x, y + 1]))
                    {
                        magnitude = 0;
                    }
                    else if ((direction > 5 * Math.PI / 8 && direction <= 7 * Math.PI / 8) &&
                             (magnitude > gradientMagnitude[x + 1, y + 1] ||
                              magnitude > gradientMagnitude[x - 1, y - 1]))
                    {
                        magnitude = 0;
                    }

                    pixel[0] = pixel[1] = pixel[2] = (byte)magnitude;

                    return new IntPtr(pixel);
                });

            nonMax.UnlockBits(data);

            return nonMax;
        }

        public unsafe Bitmap HysteresisThresholding(Bitmap image, int lowThreshold, int highThreshold)
        {
            int width = image.Width;
            int height = image.Height;

            Bitmap edgeImage = new Bitmap(width, height);

            var source = image.LockBitmapReadOnly(image.PixelFormat);
            var data = edgeImage.LockBitmapWriteOnly(edgeImage.PixelFormat).ExecuteOnPixels(
                (pixelPtr, scan0, stride, x, y) =>
                {
                    byte* pixel = (byte*)pixelPtr.ToPointer();
                    byte* sourcePixel = (byte*)source.GetPixel(x, y);
                    int intensity = sourcePixel[0];

                    if (intensity > highThreshold)
                    {
                        pixel[0] = pixel[1] = pixel[2] = 255;
                    }
                    else if (intensity < lowThreshold)
                    {
                        pixel[0] = pixel[1] = pixel[2] = 0;
                    }
                    else
                    {
                        bool isEdge = false;

                        for (int i = -1; i <= 1 && !isEdge; i++)
                        {
                            for (int j = -1; j <= 1 && !isEdge; j++)
                            {
                                if (x + j >= 0 && x + j < width && y + i >= 0 && y + i < height)
                                {
                                    byte* neighbor = (byte*)source.GetPixel(x + j, y + i).ToPointer();

                                    if (neighbor[0] > highThreshold)
                                    {
                                        isEdge = true;
                                    }
                                }
                            }
                        }

                        pixel[0] = pixel[1] = pixel[2] = isEdge ? (byte)255 : (byte)0;

                    }

                    return new IntPtr(pixel);
                });

            image.UnlockBits(source);
            edgeImage.UnlockBits(data);

            return edgeImage;
        }

        private double[,] PrepareKernel(double[,] kernel)
        {
            double sum = SumKernel(kernel);
            for (int i = 0; i < kernel.GetLength(0); i++)
            {
                for (int j = 0; j < kernel.GetLength(1); j++)
                {
                    kernel[i, j] /= sum;
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