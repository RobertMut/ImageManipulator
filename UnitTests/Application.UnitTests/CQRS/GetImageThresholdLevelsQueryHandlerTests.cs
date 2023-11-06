using System.Diagnostics.CodeAnalysis;
using Avalonia.Headless.NUnit;
using ImageManipulator.Application.Common.CQRS.Queries.GetImageThresholdLevels;
using ImageManipulator.Application.Common.Interfaces;
using Moq;

namespace Application.UnitTests.CQRS;

[ExcludeFromCodeCoverage]
[TestFixture]
public class GetImageThresholdLevelsQueryHandlerTests
{
    private Mock<IImagePointOperationsService> _imagePointOperationsServiceMock;
    private GetImageThresholdLevelsQueryHandler _handler;

    [SetUp]
    public async Task SetUp()
    {
        _imagePointOperationsServiceMock = new Mock<IImagePointOperationsService>();
        _imagePointOperationsServiceMock.Setup(x =>
            x.CalculateLowerImageThresholdPoint(It.IsAny<int[]>()))
            .Returns(30);
        _imagePointOperationsServiceMock.Setup(x =>
                x.CalculateUpperImageThresholdPoint(It.IsAny<int[]>()))
            .Returns(60);
        _handler = new GetImageThresholdLevelsQueryHandler(_imagePointOperationsServiceMock.Object);
    }

    [AvaloniaTest]
    public async Task GetImageAfterThresholdQueryHandlerInvokesMethods()
    {
        ThresholdLevels expected = new ThresholdLevels
        {
            Upper = 60,
            Lower = 30
        };
        int[] histogram = { 1, 1, 1, 1 };
        
        ThresholdLevels response = await _handler.Handle(new GetImageThresholdLevelsQuery
        {
            HistogramValues = histogram
        }, new CancellationToken());
        
        Assert.That(response.Lower, Is.EqualTo(expected.Lower));
        Assert.That(response.Upper, Is.EqualTo(expected.Upper));

        _imagePointOperationsServiceMock.Verify(x =>
            x.CalculateLowerImageThresholdPoint(It.Is<int[]>(x => x == histogram)));
        _imagePointOperationsServiceMock.Verify(x =>
            x.CalculateUpperImageThresholdPoint(It.Is<int[]>(x => x == histogram)));
    }
}