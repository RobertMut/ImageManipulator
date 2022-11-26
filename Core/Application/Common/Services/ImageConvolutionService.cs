using ImageManipulator.Domain.Common.Helpers;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ImageManipulator.Application.Common.Services
{
    public class ImageConvolutionService
    {
        public Bitmap Execute(Bitmap bitmap, double[,] matrix, double factor)
        {

            var bitmapData = bitmap.LockBitmap(bitmap.PixelFormat);

            byte[] pixelBuffer = new byte[bitmapData.Stride * bitmapData.Height];
            byte[] resultBuffer = new byte[bitmapData.Stride * bitmapData.Height];

            Marshal.Copy(bitmap.Scan0, pixelBuffer, 0, pixelBuffer.Length);
            bitmap.UnlockBits(bitmapData);
        }
    }
}
