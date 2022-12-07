using ImageManipulator.Common.Enums;
using System.Drawing;

namespace ImageManipulator.Application.Common.Interfaces
{
    public interface IImageBitwiseService
    {
        Bitmap Execute(Bitmap bitmap, object parameter, BitwiseOperationType operationType);
    }
}