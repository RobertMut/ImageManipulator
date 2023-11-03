using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Text.Json;
using Core;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Application.Common.Services;
using ImageManipulator.Common.Enums;
using NUnit.Framework;

namespace Application.UnitTests.Services;

[ExcludeFromCodeCoverage]
[TestFixture]
public class ImageBorderServiceTests
{
    private IImageBorderService _imageBorderService;
    private Bitmap _testImage;

    private readonly double[,] _kernel = {
        { 0, -1, 0 },
        { -1, 4, -1 },
        { 0, -1, 0 }
    };
    
    [SetUp]
    public async Task SetUp()
    {
        _testImage = new Bitmap("Resources/image.png");
        _imageBorderService = new ImageBorderService();
    }

    [TestCase(ImageWrapType.BORDER_WRAP, "Resources/image_border_wrap.png")]
    [TestCase(ImageWrapType.BORDER_REFLECT, "Resources/image_border_reflect.png")]
    [TestCase(ImageWrapType.BORDER_CONSTANT, "Resources/image_border_const.png")]
    public async Task ImageBorderServiceExecutes(ImageWrapType imageWrapType, string expectedImageUrl)
    {
        _imageBorderService.Execute(_testImage, imageWrapType, 20, 20, 20, 20, Color.Aquamarine)
            .Compare(new Bitmap(expectedImageUrl));
    }
    
    [Test]
    public async Task ImageBorderReturnsTheSameImage()
    { 
        _imageBorderService.Execute(_testImage, ImageWrapType.BORDER_NONE, 20, 20, 20, 20, Color.Aquamarine)
            .Compare(_testImage);
    }
    
    [Test]
    public async Task ImageBorderThrowsWrapMethodNotSupportedException()
    {
        Assert.Throws<Exception>(
            () => _imageBorderService.Execute(_testImage, ImageWrapType.BORDER_AFTER, 20, 20, 20, 20, Color.Aquamarine),
            "Wrap method not supported");
    }
}