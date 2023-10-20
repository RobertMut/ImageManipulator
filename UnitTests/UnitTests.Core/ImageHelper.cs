using System.Drawing;
using System.Drawing.Imaging;

namespace Core;

public static class ImageHelper
{
    public static byte[] ImageToByte(Image img)
    {
        byte[] bytes = null;
        
        using (var stream = new MemoryStream())
        {
            img.Save(stream, ImageFormat.Bmp);
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
}