using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ImageManipulator.Domain.Common.Helpers
{
    public static class BitmapOperationsHelper
    {
        public static BitmapData LockBitmap(this Bitmap bitmap, PixelFormat pixelFormat) => bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, pixelFormat);

        public unsafe static byte* ExecuteOnData(this IntPtr data, IntPtr scan0, int stride, Func<IntPtr, IntPtr, int, IntPtr> func) => (byte*)func(data, scan0, stride);

        public unsafe static BitmapData ExecuteOnPixel(this BitmapData data, Func<IntPtr, IntPtr, int, IntPtr> func)
        {
            byte bitsPerPixel = (byte)Image.GetPixelFormatSize(data.PixelFormat);
            byte* startPoint = (byte*)data.Scan0.ToPointer();

            Parallel.For(0, data.Height, i =>
            {
                for (int j = 0; j < data.Width; j++)
                {
                    byte* pixelData = startPoint + i * data.Stride + j * bitsPerPixel / 8;

                    ((IntPtr)pixelData).ExecuteOnData(data.Scan0, data.Stride, func);
                }
            });

            return data;
        }
    }
}