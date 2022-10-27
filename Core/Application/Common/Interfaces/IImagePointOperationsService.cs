namespace ImageManipulator.Application.Common.Interfaces
{
    public interface IImagePointOperationsService
    {
        int CalculateLowerImageThresholdPoint(double[] histogram = null);
        int CalculateUpperImageThresholdPoint(double[] histogram = null);
        System.Drawing.Bitmap StretchContrast(System.Drawing.Bitmap bitmap, int lowest, int highest);
    }
}