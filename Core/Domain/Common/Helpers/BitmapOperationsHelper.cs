using ImageManipulator.Domain.Common.Dictionaries;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace ImageManipulator.Domain.Common.Helpers
{
    public static class BitmapOperationsHelper
    {
        public static BitmapData LockBitmap(this Bitmap? bitmap, PixelFormat pixelFormat, ImageLockMode lockMode) =>
            bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), lockMode, pixelFormat);

        private static void ExecuteOnData(this IntPtr data,
            IntPtr scan0,
            int stride,
            Action<IntPtr, IntPtr, int> action) => action(data, scan0, stride);

        /// <summary>
        /// Iterates through image
        /// </summary>
        /// <param name="data">Pixel data</param>
        /// <param name="scan0">First pixel data on image</param>
        /// <param name="stride">Stride width</param>
        /// <param name="x">X of pixel</param>
        /// <param name="y">Y of pixel</param>
        /// <param name="action">Func to edit</param>
        /// <returns></returns>
        private static void ExecuteOnData(this IntPtr data,
            IntPtr scan0,
            int stride,
            int x,
            int y,
            Action<IntPtr, IntPtr, int, int, int> action) => action(data, scan0, stride, x, y);

        /// <summary>
        /// Executes on bitmapData iterating through pixels
        /// </summary>
        /// <param name="data">Bitmap data</param>
        /// <param name="action">Func to execute on pixel</param>
        /// <returns>BitmapData</returns>
        public static unsafe BitmapData ExecuteOnPixels(this BitmapData data, Action<IntPtr, IntPtr, int> action)
        {
            var pixelDataFunc = ImageXYCoordinatesDictionary.PixelData[data.PixelFormat];

            Parallel.For(0, data.Height, i =>
            {
                for (int j = 0; j < data.Width; j++)
                {
                    byte* pixelData = (byte*)pixelDataFunc(data.Scan0, data.Stride, j, i);

                    ((IntPtr)pixelData).ExecuteOnData(data.Scan0, data.Stride, action);
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
        public static unsafe BitmapData ExecuteOnPixels(this BitmapData data,
            Action<IntPtr, IntPtr, int, int, int> func)
        {
            var pixelDataFunc = ImageXYCoordinatesDictionary.PixelData[data.PixelFormat];

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
        /// <param name="action">Func to execute on pixel</param>
        /// <returns>BitmapData</returns>
        public static unsafe BitmapData ExecuteOnPixels(this BitmapData data, int offsetX, int targetX, int offsetY,
            int targetY, Action<IntPtr, IntPtr, int, int, int> action)
        {
            var pixelDataFunc = ImageXYCoordinatesDictionary.PixelData[data.PixelFormat];

            Parallel.For(offsetY, targetY, i =>
            {
                for (int j = offsetX; j < targetX; j++)
                {
                    byte* pixelData = (byte*)pixelDataFunc(data.Scan0, data.Stride, j, i);

                    ((IntPtr)pixelData).ExecuteOnData(data.Scan0, data.Stride, j, i, action);
                }
            });

            return data;
        }

        public static unsafe IntPtr ExecuteOnPixel(this IntPtr pixelData, IntPtr otherPixel,
            Func<byte, byte, byte> func)
        {
            var pixelBytePointer = (byte*)pixelData.ToPointer();
            var otherPixelBytePointer = (byte*)otherPixel.ToPointer();

            pixelBytePointer[0] = func(pixelBytePointer[0], otherPixelBytePointer[0]);
            pixelBytePointer[1] = func(pixelBytePointer[1], otherPixelBytePointer[1]);
            pixelBytePointer[2] = func(pixelBytePointer[2], otherPixelBytePointer[2]);

            return (IntPtr)pixelBytePointer;
        }

        public static IntPtr GetPixel(this BitmapData data, int x, int y) =>
            ImageXYCoordinatesDictionary.PixelData[data.PixelFormat](data.Scan0, data.Stride, x, y);
    }
}