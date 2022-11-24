using ImageManipulator.Common.Enums;
using System.Runtime.InteropServices;
using System;
using System.Drawing;
using ImageManipulator.Domain.Common.Helpers;
using ImageManipulator.Application.Common.Virtuals;
using ImageManipulator.Application.Common.Interfaces;

namespace ImageManipulator.Application.Common.Services
{
    public class ImageBitwiseService : ElementaryOperationServiceVirtual, IImageBitwiseService
    {
        public unsafe Bitmap Execute(Bitmap bitmap, object parameter, BitwiseOperationType operationType) => base.Execute(bitmap, parameter, operationType);

        protected override IntPtr Calculate(IntPtr pixelData, IntPtr otherImagePixelData, Enum operationType)
        => operationType switch
        {
            BitwiseOperationType.AND => pixelData.ExecuteOnPixel(otherImagePixelData, (current, other) => (byte)((current & other)%255)),
            BitwiseOperationType.OR => pixelData.ExecuteOnPixel(otherImagePixelData, (current, other) => (byte)((current | other)%255)),
            BitwiseOperationType.XOR => pixelData.ExecuteOnPixel(otherImagePixelData, (current, other) => (byte)((current ^ other)%255)),
            BitwiseOperationType.NOT => pixelData.ExecuteOnPixel(otherImagePixelData, (current, other) => (byte)(~current)),
            BitwiseOperationType.LeftShift => pixelData.ExecuteOnPixel(otherImagePixelData, (current, other) => (byte)(current << other)),
            BitwiseOperationType.RightShift => pixelData.ExecuteOnPixel(otherImagePixelData, (current, other) => (byte)(current >> other)),
            _ => throw new Exception("Operation not found!")
        };
    }
}
