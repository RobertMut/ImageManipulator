using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Application.Common.Models;
using ImageManipulator.Domain.Common.Helpers;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace ImageManipulator.Application.Common.Services
{
    public class ImageConvolutionService : IImageConvolutionService
    {
        private static readonly int[,] KernelX =
        {
            { -1, 0, 1 },
            { -2, 0, 2 },
            { -1, 0, 1 }
        };

        private static readonly int[,] KernelY =
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
                .LockBitmap(PixelFormat.Format32bppArgb, ImageLockMode.ReadWrite);

            BitmapData affectedData = newBitmap
                .LockBitmap(PixelFormat.Format32bppArgb, ImageLockMode.ReadWrite);

            for (int y = 0; y < newBitmap.Height; y++)
            {
                for (int x = 0; x < newBitmap.Width; x++)
                {
                    var color = new ColorDouble(0, 0, 0);

                    for (int kernelY = 0; kernelY < kernelHeight; kernelY++)
                    {
                        int kernelYRadius = kernelY - radiusY;
                        int offsetY = Math.Clamp(y + kernelYRadius, 0, affectedData.Height - 1);

                        for (int kernelX = 0; kernelX < kernelWidth; kernelX++)
                        {
                            int kernelXRadius = kernelX - radiusX;
                            int offsetX = Math.Clamp(x + kernelXRadius, 0, affectedData.Width - 1);

                            byte* offsetPixel = (byte*)sourceData.GetPixel(offsetX, offsetY).ToPointer();

                            color.Red += kernel[kernelY, kernelX] * offsetPixel[0];
                            color.Green += kernel[kernelY, kernelX] * offsetPixel[1];
                            color.Blue += kernel[kernelY, kernelX] * offsetPixel[2];
                        }
                    }

                    byte* pixel = (byte*)affectedData.GetPixel(x, y).ToPointer();
                    
                    pixel[0] = CalculationHelper.HandleValueOutsideBounds(color.Red);
                    pixel[1] = CalculationHelper.HandleValueOutsideBounds(color.Green);
                    pixel[2] = CalculationHelper.HandleValueOutsideBounds(color.Blue);
                }
            }

            newBitmap.UnlockBits(affectedData);
            bitmap.UnlockBits(sourceData);

            return newBitmap;
        }

        public unsafe double[,] ComputeGradient(Bitmap bitmap, Func<double, double, double> func)
        {
            double[,] gradient = new double[bitmap.Width, bitmap.Height];
            var bitmapData = bitmap.LockBitmap(bitmap.PixelFormat, ImageLockMode.ReadWrite);

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

                            gx += intensity * KernelX[i + 1, j + 1];
                            gy += intensity * KernelY[i + 1, j + 1];
                        }
                    }

                    gradient[x, y] = func(gx, gy);
                }
            }

            bitmap.UnlockBits(bitmapData);

            return gradient;
        }

        public double[,] NonMaxSupression(double[,] gradientMagnitude, double[,] gradientDirection)
        {
            int width = gradientMagnitude.GetLength(0);
            int height = gradientMagnitude.GetLength(1);

            var preparedDirectionsInAngle = PrepareDirections(gradientDirection);
            
            for (int x = 1; x < width - 1; x++)
            {
                for (int y = 1; y < height - 1; y++)
                {
                    double magnitude = gradientMagnitude[x, y];
                    switch (preparedDirectionsInAngle[x, y])
                    {
                        case 0:
                            if (magnitude < gradientMagnitude[x, y - 1] && magnitude < gradientMagnitude[x, y + 1])
                            {
                                gradientMagnitude[x - 1, y - 1] = 0;
                            }
                            break;
                        case 45:
                            if (magnitude < gradientMagnitude[x - 1, y + 1] && magnitude < gradientMagnitude[x + 1, y - 1])
                            {
                                gradientMagnitude[x - 1, y - 1] = 0;
                            }
                            break;
                        case 90:
                            if (magnitude < gradientMagnitude[x - 1, y] && magnitude < gradientMagnitude[x + 1, y])
                            {
                                gradientMagnitude[x - 1, y - 1] = 0;
                            }
                            break;
                        case 135:
                            if (magnitude < gradientMagnitude[x - 1, y - 1] && magnitude < gradientMagnitude[x + 1, y + 1])
                            {
                                gradientMagnitude[x - 1, y - 1] = 0;
                            }
                            break;
                    }
                }
            }

            return gradientMagnitude;
        }

        public unsafe Bitmap HysteresisThresholding(int width, int height, int lowThreshold, int highThreshold, double[,] gradientMagnitude)
        {
            Bitmap edgeImage = new Bitmap(width, height);

            using (Graphics graphics = Graphics.FromImage(edgeImage))
            using (var brush = new SolidBrush(Color.Black))
            {
                graphics.FillRectangle(brush, 0, 0, width, height);
                graphics.Save();
            }

            var data = edgeImage.LockBitmap(edgeImage.PixelFormat, ImageLockMode.ReadWrite)
                .ExecuteOnPixels(0, width,0 , height,
                    (pixelPtr, _, _, x, y) =>
                    {
                        byte* pixel = (byte*)pixelPtr.ToPointer();
                        double magnitude = gradientMagnitude[x, y];

                        if (magnitude >= highThreshold)
                        {
                            pixel[0] = 255;
                            pixel[1] = 255;
                            pixel[2] = 255;
                        }
                        else if (magnitude < lowThreshold)
                        {
                            pixel[0] = 0;
                            pixel[1] = 0;
                            pixel[2] = 0;
                        }
                        else
                        {
                            bool isEdge = false;

                            for (int i = -1; i <= 1; i++)
                            {
                                for (int j = -1; j <= 1; j++)
                                {
                                    if (x + j >= 0 && x + j < width && y + i >= 0 && y + i < height && gradientMagnitude[x + j, y + i] > highThreshold)
                                    {
                                        isEdge = true;
                                    }
                                }
                            }

                            pixel[0] = pixel[1] = pixel[2] = isEdge ? (byte)255 : (byte)0;
                        }
                    });

            edgeImage.UnlockBits(data);

            return edgeImage;
        }

        private double[,] PrepareDirections(double[,] gradientDirection)
        {
            Parallel.For(0, gradientDirection.GetLength(0), x =>
            {
                for (int y = 0; y < gradientDirection.GetLength(1); y++)
                {
                    double direction = gradientDirection[x, y];

                    if (direction < 0)
                    {
                        direction += 360;
                    }

                    gradientDirection[x, y] = GetDegreeForGradient(direction);
                }
            });

            return gradientDirection;
        }

        private static double GetDegreeForGradient(double direction) =>
            direction switch
            {
                <= 22.5 or >= 157.5 and <= 202.5 or >= 337.5 => 0,
                >= 22.5 and <= 67.5 or >= 202.5 and <= 247.5 => 45,
                >= 67.5 and <= 112.5 or >= 247.5 and <= 292.5 => 90,
                _ => 135
            };

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