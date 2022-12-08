using ImageManipulator.Domain.Common.Dictionaries;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace ImageManipulator.Domain.Common.Helpers
{
    public static class BitmapOperationsHelper
    {
        public static BitmapData LockBitmapReadOnly(this Bitmap bitmap, PixelFormat pixelFormat) => bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, pixelFormat);
        public static BitmapData LockBitmapWriteOnly(this Bitmap bitmap, PixelFormat pixelFormat) => bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, pixelFormat);

        public unsafe static byte* ExecuteOnData(this IntPtr data,
            IntPtr scan0,
            int stride,
            Func<IntPtr, IntPtr, int, IntPtr> func) => (byte*)func(data, scan0, stride);

        /// <summary>
        /// Iterates through image
        /// </summary>
        /// <param name="data">Pixel data</param>
        /// <param name="scan0">First pixel data on image</param>
        /// <param name="stride">Stride width</param>
        /// <param name="x">X of pixel</param>
        /// <param name="y">Y of pixel</param>
        /// <param name="func">Func to edit</param>
        /// <returns></returns>
        public unsafe static byte* ExecuteOnData(this IntPtr data,
            IntPtr scan0,
            int stride,
            int x,
            int y,
            Func<IntPtr, IntPtr, int, int, int, IntPtr> func) => (byte*)func(data, scan0, stride, x, y);

        /// <summary>
        /// Executes on bitmapData iterating through pixels
        /// </summary>
        /// <param name="data">Bitmap data</param>
        /// <param name="func">Func to execute on pixel</param>
        /// <returns>BitmapData</returns>
        public unsafe static BitmapData ExecuteOnPixels(this BitmapData data, Func<IntPtr, IntPtr, int, IntPtr> func)
        {
            var pixelDataFunc = ImageXYCoordinatesDictionary.PixelData[data.PixelFormat];
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

        /// <summary>
        /// Executes on bitmapData iterating through pixels
        /// </summary>
        /// <param name="data">Bitmap data</param>
        /// <param name="func">Func to execute on pixel</param>
        /// <returns>BitmapData</returns>
        public unsafe static BitmapData ExecuteOnPixels(this BitmapData data, Func<IntPtr, IntPtr, int, int, int, IntPtr> func)
        {
            var pixelDataFunc = ImageXYCoordinatesDictionary.PixelData[data.PixelFormat];
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

        /// <summary>
        /// Executes on bitmapData iterating through pixels
        /// </summary>
        /// <param name="data">Bitmap data</param>
        /// <param name="offsetX">Offset x to begin iterating</param>
        /// <param name="targetX">Target pixel to iterate to</param>
        /// <param name="offsetY">Offset y to begin iterating</param>
        /// <param name="targetY">Target y to iterate to</param>
        /// <param name="func">Func to execute on pixel</param>
        /// <returns>BitmapData</returns>
        public unsafe static BitmapData ExecuteOnPixels(this BitmapData data, int offsetX, int targetX, int offsetY, int targetY, Func<IntPtr, IntPtr, int, int, int, IntPtr> func)
        {
            var pixelDataFunc = ImageXYCoordinatesDictionary.PixelData[data.PixelFormat];
            byte* startPoint = (byte*)data.Scan0;

            Parallel.For(offsetY, targetY, i =>
            {
                for (int j = offsetX; j < targetX; j++)
                {
                    byte* pixelData = (byte*)pixelDataFunc(data.Scan0, data.Stride, j, i);

                    ((IntPtr)pixelData).ExecuteOnData(data.Scan0, data.Stride, j, i, func);
                }
            });

            return data;
        }

        public unsafe static IntPtr ExecuteOnPixel(this IntPtr pixelData, IntPtr otherPixel, Func<byte, byte, byte> func)
        {
            var pixelBytePointer = (byte*)pixelData.ToPointer();
            var otherPixelBytePointer = (byte*)otherPixel.ToPointer();

            pixelBytePointer[0] = func(pixelBytePointer[0], otherPixelBytePointer[0]);
            pixelBytePointer[1] = func(pixelBytePointer[1], otherPixelBytePointer[1]);
            pixelBytePointer[2] = func(pixelBytePointer[2], otherPixelBytePointer[2]);

            return (IntPtr)pixelBytePointer;
        }

        public unsafe static IntPtr GetPixel(this BitmapData data, int x, int y) => ImageXYCoordinatesDictionary.PixelData[data.PixelFormat](data.Scan0, data.Stride, x, y);
    }
}