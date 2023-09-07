using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing.Imaging;

namespace ImageManipulator.Domain.Common.Dictionaries;

public static class ImageXYCoordinatesDictionary
{
    public static readonly unsafe ImmutableDictionary<PixelFormat, Func<IntPtr, int, int, int, IntPtr>> PixelData =
        ImmutableDictionary.CreateRange(new[]
        {
            KeyValuePair.Create<PixelFormat, Func<IntPtr, int, int, int, IntPtr>>(PixelFormat.Format32bppArgb,
                (scan0, stride, x, y) => (IntPtr)(byte*)(scan0 + y * stride + x * 4)),
            KeyValuePair.Create<PixelFormat, Func<IntPtr, int, int, int, IntPtr>>(PixelFormat.Format24bppRgb,
                (scan0, stride, x, y) => (IntPtr)(byte*)(scan0 + y * stride + x * 3)),
            KeyValuePair.Create<PixelFormat, Func<IntPtr, int, int, int, IntPtr>>(PixelFormat.Format8bppIndexed,
                (scan0, stride, x, y) => (IntPtr)(byte*)(scan0 + y * stride + x)),
            KeyValuePair.Create<PixelFormat, Func<IntPtr, int, int, int, IntPtr>>(PixelFormat.Format4bppIndexed,
                (scan0, stride, x, y) => (IntPtr)(byte*)(scan0 + y * stride + x / 2)),
            KeyValuePair.Create<PixelFormat, Func<IntPtr, int, int, int, IntPtr>>(PixelFormat.Format1bppIndexed,
                (scan0, stride, x, y) => (IntPtr)(byte*)(scan0 + y * stride + x / 8))
        });


    public static readonly ImmutableDictionary<PixelFormat, Func<int, int, int, int>> ByteOffset =
        ImmutableDictionary.CreateRange(new[]
        {
            KeyValuePair.Create<PixelFormat, Func<int, int, int, int>>(PixelFormat.Format32bppArgb,
                (stride, x, y) => y * stride + x * 4),
            KeyValuePair.Create<PixelFormat, Func<int, int, int, int>>(PixelFormat.Format24bppRgb,
                (stride, x, y) => y * stride + x * 3),
            KeyValuePair.Create<PixelFormat, Func<int, int, int, int>>(PixelFormat.Format8bppIndexed,
                (stride, x, y) => y * stride + x),
            KeyValuePair.Create<PixelFormat, Func<int, int, int, int>>(PixelFormat.Format4bppIndexed,
                (stride, x, y) => y * stride + x / 2),
            KeyValuePair.Create<PixelFormat, Func<int, int, int, int>>(PixelFormat.Format1bppIndexed,
                (stride, x, y) => y * stride + x / 8)
        });

    public static readonly ImmutableDictionary<PixelFormat, Func<int, int, int, int, int>> CalculationOffset =
        ImmutableDictionary.CreateRange(new[]
        {
            KeyValuePair.Create<PixelFormat, Func<int, int, int, int, int>>(PixelFormat.Format32bppArgb,
                (byteOffset, stride, x, y) => byteOffset + y * stride + x * 4),
            KeyValuePair.Create<PixelFormat, Func<int, int, int, int, int>>(PixelFormat.Format24bppRgb,
                (byteOffset, stride, x, y) => byteOffset + y * stride + x * 3),
            KeyValuePair.Create<PixelFormat, Func<int, int, int, int, int>>(PixelFormat.Format8bppIndexed,
                (byteOffset, stride, x, y) => byteOffset + y * stride + x),
            KeyValuePair.Create<PixelFormat, Func<int, int, int, int, int>>(PixelFormat.Format4bppIndexed,
                (byteOffset, stride, x, y) => byteOffset + y * stride + x / 2),
            KeyValuePair.Create<PixelFormat, Func<int, int, int, int, int>>(PixelFormat.Format1bppIndexed,
                (byteOffset, stride, x, y) => byteOffset + y * stride + x / 8)
        });
}