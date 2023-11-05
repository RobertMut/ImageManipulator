using ImageManipulator.Common.Enums;
using System;
using System.Drawing;
using ImageManipulator.Domain.Common.Helpers;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Domain.Common.Virtuals;

namespace ImageManipulator.Application.Common.Services
{
    public class ImageBitwiseService : ElementaryOperationServiceVirtual, IImageBitwiseService
    {
        public Bitmap? Execute(Bitmap? bitmap, object? parameter, BitwiseOperationType operationType) => base.Execute(bitmap, parameter, operationType);

        protected override IntPtr Calculate(IntPtr pixelData, IntPtr otherImagePixelData, Enum operationType)
        => operationType switch
        {
            BitwiseOperationType.AND => pixelData.ExecuteOnPixel(otherImagePixelData, (current, other) => (byte)(current & other)),
            BitwiseOperationType.OR => pixelData.ExecuteOnPixel(otherImagePixelData, (current, other) => (byte)(current | other)),
            BitwiseOperationType.XOR => pixelData.ExecuteOnPixel(otherImagePixelData, (current, other) => (byte)(current ^ other)),
            BitwiseOperationType.NOT => pixelData.ExecuteOnPixel(otherImagePixelData, (current, _) => (byte)~current),
            BitwiseOperationType.LeftShift => pixelData.ExecuteOnPixel(otherImagePixelData, (current, other) => (byte)(current << other)),
            BitwiseOperationType.RightShift => pixelData.ExecuteOnPixel(otherImagePixelData, (current, other) => (byte)(current >> other)),
            _ => throw new Exception("Operation not found!")
        };
    }
}
