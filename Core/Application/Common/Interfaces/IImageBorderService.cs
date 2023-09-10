using ImageManipulator.Common.Enums;
using System.Drawing;

namespace ImageManipulator.Application.Common.Interfaces
{
    public interface IImageBorderService
    {
        Bitmap Execute(Bitmap bitmap, ImageWrapType wrapType, int top, int bottom, int left, int right, Color color = default);
    }
}