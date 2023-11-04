using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using ImageManipulator.Domain.Common.Helpers;

namespace ImageManipulator.Domain.Common.Virtuals
{
    public abstract class ElementaryOperationServiceVirtual
    {
        protected Bitmap? Execute(Bitmap? bitmap, object? parameter, Enum operationType) => parameter switch
        {
            Bitmap anotherImage => ExecuteWithImage(bitmap, anotherImage, operationType),
            int value => ExecuteWithColor(bitmap, new[] { (byte)value, (byte)value, (byte)value }, operationType),
            Avalonia.Media.Color color => ExecuteWithColor(bitmap, new[] { color.R, color.G, color.B }, operationType),
            _ => throw new Exception($"Invalid parameter type {parameter.GetType().FullName}")
        };

        protected abstract IntPtr Calculate(IntPtr pixelData, IntPtr otherImagePixelData, Enum operationType);

        private unsafe Bitmap ExecuteWithImage(Bitmap? bitmap, Bitmap anotherBitmap, Enum operationType)
        {
            var newBitmap = new Bitmap(bitmap);
            var otherImageData = anotherBitmap.LockBitmap(anotherBitmap.PixelFormat, ImageLockMode.ReadOnly);

            var newBitmapData = newBitmap.LockBitmap(newBitmap.PixelFormat, ImageLockMode.ReadWrite)
                .ExecuteOnPixels((x, scan0, stride, i, j) =>
                {
                    byte* pixelData = (byte*)x.ToPointer();
                    byte* otherImagePixelData = (byte*)otherImageData.GetPixel(i, j).ToPointer();
                    
                    Calculate((IntPtr)pixelData, (IntPtr)otherImagePixelData, operationType);
                });

            anotherBitmap.UnlockBits(otherImageData);
            newBitmap.UnlockBits(newBitmapData);

            return newBitmap;
        }

        private unsafe Bitmap ExecuteWithColor(Bitmap? bitmap, byte[] colorArr, Enum operationType)
        {
            IntPtr unmanagedColorPointer = Marshal.AllocHGlobal(colorArr.Length);
            Marshal.Copy(colorArr, 0, unmanagedColorPointer, colorArr.Length);

            var sourceData = bitmap.LockBitmap(bitmap.PixelFormat, ImageLockMode.ReadWrite)
                .ExecuteOnPixels((x, scan0, stride, i, j) =>
                {
                    byte* pixelData = (byte*)x.ToPointer();

                    Calculate((IntPtr)pixelData, unmanagedColorPointer, operationType);
                });

            bitmap.UnlockBits(sourceData);
            Marshal.FreeHGlobal(unmanagedColorPointer);

            return bitmap;
        }
    }
}
