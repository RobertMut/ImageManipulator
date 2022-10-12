using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ImageManipulator.Application.Common.Helpers
{
    public class ImageConverterHelper
    {
        public static Bitmap ConvertFromAvaloniaUIBitmap(Avalonia.Media.Imaging.Bitmap bitmap)
        {
            Bitmap result = null;

            if(bitmap is null)
            {
                return result;
            }

            using (var memStream = new MemoryStream())
            {
                bitmap.Save(memStream);
                result = new Bitmap(memStream);
            }

            return result;
        }

        public static Avalonia.Media.Imaging.Bitmap ConvertFromSystemDrawingBitmap(Bitmap bitmap)
        {
            if (bitmap == null) return null;

            System.Drawing.Bitmap bitmapTmp = new System.Drawing.Bitmap(bitmap);

            var bitmapdata = bitmapTmp.LockBits(new Rectangle(0, 0, bitmapTmp.Width, bitmapTmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            Avalonia.Media.Imaging.Bitmap convertedBitmap = new Avalonia.Media.Imaging.Bitmap(Avalonia.Platform.PixelFormat.Bgra8888, Avalonia.Platform.AlphaFormat.Premul,
                bitmapdata.Scan0,
                new Avalonia.PixelSize(bitmapdata.Width, bitmapdata.Height),
                new Avalonia.Vector(96, 96),
                bitmapdata.Stride);

            bitmapTmp.UnlockBits(bitmapdata);
            bitmapTmp.Dispose();
            return convertedBitmap;
        }
    }
}
