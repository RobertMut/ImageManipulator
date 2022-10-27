using System.Drawing;
using System.Drawing.Imaging;

namespace ImageManipulator.Domain.Common.Helpers
{
    public static class BitmapOperationsHelper
    {
        public static BitmapData LockBitmap(this Bitmap bitmap, PixelFormat pixelFormat) => bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, pixelFormat);
    }
}