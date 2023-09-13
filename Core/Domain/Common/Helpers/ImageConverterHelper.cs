using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ImageManipulator.Domain.Common.Helpers;

public static class ImageConverterHelper
{
    public static Bitmap? ConvertFromAvaloniaUIBitmap(Avalonia.Media.Imaging.Bitmap? bitmap)
    {
        if(bitmap is null)
            return null;
            
        using var memStream = new MemoryStream();
        bitmap.Save(memStream);
        Bitmap result = new Bitmap(memStream);

        return result;
    }

    public static Avalonia.Media.Imaging.Bitmap? ConvertFromSystemDrawingBitmap(Bitmap? bitmap)
    {
        if (bitmap == null) return null;

        using var bitmapTmp = new Bitmap(bitmap);

        var bitmapData = bitmapTmp.LockBits(new Rectangle(0, 0, bitmapTmp.Width, bitmapTmp.Height),
            ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

        Avalonia.Media.Imaging.Bitmap? convertedBitmap = new Avalonia.Media.Imaging.Bitmap(
            Avalonia.Platform.PixelFormat.Bgra8888, Avalonia.Platform.AlphaFormat.Premul,
            bitmapData.Scan0,
            new Avalonia.PixelSize(bitmapData.Width, bitmapData.Height),
            new Avalonia.Vector(96, 96),
            bitmapData.Stride);

        bitmapTmp.UnlockBits(bitmapData);
            
        return convertedBitmap;
    }
}