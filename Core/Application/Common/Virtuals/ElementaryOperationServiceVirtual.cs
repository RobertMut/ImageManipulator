using ImageManipulator.Domain.Common.Helpers;
using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ImageManipulator.Application.Common.Virtuals
{
    public abstract class ElementaryOperationServiceVirtual
    {
        public virtual unsafe Bitmap Execute(Bitmap bitmap, object parameter, Enum operationType) => parameter switch
        {
            Bitmap anotherImage => ExecuteWithImage(bitmap, anotherImage, operationType),
            int value => ExecuteWithColor(bitmap, new[] { (byte)value, (byte)value, (byte)value }, operationType),
            Avalonia.Media.Color color => ExecuteWithColor(bitmap, new[] { color.R, color.G, color.B }, operationType),
            _ => throw new Exception($"Invalid parameter type {parameter.GetType().FullName}")
        };

        protected abstract IntPtr Calculate(IntPtr pixelData, IntPtr otherImagePixelData, Enum operationType);

        private unsafe Bitmap ExecuteWithImage(Bitmap bitmap, Bitmap anotherBitmap, Enum operationType)
        {
            var newBitmap = new Bitmap(bitmap);
            var otherImageData = anotherBitmap.LockBitmap(anotherBitmap.PixelFormat);

            var newBitmapData = newBitmap.LockBitmap(newBitmap.PixelFormat)
                .ExecuteOnPixels((x, scan0, stride, i, j) =>
                {
                    byte* pixelData = (byte*)x.ToPointer();
                    byte* otherImagePixelData = (byte*)otherImageData.GetPixel(i, j).ToPointer();

                    return Calculate((IntPtr)pixelData, (IntPtr)otherImagePixelData, operationType);
                });

            anotherBitmap.UnlockBits(otherImageData);
            newBitmap.UnlockBits(newBitmapData);

            return newBitmap;
        }

        private unsafe Bitmap ExecuteWithColor(Bitmap bitmap, byte[] colorArr, Enum operationType)
        {
            IntPtr unmanagedColorPointer = Marshal.AllocHGlobal(colorArr.Length);
            Marshal.Copy(colorArr, 0, unmanagedColorPointer, colorArr.Length);

            var sourceData = bitmap.LockBitmap(bitmap.PixelFormat)
                .ExecuteOnPixels((x, scan0, stride, i, j) =>
                {
                    byte* pixelData = (byte*)x.ToPointer();
                    var test = (byte*)unmanagedColorPointer.ToPointer();
                    var t1 = test[0];
                    var t2 = test[1];
                    var t3 = test[3];

                    return Calculate((IntPtr)pixelData, unmanagedColorPointer, operationType);
                });

            bitmap.UnlockBits(sourceData);
            Marshal.FreeHGlobal(unmanagedColorPointer);

            return bitmap;
        }
    }
}
