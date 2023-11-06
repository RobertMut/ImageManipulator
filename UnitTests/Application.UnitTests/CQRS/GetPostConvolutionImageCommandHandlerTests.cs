using System.Diagnostics.CodeAnalysis;
using Avalonia.Headless.NUnit;
using ImageManipulator.Application.Common.CQRS.Queries.GetPostConvolutionImage;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Application.Common.Models;
using ImageManipulator.Application.ViewModels;
using ImageManipulator.Common.Enums;
using ImageManipulator.Domain.Common.CQRS.Interfaces;
using Moq;
using UnitTests.Core;
using static Moq.It;
using Bitmap = System.Drawing.Bitmap;
using Color = System.Drawing.Color;
using Is = NUnit.Framework.Is;

namespace Application.UnitTests.CQRS;

[ExcludeFromCodeCoverage]
[TestFixture]
public class GetPostConvolutionImageCommandHandlerTests
{
    private Mock<IImageConvolutionService> _imageConvolutionServiceMock;
    private Mock<IImageBorderService> _imageBorderServiceMock;
    private Mock<ITabService> _tabServiceMock;
    private GetPostConvolutionImageCommandHandler _handler;
    private Bitmap _testImage;
    private TabItem _sampleTab;

    [SetUp]
    public async Task SetUp()
    {
        _testImage = new Bitmap("Resources/image.png");
        _sampleTab = new TabItem("Tab 1",
            new TabControlViewModel(Mock.Of<IQueryDispatcher>(), Mock.Of<ICommandDispatcher>())
            {
                Path = "Path",
                Image = _testImage
            });
        _imageConvolutionServiceMock = new Mock<IImageConvolutionService>();
        _tabServiceMock = new Mock<ITabService>();
        _imageBorderServiceMock = new Mock<IImageBorderService>();
        _tabServiceMock.Setup(x => x.CurrentTabName).Returns("Tab 1");
        _tabServiceMock.Setup(x => x.GetTab(IsAny<string>()))
            .Returns(_sampleTab);
        _imageBorderServiceMock.Setup(x => x.Execute(IsAny<Bitmap>(), IsAny<ImageWrapType>(), IsAny<int>(),
            IsAny<int>(), IsAny<int>(), IsAny<int>(), IsAny<Color>())).Returns(_testImage);
        _imageConvolutionServiceMock.Setup(x => x.Execute(IsAny<Bitmap>(), IsAny<double[,]>(), IsAny<bool>()))
            .Returns(_testImage);
        _imageConvolutionServiceMock
            .Setup(x => x.ComputeGradient(IsAny<Bitmap>(), IsAny<Func<double, double, double>>()))
            .Returns(new double[,] { {1, 1}, {1, 1} });
        _imageConvolutionServiceMock.Setup(x => x.HysteresisThresholding(IsAny<int>(), IsAny<int>(),
                IsAny<int>(), IsAny<int>(), IsAny<double[,]>()))
            .Returns(_testImage);
        _handler = new GetPostConvolutionImageCommandHandler(_tabServiceMock.Object, _imageBorderServiceMock.Object,
            _imageConvolutionServiceMock.Object);
    }
    
    [TestCase(true, false, SoftenSharpenType.SoftenAverage, SobelType.North, MatrixSize.three, EdgeDetectionType.Laplace, ImageWrapType.BORDER_CONSTANT)]
    [TestCase(false, true, SoftenSharpenType.SoftenAverage, SobelType.North, MatrixSize.three, EdgeDetectionType.Laplace, ImageWrapType.BORDER_CONSTANT)]
    [TestCase(false, true, SoftenSharpenType.SoftenAverage, SobelType.North, MatrixSize.three, EdgeDetectionType.Laplace, ImageWrapType.BORDER_NONE)]
    [TestCase(false, true, SoftenSharpenType.SoftenAverage, SobelType.North, MatrixSize.three, EdgeDetectionType.Laplace, ImageWrapType.BORDER_AFTER)]
    [TestCase(false, true, SoftenSharpenType.SoftenAverage, SobelType.North, MatrixSize.three, EdgeDetectionType.Laplace, ImageWrapType.BORDER_WRAP)]
    [TestCase(false, false, SoftenSharpenType.SoftenAverage, SobelType.North, MatrixSize.three, EdgeDetectionType.Laplace, ImageWrapType.BORDER_REFLECT)]
    [TestCase(false, false, SoftenSharpenType.SoftenAverageWithWeight, SobelType.North, MatrixSize.three, EdgeDetectionType.Laplace, ImageWrapType.BORDER_REFLECT)]
    [TestCase(false, false, SoftenSharpenType.Laplace1, SobelType.North, MatrixSize.three, EdgeDetectionType.Laplace, ImageWrapType.BORDER_REFLECT)]
    [TestCase(false, false, SoftenSharpenType.SoftenGauss, SobelType.North, MatrixSize.three, EdgeDetectionType.Laplace, ImageWrapType.BORDER_REFLECT)]
    [TestCase(false, false, SoftenSharpenType.Laplace2, SobelType.North, MatrixSize.three, EdgeDetectionType.Laplace, ImageWrapType.BORDER_REFLECT)]
    [TestCase(false, false, SoftenSharpenType.Laplace3, SobelType.North, MatrixSize.three, EdgeDetectionType.Laplace, ImageWrapType.BORDER_REFLECT)]
    [TestCase(false, false, SoftenSharpenType.Laplace3, SobelType.North, MatrixSize.three, EdgeDetectionType.Laplace, ImageWrapType.BORDER_REFLECT)]
    [TestCase(false, true, SoftenSharpenType.Laplace3, SobelType.North, MatrixSize.three, EdgeDetectionType.Canny, ImageWrapType.BORDER_NONE)]
    [TestCase(false, true, SoftenSharpenType.Laplace3, SobelType.North, MatrixSize.three, EdgeDetectionType.PrewittHorizontal, ImageWrapType.BORDER_NONE)]
    [TestCase(false, true, SoftenSharpenType.Laplace3, SobelType.North, MatrixSize.three, EdgeDetectionType.PrewittVertical, ImageWrapType.BORDER_NONE)]
    [TestCase(true, false, SoftenSharpenType.Laplace3, SobelType.North, MatrixSize.five, EdgeDetectionType.PrewittVertical, ImageWrapType.BORDER_NONE)]
    [TestCase(true, false, SoftenSharpenType.Laplace3, SobelType.South, MatrixSize.seven, EdgeDetectionType.PrewittVertical, ImageWrapType.BORDER_NONE)]
    [TestCase(true, false, SoftenSharpenType.Laplace3, SobelType.NorthEast, MatrixSize.nine, EdgeDetectionType.PrewittVertical, ImageWrapType.BORDER_NONE)]
    [AvaloniaTest]
    public async Task GetPostConvolutionImageCommandInvokesItsMethods(bool sobel, bool edgeDetection,
        SoftenSharpenType softenSharpenType, SobelType sobelType, MatrixSize matrixSize,
        EdgeDetectionType edgeDetectionType, ImageWrapType imageWrapType)
    {
        bool isCannyExecuted = edgeDetectionType == EdgeDetectionType.Canny && edgeDetection;
        Bitmap response = await _handler.Handle(new GetPostConvolutionImageQuery
        {
            Sobel = sobel,
            EdgeDetection = edgeDetection,
            Value = 3,
            Border = 4,
            Color = Color.Black,
            SoftenSharpenType = SoftenSharpenType.SoftenAverage,
            SobelType = sobelType,
            MatrixSize = matrixSize,
            EdgeDetectionType = edgeDetectionType,
            ImageWrapType = imageWrapType,
        }, new CancellationToken());
        
        Assert.That(ImageHelper.ImageToByte(response), Is.EqualTo(ImageHelper.ImageToByte(_testImage)));
        _tabServiceMock.Verify(x => x.CurrentTabName, Times.Once);
        _tabServiceMock.Verify(x => x.GetTab(Is<string>(y => y == "Tab 1")), Times.Once);
        _imageBorderServiceMock
            .Verify(x => x.Execute(IsAny<Bitmap>(), Is<ImageWrapType>(x => x == imageWrapType), IsAny<int>(),
            IsAny<int>(), IsAny<int>(), IsAny<int>(), IsAny<Color>()), (int)imageWrapType > 0 && (int)imageWrapType < 4 ? Times.Once : Times.Never);
        _imageConvolutionServiceMock
            .Verify(x => x.Execute(IsAny<Bitmap>(), IsAny<double[,]>(), IsAny<bool>()), Times.Once);
        _imageConvolutionServiceMock
            .Verify(x => x.ComputeGradient(IsAny<Bitmap>(), IsAny<Func<double, double, double>>()), 
                isCannyExecuted ? Times.Once : Times.Never);
        _imageConvolutionServiceMock.Verify(x => x.HysteresisThresholding(IsAny<int>(), IsAny<int>(),
            IsAny<int>(), IsAny<int>(), IsAny<double[,]>()), isCannyExecuted ? Times.Once : Times.Never);
    }
    
    [AvaloniaTest]
    public async Task GetPostConvolutionImageCommandThrowsInvalidOperationException()
    {
        _tabServiceMock.Setup(x => x.GetTab(It.IsAny<string>())).Returns(default(TabItem));
        
        Assert.ThrowsAsync<NullReferenceException>(async () => await _handler.Handle(new GetPostConvolutionImageQuery
        {
            Sobel = true,
            EdgeDetection = true,
            Value = 3,
            Border = 4,
            Color = Color.Black,
            SoftenSharpenType = SoftenSharpenType.SoftenAverage,
            SobelType = SobelType.East,
            MatrixSize = MatrixSize.five,
            EdgeDetectionType = EdgeDetectionType.Canny,
            ImageWrapType = ImageWrapType.BORDER_NONE,
        }, new CancellationToken()), "Actual displayed bitmap does not exist");
        
        _tabServiceMock.Verify(x => x.CurrentTabName, Times.Once);
        _tabServiceMock.Verify(x => x.GetTab(Is<string>(y => y == "Tab 1")), Times.Once);
        _imageBorderServiceMock
            .Verify(x => x.Execute(IsAny<Bitmap>(), It.IsAny<ImageWrapType>(), IsAny<int>(),
            IsAny<int>(), IsAny<int>(), IsAny<int>(), IsAny<Color>()), Times.Never);
        _imageConvolutionServiceMock
            .Verify(x => x.Execute(IsAny<Bitmap>(), IsAny<double[,]>(), IsAny<bool>()), Times.Never);
        _imageConvolutionServiceMock
            .Verify(x => x.ComputeGradient(IsAny<Bitmap>(), IsAny<Func<double, double, double>>()), Times.Never);
        _imageConvolutionServiceMock.Verify(x => x.HysteresisThresholding(IsAny<int>(), IsAny<int>(),
            IsAny<int>(), IsAny<int>(), IsAny<double[,]>()), Times.Never);
    }
}