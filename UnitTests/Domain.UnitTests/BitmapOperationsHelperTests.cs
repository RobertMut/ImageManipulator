using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using ImageManipulator.Domain.Common.Helpers;
using UnitTests.Core;

namespace Domain.UnitTests;

[ExcludeFromCodeCoverage]
public class BitmapOperationsHelperTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void BitmapOperationsHelperLocksBitmap()
    {
        BitmapData bitmapData = new Bitmap(100, 100).LockBitmap(PixelFormat.Format32bppArgb, ImageLockMode.ReadWrite);
        
        Assert.That(bitmapData.Height, Is.EqualTo(100));
        Assert.That(bitmapData.Width, Is.EqualTo(100));
    }

    [Test]
    public unsafe void BitmapOperationsHelperExecutesOnPixelsWith5Arg()
    {
        Bitmap expectedBitmap = new Bitmap(100, 100);
        using Graphics g = Graphics.FromImage(expectedBitmap);
        {
            g.DrawRectangle(new Pen(new SolidBrush(Color.FromArgb(255, 128, 128, 128))), 0, 0, 100, 100);
            g.Save();
        }
        
        Bitmap emptyBitmap = new Bitmap(100, 100);
        BitmapData bitmapData = emptyBitmap.LockBitmap(PixelFormat.Format32bppArgb, ImageLockMode.WriteOnly)
            .ExecuteOnPixels((data, scan0, stride, x, y) =>
            {
                byte* pixel = (byte*)data.ToPointer();
                pixel[0] = 128;
                pixel[1] = 128;
                pixel[2] = 128;
            });
        emptyBitmap.UnlockBits(bitmapData);

        emptyBitmap.Compare(emptyBitmap);
    }
    
    [Test]
    public unsafe void BitmapOperationsHelperExecutesOnPixelsWith3Args()
    {
        Bitmap expectedBitmap = new Bitmap(100, 100);
        using Graphics g = Graphics.FromImage(expectedBitmap);
        {
            g.DrawRectangle(new Pen(new SolidBrush(Color.FromArgb(255, 128, 128, 128))), 0, 0, 100, 100);
            g.Save();
        }
        
        Bitmap emptyBitmap = new Bitmap(100, 100);
        BitmapData bitmapData = emptyBitmap.LockBitmap(PixelFormat.Format32bppArgb, ImageLockMode.WriteOnly)
            .ExecuteOnPixels((data, stride, i) =>
            {
                byte* pixel = (byte*)data.ToPointer();
                pixel[0] = 128;
                pixel[1] = 128;
                pixel[2] = 128;
            });
        emptyBitmap.UnlockBits(bitmapData);

        emptyBitmap.Compare(emptyBitmap);
    }
    
    [Test]
    public unsafe void BitmapOperationsHelperExecutesOnPixelsOffset()
    {
        Bitmap expectedBitmap = new Bitmap(100, 100);
        using Graphics g = Graphics.FromImage(expectedBitmap);
        {
            g.DrawRectangle(new Pen(new SolidBrush(Color.FromArgb(255, 128, 128, 128))), 50, 50, 100, 100);
            g.Save();
        }
        
        Bitmap emptyBitmap = new Bitmap(100, 100);
        BitmapData bitmapData = emptyBitmap.LockBitmap(PixelFormat.Format32bppArgb, ImageLockMode.WriteOnly)
            .ExecuteOnPixels(50, 100, 100, 100,(data, scan0, stride, x, y) =>
            {
                byte* pixel = (byte*)data.ToPointer();
                pixel[0] = 128;
                pixel[1] = 128;
                pixel[2] = 128;
            });
        emptyBitmap.UnlockBits(bitmapData);

        emptyBitmap.Compare(emptyBitmap);
    }
    
    [Test]
    public unsafe void BitmapOperationsHelperExecuteOnSinglePixel()
    {
        Bitmap expectedBitmap = new Bitmap(100, 100);
        using Graphics graphics = Graphics.FromImage(expectedBitmap);
        {
            graphics.DrawRectangle(new Pen(new SolidBrush(Color.FromArgb(255, 128, 128, 128))), 99, 99, 100, 100);
            graphics.Save();
        }

        Bitmap otherImage = new Bitmap(1, 1);
        using Graphics g = Graphics.FromImage(otherImage);
        {
            graphics.DrawRectangle(new Pen(new SolidBrush(Color.FromArgb(255, 128, 128, 128))), 0, 0, 1, 1);
            graphics.Save();
        }
        
        Bitmap emptyBitmap = new Bitmap(100, 100);
        BitmapData bitmapData = emptyBitmap.LockBitmap(PixelFormat.Format32bppArgb, ImageLockMode.WriteOnly);
        BitmapData otherImageData = otherImage.LockBitmap(PixelFormat.Format32bppArgb, ImageLockMode.ReadOnly);
        bitmapData.GetPixel(99, 99).ExecuteOnPixel(otherImageData.GetPixel(0, 0), (current, other) => other);
        otherImage.UnlockBits(otherImageData);
        emptyBitmap.UnlockBits(bitmapData);
        

        emptyBitmap.Compare(emptyBitmap);
    }
}