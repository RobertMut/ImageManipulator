using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using Avalonia.Headless.NUnit;
using ImageManipulator.Application.Common.CQRS.Queries.GetImageValues;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Application.Common.Models;
using ImageManipulator.Application.ViewModels;
using ImageManipulator.Domain.Common.CQRS.Interfaces;
using Moq;

namespace Application.UnitTests.CQRS;

[ExcludeFromCodeCoverage]
[TestFixture]
public class GetImageValuesQueryHandlerTests
{
    private Bitmap _testImage;
    private TabItem _sampleTab;
    private Mock<IImageDataService> _imageDataServiceMock;
    private GetImageValuesQueryHandler _handler;

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
        _imageDataServiceMock = new Mock<IImageDataService>();
        _imageDataServiceMock.Setup(x => x.CalculateLevels(It.IsAny<Bitmap>()))
            .Returns(new[] { new[] { 1, 2 ,3 ,4}});
        _imageDataServiceMock.Setup(x => x.CalculateAverageForGrayGraph(It.IsAny<int[]?[]>()))
            .Returns(new[] { 1,2,3,4,5});
        _handler = new GetImageValuesQueryHandler(_imageDataServiceMock.Object);
    }

    [AvaloniaTest]
    public async Task GetImageValuesQueryHandlerInvokesMethods()
    {
        int[][] expected = { new[] { 1, 2, 3, 4, 5 } };
        int[][] response = await _handler.Handle(new GetImageValuesQuery
        {
            Luminance = true,
            Image = _testImage
        }, new CancellationToken());
        
        Assert.That(response, Is.EqualTo(expected));
        _imageDataServiceMock.Verify(x => x.CalculateLevels(It.IsAny<Bitmap>()), Times.Once);
        _imageDataServiceMock.Verify(x => x.CalculateAverageForGrayGraph(It.IsAny<int[]?[]>()), Times.Once);
    }
}