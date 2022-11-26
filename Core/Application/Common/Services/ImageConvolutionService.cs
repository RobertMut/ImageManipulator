using ImageManipulator.Domain.Common.Helpers;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ImageManipulator.Application.Common.Services
{
    public class ImageConvolutionService
    {
        public unsafe Bitmap Execute(Bitmap bitmap, double[,] matrix, double factor)
        {
            var newBitmap = new Bitmap(bitmap);

            //if (grayscale == true)
            //{
            //    float rgb = 0;


            //    for (int k = 0; k < pixelBuffer.Length; k += 4)
            //    {
            //        rgb = pixelBuffer[k] * 0.11f;
            //        rgb += pixelBuffer[k + 1] * 0.59f;
            //        rgb += pixelBuffer[k + 2] * 0.3f;


            //        pixelBuffer[k] = (byte)rgb;
            //        pixelBuffer[k + 1] = pixelBuffer[k];
            //        pixelBuffer[k + 2] = pixelBuffer[k];
            //        pixelBuffer[k + 3] = 255;
            //    }
            //}


            int filterWidth = matrix.GetLength(1);
            int filterHeight = matrix.GetLength(0);


            int filterOffset = (filterWidth - 1) / 2;
            int calcOffset = 0;


            int byteOffset = 0;


            BitmapData sourceData = bitmap.LockBitmap(bitmap.PixelFormat);

            byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];
            byte[] resultBuffer = new byte[sourceData.Stride * sourceData.Height];


            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
            bitmap.UnlockBits(sourceData);

                newBitmap.LockBitmap(newBitmap.PixelFormat)
                .ExecuteOnPixels(filterOffset,
                    bitmap.Width - filterOffset,
                    filterOffset,
                    bitmap.Height - filterOffset, 
                    (x, scan0, stride, i, j) =>
                    {
                        byte* pixelData = (byte*)x;
                        double blue = 0;
                        double green = 0;
                        double red = 0;

                        byteOffset = j *
                                     sourceData.Stride +
                                     i * 4;

                        for (int filterY = -filterOffset;
                            filterY <= filterOffset; filterY++)
                        {
                            for (int filterX = -filterOffset;
                                filterX <= filterOffset; filterX++)
                            {


                                calcOffset = byteOffset +
                                             (filterX * 4) +
                                             (filterY * sourceData.Stride);


                                blue += (double)(pixelBuffer[calcOffset]) *
                                        matrix[filterY + filterOffset,
                                                     filterX + filterOffset];


                                green += (double)(pixelBuffer[calcOffset + 1]) *
                                         matrix[filterY + filterOffset,
                                                      filterX + filterOffset];


                                red += (double)(pixelBuffer[calcOffset + 2]) *
                                       matrix[filterY + filterOffset,
                                                    filterX + filterOffset];
                            }
                        }


                        blue = factor * blue + 1;
                        green = factor * green + 1;
                        red = factor * red + 1;


                        if (blue > 255)
                        { blue = 255; }
                        else if (blue < 0)
                        { blue = 0; }


                        if (green > 255)
                        { green = 255; }
                        else if (green < 0)
                        { green = 0; }


                        if (red > 255)
                        { red = 255; }
                        else if (red < 0)
                        { red = 0; }


                        pixelData[0] = (byte)(blue);
                        pixelData[1] = (byte)(green);
                        pixelData[2] = (byte)(red);

                        return new IntPtr(pixelData);
                    }
                );


            return newBitmap;
        }
    }
}
