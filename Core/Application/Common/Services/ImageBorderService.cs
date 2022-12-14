using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Common.Enums;
using ImageManipulator.Domain.Common.Helpers;
using System;
using System.Drawing;

namespace ImageManipulator.Application.Common.Services
{
    public class ImageBorderService : IImageBorderService
    {
        public unsafe Bitmap Execute(Bitmap bitmap, ImageWrapEnum wrapEnum, int top, int bottom, int left, int right, Color color = default)
        {
            var borderedBitmap = new Bitmap(bitmap.Width + left + right, bitmap.Height + top + bottom);
            Tuple<RotateFlipType, RotateFlipType> rotateFlipType = null;

            if (wrapEnum == ImageWrapEnum.BORDER_CONSTANT)
            {
                borderedBitmap = SetBackground(borderedBitmap, color);
            }
            else
            {
                rotateFlipType = TranslateWrap(wrapEnum);
            }

            using(Graphics graphics = Graphics.FromImage(borderedBitmap))
            {
                graphics.DrawImageUnscaledAndClipped(bitmap, new Rectangle(left, top, bitmap.Width, bitmap.Height));

                graphics.Save();
            }

            if (rotateFlipType != null)
            {
                borderedBitmap = Reflect(borderedBitmap, bitmap, rotateFlipType, top, bottom, left, right);
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

        private Bitmap Reflect(Bitmap bitmap, Bitmap source, Tuple<RotateFlipType, RotateFlipType> flipTypes, int top, int bottom, int left, int right)
        {
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                bitmap.RotateFlip(flipTypes.Item1);
                //Reflect to the left
                graphics.DrawImageUnscaledAndClipped(source, new Rectangle(0, top, left, source.Height));
                //Reflect to the right
                graphics.DrawImageUnscaledAndClipped(source, new Rectangle(left + source.Width, top, source.Width + left + right, source.Height));

                bitmap.RotateFlip(flipTypes.Item1);
                bitmap.RotateFlip(flipTypes.Item2);

                //Reflect top image
                graphics.DrawImageUnscaledAndClipped(source, new Rectangle(left, 0, source.Width + left, top));
                //Reflect to the top left
                graphics.DrawImageUnscaledAndClipped(source, new Rectangle(0, 0, top, left));
                //Reflect to the top right
                graphics.DrawImageUnscaledAndClipped(source, new Rectangle(source.Width + left, 0, source.Width + left + right, top));
                //Reflect to the bottom
                graphics.DrawImageUnscaledAndClipped(source, new Rectangle(left, source.Height + top, source.Width + left, source.Height + top + bottom));
                //Reflect to the bottom left
                graphics.DrawImageUnscaledAndClipped(source, new Rectangle(0, source.Height + top, left, source.Height + top + bottom));
                //Reflect to the bottom right
                graphics.DrawImageUnscaledAndClipped(source, new Rectangle(source.Width + left, source.Height + top, source.Width + left + right, source.Height + bottom + top));

                bitmap.RotateFlip(flipTypes.Item2);

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

                default:
                    throw new Exception("Wrap method not recognized");
            }
        }
    }
}