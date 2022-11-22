using ImageManipulator.Common.Enums;
using System;
using System.Drawing;

namespace ImageManipulator.Application.Common.Interfaces
{
    public interface IImageBitwiseService
    {
        Bitmap Execute(Bitmap bitmap, object parameter, BitwiseOperationType operationType);
    }
}