using System.Drawing;

namespace ImageManipulator.Application.Common.Interfaces
{
    public interface IImageDataService
    {
        int[]?[] CalculateLevels(Bitmap? bitmap);
        int[]? CalculateAverageForGrayGraph(int[]?[] levels);
    }
}