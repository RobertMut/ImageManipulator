using System;
using System.Drawing;

namespace ImageManipulator.Application.Common.Interfaces
{
    public interface IImageConvolutionService
    {
        Bitmap Execute(Bitmap bitmap, double[,] kernel, double factor, bool soften = false);
        unsafe double[,] ComputeGradient(Bitmap bitmap, Func<double, double, double> func);
        unsafe Bitmap NonMaxSupression(double[,] gradientMagnitude, double[,] gradientDirection);
        unsafe Bitmap HysteresisThresholding(Bitmap image, int lowThreshold, int highThreshold);
    }
}