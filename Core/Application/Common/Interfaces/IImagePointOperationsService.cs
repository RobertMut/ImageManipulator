using System.Drawing;

namespace ImageManipulator.Application.Common.Interfaces
{
    public interface IImagePointOperationsService
    {
        int CalculateLowerImageThresholdPoint(double[] histogram = null);
        int CalculateUpperImageThresholdPoint(double[] histogram = null);
        Bitmap StretchContrast(Bitmap bitmap, int lowest, int highest);
        Bitmap NonLinearlyStretchContrast(Bitmap bitmap, double gamma);
        Bitmap HistogramEqualization(Bitmap bitmap);
        Bitmap Negation(Bitmap bitmap);
        Bitmap Tresholding(Bitmap bitmap, int treshold);
    }
}