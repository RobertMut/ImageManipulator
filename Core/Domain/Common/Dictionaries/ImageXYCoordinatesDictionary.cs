using System;
using System.Collections.Generic;
using System.Drawing.Imaging;

namespace ImageManipulator.Domain.Common.Dictionaries
{
    public static class ImageXYCoordinatesDictionary
    {
        public unsafe static Dictionary<PixelFormat, Func<IntPtr, int, int, int, IntPtr>> Formula = new Dictionary<PixelFormat, Func<IntPtr, int, int, int, IntPtr>>()
        {
            { PixelFormat.Format32bppArgb, (scan0, stride, x, y) => (IntPtr)(byte*)(scan0+(y*stride)+(x*4)) },
            { PixelFormat.Format24bppRgb, (scan0, stride, x, y) => (IntPtr)(byte*)(scan0+(y*stride)+(x*3)) },
            { PixelFormat.Format8bppIndexed, (scan0, stride, x, y) => (IntPtr)(byte*)(scan0+(y*stride)+x) },
            { PixelFormat.Format4bppIndexed, (scan0, stride, x, y) => (IntPtr)(byte*)(scan0+(y*stride)+(x/2)) },
            { PixelFormat.Format1bppIndexed, (scan0, stride, x, y) => (IntPtr)(byte*)(scan0+(y*stride)+(x/8)) }
        };
    }
}
