using System.Drawing;

namespace ImageManipulator.Application.Common.Interfaces
{
    public interface IImageConvolutionService
    {
        Bitmap Execute(Bitmap bitmap, double[,] kernel, double factor, bool soften = false);
    }
}