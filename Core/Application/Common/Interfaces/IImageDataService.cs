using System.Collections.Generic;

namespace ImageManipulator.Application.Common.Interfaces
{
    public interface IImageDataService
    {
        Dictionary<string, double[]> CalculateHistogramForImage(Avalonia.Media.Imaging.Bitmap bitmap);
        double[] CalculateLUT(double[] values);
        double[] CalculateLuminanceFromRGB(Dictionary<string, double[]> rgbDictionary);
        double[] CalculateBrightnessFromLuminance(double[] luminanceArray);
    }
}