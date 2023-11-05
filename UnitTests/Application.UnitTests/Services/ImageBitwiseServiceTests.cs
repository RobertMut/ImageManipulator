using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using ImageManipulator.Application.Common.Services;
using ImageManipulator.Common.Enums;
using UnitTests.Core;

namespace Application.UnitTests.Services;

[ExcludeFromCodeCoverage]
[TestFixture]
public class ImageBitwiseServiceTests
{
    private ImageBitwiseService _imageBitwiseService;
    private Bitmap _testImage;

    [SetUp]
    public async Task SetUp()
    {
        _testImage = new Bitmap("Resources/image.png");
        _imageBitwiseService = new ImageBitwiseService();
    }

    [TestCase(BitwiseOperationType.AND, "Resources/Bitwise/image_and.png")]
    [TestCase(BitwiseOperationType.OR, "Resources/Bitwise/image_or.png")]
    [TestCase(BitwiseOperationType.XOR, "Resources/Bitwise/image_xor.png")]
    [TestCase(BitwiseOperationType.NOT, "Resources/Bitwise/image_not.png")]
    [TestCase(BitwiseOperationType.RightShift, "Resources/Bitwise/image_rshift.png")]
    [TestCase(BitwiseOperationType.LeftShift, "Resources/Bitwise/image_lshift.png")]
    public async Task ImageArithmeticExecutes(BitwiseOperationType operation, string expectedImage)
    {
        _imageBitwiseService.Execute(_testImage, 10, operation)
            .Compare(new Bitmap(expectedImage), ImageFormat.Png);
    }
}