using System;
using System.Collections.Generic;
using System.Drawing.Imaging;

namespace ImageManipulator.Domain.Common.Dictionaries
{
    public static class ImageXYCoordinatesDictionary
    {
        public unsafe static Dictionary<PixelFormat, Func<IntPtr, int, int, int, IntPtr>> PixelData = new Dictionary<PixelFormat, Func<IntPtr, int, int, int, IntPtr>>()
        {
            { PixelFormat.Format32bppArgb, (scan0, stride, x, y) => (IntPtr)(byte*)(scan0+(y*stride)+(x*4)) },
            { PixelFormat.Format24bppRgb, (scan0, stride, x, y) => (IntPtr)(byte*)(scan0+(y*stride)+(x*3)) },
            { PixelFormat.Format8bppIndexed, (scan0, stride, x, y) => (IntPtr)(byte*)(scan0+(y*stride)+x) },
            { PixelFormat.Format4bppIndexed, (scan0, stride, x, y) => (IntPtr)(byte*)(scan0+(y*stride)+(x/2)) },
            { PixelFormat.Format1bppIndexed, (scan0, stride, x, y) => (IntPtr)(byte*)(scan0+(y*stride)+(x/8)) }
        };

        public unsafe static Dictionary<PixelFormat, Func<int, int, int, int>> ByteOffset = new Dictionary<PixelFormat, Func<int, int, int, int>>
        {
            { PixelFormat.Format32bppArgb, (stride, x, y) => (y*stride)+(x*4) },
            { PixelFormat.Format24bppRgb, (stride, x, y) => (y*stride)+(x*3) },
            { PixelFormat.Format8bppIndexed, (stride, x, y) => (y*stride)+x },
            { PixelFormat.Format4bppIndexed, (stride, x, y) => (y*stride)+(x/2) },
            { PixelFormat.Format1bppIndexed, (stride, x, y) => (y*stride)+(x/8) }
        };

        public unsafe static Dictionary<PixelFormat, Func<int, int, int, int, int>> CalculationOffset = new Dictionary<PixelFormat, Func<int, int, int, int, int>>
        {
            { PixelFormat.Format32bppArgb, (byteOffset, stride, x, y) => byteOffset+(y*stride)+(x*4) },
            { PixelFormat.Format24bppRgb,(byteOffset, stride, x, y) => byteOffset+(y*stride)+(x*3) },
            { PixelFormat.Format8bppIndexed, (byteOffset, stride, x, y) => byteOffset+(y*stride)+x },
            { PixelFormat.Format4bppIndexed, (byteOffset, stride, x, y) => byteOffset+(y*stride)+(x/2) },
            { PixelFormat.Format1bppIndexed, (byteOffset, stride, x, y) => byteOffset+(y*stride)+(x/8) }
        };
    }
}
