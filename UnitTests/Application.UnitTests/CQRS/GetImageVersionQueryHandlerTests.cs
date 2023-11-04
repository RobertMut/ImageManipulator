using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using Avalonia.Headless.NUnit;
using ImageManipulator.Application.Common.CQRS.Queries.GetImageAfterThreshold;
using ImageManipulator.Application.Common.CQRS.Queries.GetImageValues;
using ImageManipulator.Application.Common.CQRS.Queries.GetImageVersion;
using ImageManipulator.Application.Common.CQRS.Queries.GetImageVersions;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Application.Common.Models;
using ImageManipulator.Application.ViewModels;
using ImageManipulator.Domain.Common.CQRS.Interfaces;
using Moq;
using UnitTests.Core;

namespace Application.UnitTests.CQRS;

[ExcludeFromCodeCoverage]
[TestFixture]
public class GetImageVersionQueryHandlerTests
{
    private Bitmap _testImage;
    private TabItem _sampleTab;
    private Mock<IImageHistoryService> _imageHistoryServiceMock;
    private GetImageVersionQueryHandler _handler;

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
        _imageHistoryServiceMock = new Mock<IImageHistoryService>();
        _imageHistoryServiceMock.Setup(x => x.RestoreVersion(It.IsAny<string>(), It.IsAny<int>()))
            .Returns(_testImage);
        _handler = new GetImageVersionQueryHandler(_imageHistoryServiceMock.Object);
    }

    [AvaloniaTest]
    public async Task GetImageVersionQueryHandlerInvokesMethods()
    {
        Bitmap response = await _handler.Handle(new GetImageVersionQuery
        {
            Path = "test/path",
            Version = 1
        }, new CancellationToken());
        
        Assert.That(ImageHelper.ImageToByte(response), Is.EqualTo(ImageHelper.ImageToByte(_testImage)));

        _imageHistoryServiceMock.Verify(
            x => x.RestoreVersion(It.Is<string>(x => x == "test/path"), It.Is<int>(y => y == 1)),
            Times.Once);
    }
}