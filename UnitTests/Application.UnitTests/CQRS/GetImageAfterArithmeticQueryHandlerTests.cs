using System.Diagnostics.CodeAnalysis;
using Avalonia.Headless.NUnit;
using Core;
using ImageManipulator.Application.Common.CQRS.Queries.GetImageAfterArithmetic;
using ImageManipulator.Application.Common.Enums;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Application.Common.Models;
using ImageManipulator.Application.ViewModels;
using ImageManipulator.Common.Enums;
using ImageManipulator.Domain.Common.CQRS.Interfaces;
using Moq;
using static Moq.It;
using Bitmap = System.Drawing.Bitmap;
using Is = NUnit.Framework.Is;

namespace Application.UnitTests.CQRS;

[ExcludeFromCodeCoverage]
[TestFixture]
public class GetImageAfterArithmeticQueryHandlerTests
{
    private Mock<IImageArithmeticService> _imageArithmeticServiceMock;
    private Mock<ITabService> _tabServiceMock;
    private GetImageAfterArithmeticQueryHandler _handler;
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
        _imageArithmeticServiceMock = new Mock<IImageArithmeticService>();
        _tabServiceMock = new Mock<ITabService>();
        _tabServiceMock.Setup(x => x.CurrentTabName).Returns("Tab 1");
        _tabServiceMock.Setup(x => x.GetTab(IsAny<string>()))
            .Returns(_sampleTab);
        _imageArithmeticServiceMock.Setup(x =>
                x.Execute(IsAny<Bitmap>(), IsAny<object>(), IsAny<ArithmeticOperationType>()))
            .Returns(_testImage);
        _handler = new GetImageAfterArithmeticQueryHandler(_tabServiceMock.Object, _imageArithmeticServiceMock.Object);
    }

    [TestCase(ElementaryOperationParameterType.Image, ArithmeticOperationType.Average)]
    [TestCase(ElementaryOperationParameterType.Value, ArithmeticOperationType.Average)]
    [TestCase(ElementaryOperationParameterType.Color, ArithmeticOperationType.Average)]
    [TestCase(ElementaryOperationParameterType.Color, ArithmeticOperationType.Add)]
    [TestCase(ElementaryOperationParameterType.Color, ArithmeticOperationType.Amplitude)]
    [TestCase(ElementaryOperationParameterType.Value, ArithmeticOperationType.Max)]
    [TestCase(ElementaryOperationParameterType.Value, ArithmeticOperationType.Difference)]
    [TestCase(ElementaryOperationParameterType.Value, ArithmeticOperationType.Divide)]
    [TestCase(ElementaryOperationParameterType.Value, ArithmeticOperationType.Min)]
    [TestCase(ElementaryOperationParameterType.Image, ArithmeticOperationType.Multiply)]
    [TestCase(ElementaryOperationParameterType.Image, ArithmeticOperationType.SubtractLeft)]
    [TestCase(ElementaryOperationParameterType.Image, ArithmeticOperationType.SubtractRight)]
    [AvaloniaTest]
    public async Task GetImageAfterArithmeticQueryHandlerInvokesItsMethods(
        ElementaryOperationParameterType elementaryOperationParameterType,
        ArithmeticOperationType arithmeticOperationType)
    {
        Bitmap response = await _handler.Handle(new GetImageAfterArithmeticQuery
        {
            OperationValue = 1,
            OperationImage = _testImage,
            OperationColor = new Avalonia.Media.Color(100, 100, 100, 255),
            ElementaryOperationParameterType = elementaryOperationParameterType,
            ArithmeticOperationType = arithmeticOperationType
        }, new CancellationToken());

        Assert.That(response.Size.Height, Is.EqualTo(512));
        Assert.That(response.Size.Width, Is.EqualTo(512));
        Assert.That(ImageHelper.ImageToByte(response), Is.EqualTo(ImageHelper.ImageToByte(_testImage)));
        _tabServiceMock.Verify(x => x.CurrentTabName, Times.Once);
        _tabServiceMock.Verify(x => x.GetTab(IsAny<string>()), Times.Once);
        _imageArithmeticServiceMock.Verify(x =>
            x.Execute(IsAny<Bitmap>(), IsAny<object>(), IsAny<ArithmeticOperationType>()), Times.Once);
    }
    
    [AvaloniaTest]
    public async Task GetImageAfterArithmeticQueryHandlerThrowsNullReferenceException()
    {
        _tabServiceMock.Setup(x => x.GetTab(IsAny<string>()))
            .Returns(default(TabItem));
        Assert.ThrowsAsync<NullReferenceException>(async () => await _handler.Handle(new GetImageAfterArithmeticQuery
        {
            OperationValue = 1,
            OperationImage = _testImage,
            OperationColor = new Avalonia.Media.Color(100, 100, 100, 255),
            ElementaryOperationParameterType = ElementaryOperationParameterType.Color,
            ArithmeticOperationType = ArithmeticOperationType.Amplitude
        }, new CancellationToken()), "Actual displayed bitmap does not exist");
    }
    
        
    [AvaloniaTest]
    public async Task GetImageAfterArithmeticQueryHandlerThrowsInvalidOperationException()
    {
        Assert.ThrowsAsync<InvalidOperationException>(async () => await _handler.Handle(new GetImageAfterArithmeticQuery
        {
            OperationValue = 1,
            OperationImage = _testImage,
            OperationColor = new Avalonia.Media.Color(100, 100, 100, 255),
            ElementaryOperationParameterType = (ElementaryOperationParameterType)15,
            ArithmeticOperationType = ArithmeticOperationType.Amplitude
        }, new CancellationToken()), "Invalid operation");
    }
}