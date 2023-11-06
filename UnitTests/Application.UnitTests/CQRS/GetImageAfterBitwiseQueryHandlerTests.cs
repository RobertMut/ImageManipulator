using System.Diagnostics.CodeAnalysis;
using Avalonia.Headless.NUnit;
using ImageManipulator.Application.Common.CQRS.Queries.GetImageAfterBitwise;
using ImageManipulator.Application.Common.Enums;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Application.Common.Models;
using ImageManipulator.Application.ViewModels;
using ImageManipulator.Common.Enums;
using ImageManipulator.Domain.Common.CQRS.Interfaces;
using Moq;
using UnitTests.Core;
using static Moq.It;
using Bitmap = System.Drawing.Bitmap;
using Is = NUnit.Framework.Is;

namespace Application.UnitTests.CQRS;

[ExcludeFromCodeCoverage]
[TestFixture]
public class GetImageAfterBitwiseQueryHandlerTests
{
    private Mock<IImageBitwiseService> _imageBitwiseServiceMock;
    private Mock<ITabService> _tabServiceMock;
    private GetImageAfterBitwiseQueryHandler _handler;
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
        _imageBitwiseServiceMock = new Mock<IImageBitwiseService>();
        _tabServiceMock = new Mock<ITabService>();
        _tabServiceMock.Setup(x => x.CurrentTabName).Returns("Tab 1");
        _tabServiceMock.Setup(x => x.GetTab(IsAny<string>()))
            .Returns(_sampleTab);
        _imageBitwiseServiceMock.Setup(x =>
                x.Execute(IsAny<Bitmap>(), IsAny<object>(), IsAny<BitwiseOperationType>()))
            .Returns(_testImage);
        _handler = new GetImageAfterBitwiseQueryHandler(_tabServiceMock.Object, _imageBitwiseServiceMock.Object);
    }

    [TestCase(ElementaryOperationParameterType.Image, BitwiseOperationType.LeftShift)]
    [TestCase(ElementaryOperationParameterType.Value, BitwiseOperationType.RightShift)]
    [TestCase(ElementaryOperationParameterType.Color, BitwiseOperationType.OR)]
    [TestCase(ElementaryOperationParameterType.Color, BitwiseOperationType.AND)]
    [TestCase(ElementaryOperationParameterType.Color, BitwiseOperationType.XOR)]
    [TestCase(ElementaryOperationParameterType.Value, BitwiseOperationType.NOT)]
    [AvaloniaTest]
    public async Task GetImageAfterBitwiseQueryHandlerInvokesItsMethods(
        ElementaryOperationParameterType elementaryOperationParameterType,
        BitwiseOperationType bitwiseOperationType)
    {
        Bitmap response = await _handler.Handle(new GetImageAfterBitwiseQuery
        {
            OperationValue = 1,
            OperationImage = _testImage,
            OperationColor = new Avalonia.Media.Color(100, 100, 100, 255),
            ElementaryOperationParameterType = elementaryOperationParameterType,
            BitwiseOperationType = bitwiseOperationType
        }, new CancellationToken());

        Assert.That(response.Size.Height, Is.EqualTo(512));
        Assert.That(response.Size.Width, Is.EqualTo(512));
        Assert.That(ImageHelper.ImageToByte(response), Is.EqualTo(ImageHelper.ImageToByte(_testImage)));
        _tabServiceMock.Verify(x => x.CurrentTabName, Times.Once);
        _tabServiceMock.Verify(x => x.GetTab(IsAny<string>()), Times.Once);
        _imageBitwiseServiceMock.Verify(x =>
            x.Execute(IsAny<Bitmap>(), IsAny<object>(), IsAny<BitwiseOperationType>()), Times.Once);
    }
    
    [AvaloniaTest]
    public async Task GetImageAfterBitwiseQueryHandlerThrowsNullReferenceException()
    {
        _tabServiceMock.Setup(x => x.GetTab(IsAny<string>()))
            .Returns(default(TabItem));
        Assert.ThrowsAsync<NullReferenceException>(async () => await _handler.Handle(new GetImageAfterBitwiseQuery
        {
            OperationValue = 1,
            OperationImage = _testImage,
            OperationColor = new Avalonia.Media.Color(100, 100, 100, 255),
            ElementaryOperationParameterType = ElementaryOperationParameterType.Color,
            BitwiseOperationType = BitwiseOperationType.OR
        }, new CancellationToken()), "Actual displayed bitmap does not exist");
    }
    
        
    [AvaloniaTest]
    public async Task GetImageAfterBitwiseQueryHandlerThrowsInvalidOperationException()
    {
        Assert.ThrowsAsync<InvalidOperationException>(async () => await _handler.Handle(new GetImageAfterBitwiseQuery
        {
            OperationValue = 1,
            OperationImage = _testImage,
            OperationColor = new Avalonia.Media.Color(100, 100, 100, 255),
            ElementaryOperationParameterType = (ElementaryOperationParameterType)15,
            BitwiseOperationType = BitwiseOperationType.RightShift
        }, new CancellationToken()), "Invalid operation");
    }
}