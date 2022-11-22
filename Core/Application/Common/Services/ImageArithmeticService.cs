﻿using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Application.Common.Virtuals;
using ImageManipulator.Common.Enums;
using ImageManipulator.Domain.Common.Helpers;
using System;
using System.Drawing;

namespace ImageManipulator.Application.Common.Services
{
    public class ImageArithmeticService : ElementaryOperationServiceVirtual, IImageArithmeticService
    {
        public unsafe Bitmap Execute(Bitmap bitmap, object parameter, ArithmeticOperationType operationType) => base.Execute(bitmap, parameter, operationType);

        protected override IntPtr Calculate(IntPtr pixelData, IntPtr otherImagePixelData, Enum operationType) => operationType switch
        {
            ArithmeticOperationType.Add => pixelData.ExecuteOnPixel(otherImagePixelData, (current, other) => (byte)((current + other)%255)),
            ArithmeticOperationType.Average => pixelData.ExecuteOnPixel(otherImagePixelData, (current, other) => (byte)(((current + other) / 2)%255)),
            ArithmeticOperationType.SubtractLeft => pixelData.ExecuteOnPixel(otherImagePixelData, (current, other) => (byte)((current - other)%255)),
            ArithmeticOperationType.SubtractRight => pixelData.ExecuteOnPixel(otherImagePixelData, (current, other) => (byte)((other - current)%255)),
            ArithmeticOperationType.Difference => pixelData.ExecuteOnPixel(otherImagePixelData, (current, other) => (byte)Math.Abs(current - other)),
            ArithmeticOperationType.Multiply => pixelData.ExecuteOnPixel(otherImagePixelData, (current, other) => (byte)(((current / 255.0 * other / 255.0) * 255.0)%255)),
            ArithmeticOperationType.Min => pixelData.ExecuteOnPixel(otherImagePixelData, (current, other) => current < other ? current : other),
            ArithmeticOperationType.Max => pixelData.ExecuteOnPixel(otherImagePixelData, (current, other) => current > other ? current : other),
            ArithmeticOperationType.Amplitude => pixelData.ExecuteOnPixel(otherImagePixelData, (current, other) => (byte)(Math.Sqrt(current * current + other * other) / Math.Sqrt(2.0))),
            _ => throw new Exception("Operation not found!")
        };
    }
}