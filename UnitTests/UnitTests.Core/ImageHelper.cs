using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using NUnit.Framework;

namespace Core;

[ExcludeFromCodeCoverage(Justification = "Test helper class")]
public static class ImageHelper
{
    public static byte[] ImageToByte(Bitmap img, ImageFormat? format = null)
    {
        format ??= ImageFormat.Bmp;
        
        byte[] bytes = null;
        Bitmap copiedBitmap = new Bitmap(img);
        
        using (var stream = new MemoryStream())
        {
            copiedBitmap.Save(stream, format);
            stream.Position = 0;
            bytes = stream.ToArray();
        }
        
        return bytes;
    }
    
    public static Bitmap PaintImage(Bitmap bitmap, Color color)
    {
        using (Graphics graphics = Graphics.FromImage(bitmap))
        {
            Pen pen = new Pen(color);
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            graphics.DrawRectangle(pen, rect);
            graphics.Save(); 
        }
        
        return new Bitmap(bitmap);
    }
    
    public static Image GetBitmapWithoutLock(string path)
    {
        Image img;
            
        using (var bmpTemp = new Bitmap(path))
        {
            img = new Bitmap(bmpTemp);
        }
            
        return img;
    }

    public static void Compare(this Bitmap returnedBitmap, Bitmap expectedBitmap, ImageFormat format = null)
    {
        byte[] expectedBytes = ImageToByte(expectedBitmap, format);
        byte[] actualBytes = ImageToByte(returnedBitmap, format);
        
        Assert.That(actualBytes, Is.EqualTo(expectedBytes));
    }
}