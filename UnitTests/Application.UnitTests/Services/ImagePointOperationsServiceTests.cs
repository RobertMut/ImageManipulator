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
        _imagePointOperationsService.Thresholding(_testImage, 128)
            .Compare(new Bitmap("Resources/image_threshold_replace.png"));
    }
    
    [Test]
    public async Task ImagePointOperationsServiceExecutesThresholdAndDontReplacesColors()
    {
        _imagePointOperationsService.Thresholding(_testImage, 128, false)
            .Compare(new Bitmap("Resources/image_threshold.png"));
    }
    
    [Test]
    public async Task ImagePointOperationsServiceExecutesHistogramEqualization()
    {
        int[][]? lut = JsonSerializer.Deserialize<int[][]>(await File.ReadAllTextAsync("Resources/ImageLUT.json"));

        _imagePointOperationsService.HistogramEqualization(_testImage, lut)
            .Compare(new Bitmap("Resources/image_histogram_eq.png"));
    }
    
    [Test]
    public async Task ImagePointOperationsServiceExecutesStretchContrast()
    {
        _imagePointOperationsService.StretchContrast(_testImage, 57, 170)
            .Compare(new Bitmap("Resources/image_stretch_contrast.png"));
    }
    
    [Test]
    public async Task ImagePointOperationsServiceExecutesMultiThresholdWithReplace()
    {
        _imagePointOperationsService.MultiThresholding(_testImage, 75, 170)
            .Compare(new Bitmap("Resources/image_multithreshold_replace.png"));
    }
    
    [Test]
    public async Task ImagePointOperationsServiceExecutesMultiThresholdWithoutReplace()
    {
        _imagePointOperationsService.MultiThresholding(_testImage, 75, 170, false)
            .Compare(new Bitmap("Resources/image_multithreshold.png"));
    }
    
    [Test]
    public async Task ImagePointOperationsServiceExecutesNonLinearlyStretchContrastWithoutReplace()
    {
        _imagePointOperationsService.NonLinearlyStretchContrast(_testImage, 1.5d)
            .Compare(new Bitmap("Resources/image_nonlinstretch.png"));
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