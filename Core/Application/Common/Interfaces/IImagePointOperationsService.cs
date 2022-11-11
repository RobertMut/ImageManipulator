using System.Drawing;

namespace ImageManipulator.Application.Common.Interfaces
{
    public interface IImagePointOperationsService
    {
        int CalculateLowerImageThresholdPoint(double[] histogram = null);
        int CalculateUpperImageThresholdPoint(double[] histogram = null);
        Bitmap StretchContrast(Bitmap bitmap, int lowest, int highest);
        Bitmap NonLinearlyStretchContrast(Bitmap bitmap, double gamma);
        Bitmap HistogramEqualization(Bitmap bitmap, double[][] lut);
        Bitmap Negation(Bitmap bitmap);
        Bitmap Thresholding(Bitmap bitmap, int threshold, bool replace = true);
        Bitmap MultiThresholding(Bitmap bitmap, int lowerThreshold, int upperThreshold, bool replace = true);
    }
}