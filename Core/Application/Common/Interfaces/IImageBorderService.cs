using ImageManipulator.Common.Enums;
using System.Drawing;

namespace ImageManipulator.Application.Common.Interfaces
{
    public interface IImageBorderService
    {
        Bitmap Execute(Bitmap bitmap, ImageWrapEnum wrapEnum, int top, int bottom, int left, int right, Color color = default);
    }
}