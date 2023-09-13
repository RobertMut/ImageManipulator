using System;
using System.Drawing;

namespace ImageManipulator.Application.Common.Interfaces
{
    public interface IImageConvolutionService
    {
        Bitmap Execute(Bitmap bitmap, double[,] kernel, bool soften = false);
        double[,] ComputeGradient(Bitmap bitmap, Func<double, double, double> func);
        Bitmap HysteresisThresholding(int width, int height, int lowThreshold, int highThreshold,
            double[,] gradientMagnitude);
    }
}