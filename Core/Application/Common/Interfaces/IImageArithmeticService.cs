using ImageManipulator.Common.Enums;
using System;
using System.Drawing;

namespace ImageManipulator.Application.Common.Interfaces
{
    public interface IImageArithmeticService
    {
        Bitmap Execute(Bitmap bitmap, object parameter, ArithmeticOperationType operationType);
    }
}