using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using Avalonia.Headless.NUnit;
using ImageManipulator.Application.Common.CQRS.Queries.GetImageAfterHistogramEqualization;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Application.Common.Models;
using ImageManipulator.Application.ViewModels;
using ImageManipulator.Domain.Common.CQRS.Interfaces;
using Moq;

namespace Application.UnitTests.CQRS;

[ExcludeFromCodeCoverage]
[TestFixture]
public class GetImageAfterHistogramEqualizationQueryHandlerTests
{
    private Bitmap _testImage;
    private TabItem _sampleTab;
    private Mock<IImagePointOperationsService> _imagePointOperationsServiceMock;
    private GetImageAfterHistogramEqualizationQueryHandler _handler;
    private Mock<ITabService> _tabServiceMock;

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
        _imagePointOperationsServiceMock = new Mock<IImagePointOperationsService>();
        _tabServiceMock = new Mock<ITabService>();
        _tabServiceMock.Setup(x => x.CurrentTabName).Returns("Tab 1");
        _tabServiceMock.Setup(x => x.GetTab(It.IsAny<string>()))
            .Returns(_sampleTab);
        _imagePointOperationsServiceMock.Setup(x =>
                x.HistogramEqualization(It.IsAny<Bitmap>(), It.IsAny<int[]?[]>()))
            .Returns(_testImage);
        _handler = new GetImageAfterHistogramEqualizationQueryHandler(_tabServiceMock.Object, _imagePointOperationsServiceMock.Object);
    }

    [AvaloniaTest]
    public async Task GetImageAfterHistogramEqualizationQueryInvokesMethods()
    {
        int[][] lookupTable = { new[] { 255, 255 } };
        Bitmap response = await _handler.Handle(new GetImageAfterHistogramEqualizationQuery
        {
            LookupTable = lookupTable
        }, new CancellationToken());
        
        Assert.That(response, Is.EqualTo(_testImage));
        _tabServiceMock.Verify(x => x.CurrentTabName, Times.Once);
        _tabServiceMock.Verify(x => x.GetTab(It.IsAny<string>()), Times.Once);
        _imagePointOperationsServiceMock.Verify(x =>
            x.HistogramEqualization(It.IsAny<Bitmap>(), It.Is<int[]?[]>(x => x == lookupTable)), Times.Once);
    }
}