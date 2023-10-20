using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using ImageManipulator.Application.Common.CQRS.Command.AddImageVersion;
using ImageManipulator.Application.Common.Interfaces;
using Moq;

namespace Application.UnitTests.CQRS;

[ExcludeFromCodeCoverage]
[TestFixture]
public class AddImageVersionCommandHandlerTests
{
    private Mock<IImageHistoryService> _imageHistoryServiceMock;
    private AddImageVersionCommandHandler _handler;
    
    [SetUp]
    public async Task SetUp()
    {
        _imageHistoryServiceMock = new Mock<IImageHistoryService>();
        _imageHistoryServiceMock
            .Setup(x => x.StoreCurrentVersionAndGetThumbnail(It.IsAny<Bitmap>(), It.IsAny<string>()))
            .ReturnsAsync(new Bitmap(300, 300));
        _handler = new AddImageVersionCommandHandler(_imageHistoryServiceMock.Object);
    }

    [Test]
    public async Task AddImageVersionCommandHandlerInvokesExpectedMethodTest()
    {
        Bitmap bitmap = new Bitmap(500, 500);
        
        Bitmap response = await _handler.Handle(new AddImageVersionCommand()
        {
            Image = bitmap,
            Path = "path"
        }, new CancellationToken());
        
        Assert.That(response.Height, Is.EqualTo(300));
        Assert.That(response.Width, Is.EqualTo(300));
        _imageHistoryServiceMock.Verify(x =>
                x.StoreCurrentVersionAndGetThumbnail(It.Is<Bitmap>(x => x == bitmap), It.Is<string>(x => x == "path")),
            Times.Once);
    }
}