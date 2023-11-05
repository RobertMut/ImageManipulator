using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using ImageManipulator.Application.Common.Services;
using ImageManipulator.Common.Enums;
using UnitTests.Core;

namespace Application.UnitTests.Services;

[ExcludeFromCodeCoverage]
[TestFixture]
public class ImageArithmeticServiceTests
{
    private ImageArithmeticService _imageArithmeticService;
    private Bitmap _testImage;

    [SetUp]
    public async Task SetUp()
    {
        _testImage = new Bitmap("Resources/image.png");
        _imageArithmeticService = new ImageArithmeticService();
    }

    [TestCase(ArithmeticOperationType.Add, "Resources/Arithmetic/image_add.png")]
    [TestCase(ArithmeticOperationType.Amplitude, "Resources/Arithmetic/image_amp.png")]
    [TestCase(ArithmeticOperationType.Average, "Resources/Arithmetic/image_avg.png")]
    [TestCase(ArithmeticOperationType.Difference, "Resources/Arithmetic/image_diff.png")]
    [TestCase(ArithmeticOperationType.Divide, "Resources/Arithmetic/image_div.png")]
    [TestCase(ArithmeticOperationType.SubtractLeft, "Resources/Arithmetic/image_lsub.png")]
    [TestCase(ArithmeticOperationType.SubtractRight, "Resources/Arithmetic/image_rsub.png")]
    [TestCase(ArithmeticOperationType.Max, "Resources/Arithmetic/image_max.png")]
    [TestCase(ArithmeticOperationType.Min, "Resources/Arithmetic/image_min.png")]
    [TestCase(ArithmeticOperationType.Multiply, "Resources/Arithmetic/image_mul.png")]
    public async Task ImageArithmeticExecutes(ArithmeticOperationType operation, string expectedImage)
    {
        _imageArithmeticService.Execute(_testImage, 10, operation)
            .Compare(new Bitmap(expectedImage), ImageFormat.Png);
    }
}