namespace ImageManipulator.Application.Common.Interfaces
{
    public interface IImageDataService
    {
        double[][] CalculateLevels(Avalonia.Media.Imaging.Bitmap bitmap);
        double[] CalculateLuminanceFromRGB(double[][] rgbDictionary);
        double[] CalculateBrightnessFromLuminance(double[] luminanceArray);
        double[] CalculateAverageForGrayGraph(double[][] levels);
        double[][] StretchHistogram(double[][] values, System.Drawing.Bitmap existingImage);
    }
}