using Avalonia.Media.Imaging;

namespace ImageManipulator.Application.Common.Interfaces
{
    public interface IImageDataService
    {
        int[]?[] CalculateLevels(Bitmap? bitmap);
        int[]? CalculateAverageForGrayGraph(int[]?[] levels);
        int[]?[] GetHistogramValues(int[]?[] values, System.Drawing.Bitmap? existingImage);
    }
}