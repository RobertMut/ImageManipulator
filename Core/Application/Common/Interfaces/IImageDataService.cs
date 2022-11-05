using System.Collections.Generic;

namespace ImageManipulator.Application.Common.Interfaces
{
    public interface IImageDataService
    {
        double[][] CalculateLevels(Avalonia.Media.Imaging.Bitmap bitmap);
        double[] CalculateLUT(double[] values);
        double[] CalculateLuminanceFromRGB(double[][] rgbDictionary);
        double[] CalculateBrightnessFromLuminance(double[] luminanceArray);
        Dictionary<string, double[]> StretchHistogram(Dictionary<string, double[]> existingHistogram, System.Drawing.Bitmap existingImage);
    }
}