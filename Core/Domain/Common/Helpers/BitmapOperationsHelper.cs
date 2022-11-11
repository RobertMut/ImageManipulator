using ImageManipulator.Domain.Common.Dictionaries;
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

        public unsafe static byte* ExecuteOnData(this IntPtr data,
            IntPtr scan0,
            int stride,
            Func<IntPtr, IntPtr, int, IntPtr> func) => (byte*)func(data, scan0, stride);

        public unsafe static byte* ExecuteOnData(this IntPtr data,
            IntPtr scan0,
            int stride,
            int x,
            int y,
            Func<IntPtr, IntPtr, int, int, int, IntPtr> func) => (byte*)func(data, scan0, stride, x, y);

        public unsafe static BitmapData ExecuteOnPixel(this BitmapData data, Func<IntPtr, IntPtr, int, IntPtr> func)
        {
            var pixelDataFunc = ImageXYCoordinatesDictionary.Formula[data.PixelFormat];
            byte* startPoint = (byte*)data.Scan0;

            Parallel.For(0, data.Height, i =>
            {
                for (int j = 0; j < data.Width; j++)
                {
                    byte* pixelData = (byte*)pixelDataFunc(data.Scan0, data.Stride, j, i);

                    ((IntPtr)pixelData).ExecuteOnData(data.Scan0, data.Stride, func);
                }
            });

            return data;
        }

        public unsafe static BitmapData ExecuteOnPixel(this BitmapData data, Func<IntPtr, IntPtr, int, int, int, IntPtr> func)
        {
            var pixelDataFunc = ImageXYCoordinatesDictionary.Formula[data.PixelFormat];
            int bitsPerPixel = Image.GetPixelFormatSize(data.PixelFormat);

            Parallel.For(0, data.Height, i =>
            {
                for (int j = 0; j < data.Width; j++)
                {
                    byte* pixelData = (byte*)pixelDataFunc(data.Scan0, data.Stride, j, i);

                    ((IntPtr)pixelData).ExecuteOnData(data.Scan0, data.Stride, j, i, func);
                }
            });

            return data;
        }

        public unsafe static int GetPixel(this BitmapData data, int x, int y, int colorIndex)
        {
            var pixelDataFunc = ImageXYCoordinatesDictionary.Formula[data.PixelFormat];

            return ((byte*)pixelDataFunc(data.Scan0, data.Stride, x, y))[colorIndex];
        }
    }
}