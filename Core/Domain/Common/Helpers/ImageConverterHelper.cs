using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ImageManipulator.Domain.Common.Helpers;

public static class ImageConverterHelper
{
    public static Bitmap? ConvertFromAvaloniaUIBitmap(Avalonia.Media.Imaging.Bitmap? bitmap)
    {
        Bitmap? result = null;
        
        if(bitmap is null)
            return result;

        using (MemoryStream memStream = new MemoryStream())
        {
            bitmap.Save(memStream);
            memStream.Position = 0;
            result = new(memStream);
        }
        
        return result;
    }

    public static Avalonia.Media.Imaging.Bitmap? ConvertFromSystemDrawingBitmap(Bitmap? bitmap)
    {
        Avalonia.Media.Imaging.Bitmap? result = null;
        if (bitmap == null) return result;
        ImageFormat imageFormat = new ImageFormat(bitmap.RawFormat.Guid);
        
        if (imageFormat.Equals(ImageFormat.MemoryBmp))
        {
            imageFormat = ImageFormat.Bmp;
        }
        
        using (MemoryStream memStream = new MemoryStream())
        {
            bitmap.Save(memStream, imageFormat);
            memStream.Position = 0;
            result = new Avalonia.Media.Imaging.Bitmap(memStream);
        }
            
        return result;
    }
    

}