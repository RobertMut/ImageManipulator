using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using Avalonia.Headless.NUnit;
using Core;
using ImageManipulator.Application.Common.CQRS.Queries.GetImageVersions;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Application.Common.Models;
using ImageManipulator.Application.ViewModels;
using ImageManipulator.Domain.Common.CQRS.Interfaces;
using Moq;

namespace Application.UnitTests.CQRS;

[ExcludeFromCodeCoverage]
[TestFixture]
public class GetImageVersionsQueryHandlerTests
{
    private Bitmap _testImage;
    private TabItem _sampleTab;
    private Mock<IImageHistoryService> _imageHistoryServiceMock;
    private GetImageVersionsQueryHandler _handler;

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
        _imageHistoryServiceMock.Setup(x => x.GetVersions(It.IsAny<string>()))
            .ReturnsAsync(new List<Bitmap> { _testImage });
        _handler = new GetImageVersionsQueryHandler(_imageHistoryServiceMock.Object);
    }

    [AvaloniaTest]
    public async Task GetImageVersionsQueryHandlerInvokesMethods()
    {
        IEnumerable<Bitmap> expected = new List<Bitmap>() { _testImage }.AsEnumerable();
        IEnumerable<Bitmap> response = await _handler.Handle(new GetImageVersionsQuery
        {
            Path = "test/path"
        }, new CancellationToken());

        Assert.That(response.Count(), Is.EqualTo(expected.Count()));
        
        for (int i = 0; i < expected.Count(); i++)
        {
            byte[] responseBytes = ImageHelper.ImageToByte(response.ElementAt(i));
            byte[] expectedBytes = ImageHelper.ImageToByte(expected.ElementAt(i));
            Assert.That(responseBytes, Is.EqualTo(expectedBytes));
        }  
        
        _imageHistoryServiceMock.Verify(x => x.GetVersions(It.Is<string>(x => x == "test/path")),
            Times.Once);
    }
}