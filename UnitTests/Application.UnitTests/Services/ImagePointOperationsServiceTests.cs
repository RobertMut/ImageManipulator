using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using Core;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Application.Common.Services;
using NUnit.Framework;

namespace Application.UnitTests.Services;

[ExcludeFromCodeCoverage]
[TestFixture]
public class ImagePointOperationsServiceTests
{
    private IImagePointOperationsService _imagePointOperationsService;
    private Bitmap _testImage;

    [SetUp]
    public async Task SetUp()
    {
        _testImage = new Bitmap("Resources/image.png");
        _imagePointOperationsService = new ImagePointOperationsService();
    }

    [Test]
    public async Task ImagePointOperationsServiceExecutesThresholdAndReplacesColors()
    {
        Bitmap expectedImage = new Bitmap("Resources/image_threshold_replace.png");
        Bitmap bitmap = _imagePointOperationsService.Thresholding(_testImage, 128);

        Bitmap pngBitmap;
        using (MemoryStream stream = new MemoryStream())
        {
            bitmap.Save(stream, ImageFormat.Png);
            pngBitmap = new Bitmap(stream);
        }
        
        byte[] expectedBytes = ImageHelper.ImageToByte(expectedImage);
        byte[] actualBytes = ImageHelper.ImageToByte(pngBitmap);
        
        Assert.That(actualBytes, Is.EqualTo(expectedBytes));
    }
}