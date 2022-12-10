using ImageManipulator.Domain.Common.Enums;
using ImageManipulator.Domain.Common.Helpers;
using System;
using System.Drawing;

namespace ImageManipulator.Application.Common.Services
{
    internal class ImageBorderService
    {
        public unsafe Bitmap Execute(Bitmap bitmap, ImageWrapEnum wrapEnum, int top, int bottom, int left, int right, Color color = default)
        {
            var borderedBitmap = new Bitmap(bitmap.Width + left + right, bitmap.Height + top + bottom);
            var source = bitmap.LockBitmapReadOnly(bitmap.PixelFormat);
            Tuple<RotateFlipType, RotateFlipType> rotateFlipType = null;

            if (wrapEnum == ImageWrapEnum.BORDER_CONSTANT)
            {
                borderedBitmap = SetBackground(borderedBitmap, color);
            } else
            {
                rotateFlipType = TranslateWrap(wrapEnum);
            }

            borderedBitmap.UnlockBits(
                borderedBitmap
                .LockBitmapWriteOnly(bitmap.PixelFormat)
                .ExecuteOnPixels(left, left + bitmap.Width, top, top + bitmap.Height,
                    (pixel, scan0, stride, x, y) =>
                    {
                        byte* sourcePixel = (byte*)source.GetPixel(x, y).ToPointer();

                        byte* data = (byte*)pixel.ToPointer();
                        data[0] = sourcePixel[0];
                        data[1] = sourcePixel[1];
                        data[2] = sourcePixel[2];

                        return new IntPtr(data);
                    }
            ));

            bitmap.UnlockBits(source);

            if (rotateFlipType != null)
            {
                borderedBitmap = Reflect(borderedBitmap, bitmap, top, bottom, left, right);
            }

            return borderedBitmap;
        }

        private Bitmap SetBackground(Bitmap bitmap, Color color)
        {
            using (Graphics graphics = Graphics.FromImage(bitmap))
            using (SolidBrush brush = new SolidBrush(color))
            {
                graphics.FillRectangle(brush, 0, 0, bitmap.Width, bitmap.Height);
                graphics.Save();
            }

            return bitmap;
        }

        private Bitmap Reflect(Bitmap bitmap, Bitmap source, int top, int bottom, int left, int right)
        {
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                bitmap.RotateFlip(RotateFlipType.Rotate180FlipY);
                //Reflect to the left
                graphics.DrawImageUnscaled(bitmap, 0 + top, 0, left, source.Height);
                //Reflect to the right
                graphics.DrawImageUnscaled(bitmap, left + source.Width, top, bitmap.Width + left + right, source.Height);

                bitmap.RotateFlip(RotateFlipType.Rotate180FlipY);
                bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);

                //Reflect top image
                graphics.DrawImageUnscaled(bitmap, left, 0, source.Width + left, top);
                //Reflect to the top left
                graphics.DrawImageUnscaled(bitmap, 0, 0, top, left);
                //Reflect to the top right
                graphics.DrawImageUnscaled(bitmap, source.Width + left, 0, source.Width + left + right, top);
                //Reflect to the bottom
                graphics.DrawImageUnscaled(bitmap, left, source.Height + top, source.Width + left, source.Height + top + bottom);
                //Reflect to the bottom left
                graphics.DrawImageUnscaled(bitmap, 0, source.Height + top, left, source.Height + top + bottom);
                //Reflect to the bottom right
                graphics.DrawImageUnscaled(bitmap, source.Width + left, source.Height + top, source.Width + left + right, source.Height + bottom + top);

                bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);

                graphics.Save();
            }

            return bitmap;
        }

        private Tuple<RotateFlipType, RotateFlipType> TranslateWrap(ImageWrapEnum wrapEnum)
        {
            switch (wrapEnum)
            {
                case ImageWrapEnum.BORDER_WRAP:
                    return new Tuple<RotateFlipType, RotateFlipType>(RotateFlipType.RotateNoneFlipNone, RotateFlipType.RotateNoneFlipNone);

                case ImageWrapEnum.BORDER_REFLECT:
                    return new Tuple<RotateFlipType, RotateFlipType>(RotateFlipType.Rotate180FlipY, RotateFlipType.Rotate180FlipX);

                case (_):
                    throw new Exception("Wrap method not recognized");
            }
        }
    }
}