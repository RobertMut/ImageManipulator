using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.Json;
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
    
    [Test]
    public async Task ImagePointOperationsServiceExecutesThresholdAndDontReplacesColors()
    {
        Bitmap expectedImage = new Bitmap("Resources/image_threshold.png");
        Bitmap bitmap = _imagePointOperationsService.Thresholding(_testImage, 128, false);

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
    
    [Test]
    public async Task ImagePointOperationsServiceExecutesHistogramEqualization()
    {
        int[][]? lut = JsonSerializer.Deserialize<int[][]>(await File.ReadAllTextAsync("Resources/ImageLUT.json"));
        Bitmap expectedImage = new Bitmap("Resources/image_histogram_eq.png");

        Bitmap bitmap = _imagePointOperationsService.HistogramEqualization(_testImage, lut);

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
    
    [Test]
    public async Task ImagePointOperationsServiceExecutesStretchContrast()
    {
        Bitmap expectedImage = new Bitmap("Resources/image_stretch_contrast.png");

        Bitmap bitmap = _imagePointOperationsService.StretchContrast(_testImage, 57, 170);

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
    
    [Test]
    public async Task ImagePointOperationsServiceExecutesMultiThresholdWithReplace()
    {
        Bitmap expectedImage = new Bitmap("Resources/image_multithreshold_replace.png");

        Bitmap bitmap = _imagePointOperationsService.MultiThresholding(_testImage, 75, 170);

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
    
    [Test]
    public async Task ImagePointOperationsServiceExecutesMultiThresholdWithoutReplace()
    {
        Bitmap expectedImage = new Bitmap("Resources/image_multithreshold.png");

        Bitmap bitmap = _imagePointOperationsService.MultiThresholding(_testImage, 75, 170, false);

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
    
    [Test]
    public async Task ImagePointOperationsServiceExecutesNonLinearlyStretchContrastWithoutReplace()
    {
        Bitmap expectedImage = new Bitmap("Resources/image_nonlinstretch.png");

        Bitmap bitmap = _imagePointOperationsService.NonLinearlyStretchContrast(_testImage, 1.5d);

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
    
    [Test]
    public async Task ImagePointOperationsServiceExecutesCalculateLowerImageThreshold()
    {
        int[]? histogram = JsonSerializer.Deserialize<int[]>(await File.ReadAllTextAsync("Resources/ImageLuminance.json"));

        int lowerImageThresholdPoint = _imagePointOperationsService.CalculateLowerImageThresholdPoint(histogram);
        
        Assert.That(lowerImageThresholdPoint, Is.EqualTo(99));
    }
    
    [Test]
    public async Task ImagePointOperationsServiceExecutesCalculateUpperImageThreshold()
    {
        int[]? histogram = JsonSerializer.Deserialize<int[]>(await File.ReadAllTextAsync("Resources/ImageLuminance.json"));

        int upperImageThresholdPoint = _imagePointOperationsService.CalculateUpperImageThresholdPoint(histogram);
        
        Assert.That(upperImageThresholdPoint, Is.EqualTo(255));
    }
    
    [Test]
    public async Task ImagePointOperationsServiceExecutesCalculateLowerImageThresholdAndThrowsException()
    {
        Assert.Throws<NullReferenceException>(
            () => _imagePointOperationsService.CalculateLowerImageThresholdPoint(null), "Histogram is null");
    }
    
    [Test]
    public async Task ImagePointOperationsServiceExecutesCalculateUpperImageThresholdAndThrowsException()
    {
        Assert.Throws<NullReferenceException>(
            () => _imagePointOperationsService.CalculateUpperImageThresholdPoint(null), "Histogram is null");
    }
}