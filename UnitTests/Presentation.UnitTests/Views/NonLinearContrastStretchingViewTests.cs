using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using ImageManipulator.Application.Common.CQRS.Queries.GetImageAfterContrastStretch;
using ImageManipulator.Application.Common.CQRS.Queries.GetImageAfterMultiThreshold;
using ImageManipulator.Application.Common.CQRS.Queries.GetImageAfterNonLinearContrastStretching;
using ImageManipulator.Application.Common.CQRS.Queries.GetImageThresholdLevels;
using ImageManipulator.Application.Common.CQRS.Queries.GetPostConvolutionImage;
using ImageManipulator.Application.ViewModels;
using ImageManipulator.Domain.Common.CQRS.Interfaces;
using ImageManipulator.Presentation.Views;
using Moq;

namespace Presentation.UnitTests.Views;

[ExcludeFromCodeCoverage]
public class NonLinearContrastStretchingViewTests
{
    private Mock<IQueryDispatcher> _queryDispatcherMock;
    private Window _window;
    [SetUp]
    public async Task SetUp()
    {
        _queryDispatcherMock = new Mock<IQueryDispatcher>();
        _queryDispatcherMock.Setup(x =>
            x.Dispatch<GetImageAfterNonLinearContrastStretchingQuery, Bitmap>(
                It.IsAny<GetImageAfterNonLinearContrastStretchingQuery>(), It.IsAny<CancellationToken>()));
        
        _window = new Window()
        {
            Content = new NonLinearContrastStretchingView()
            {
                ViewModel = new NonLinearContrastStretchingViewModel(_queryDispatcherMock.Object)
                {
                    BeforeImage = null,
                    AfterImage = null,
                    GammaValue = 1.5d
                }
            }
        };
        
        _window.Show();
    }

    [AvaloniaTest]
    public async Task ContrastStretchingExecutesDispatcherOnStretchingCommand()
    {
        _queryDispatcherMock.Setup(x =>
            x.Dispatch<GetImageAfterNonLinearContrastStretchingQuery, Bitmap>(
                It.IsAny<GetImageAfterNonLinearContrastStretchingQuery>(), It.IsAny<CancellationToken>()));
        
        var button = ((NonLinearContrastStretchingView)_window.Content).FindControl<Button>("StretchCommand");
        button.Command.Execute(default);
        
        _queryDispatcherMock.Verify(x =>
            x.Dispatch<GetImageAfterNonLinearContrastStretchingQuery, Bitmap>(
                It.IsAny<GetImageAfterNonLinearContrastStretchingQuery>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [AvaloniaTest]
    public async Task AcceptCommandClosesWindow()
    {
        int raisedCount = 0;
        _window.Closed += (_, _) => raisedCount++;
        
        var button = ((NonLinearContrastStretchingView)_window.Content).FindControl<Button>("AcceptCommand");
        button.Command.Execute(_window);
        
        Assert.That(raisedCount, Is.EqualTo(1));
    }
    
    [AvaloniaTest]
    public async Task CloseCommandClosesWindow()
    {
        int raisedCount = 0;
        _window.Closed += (_, _) => raisedCount++;
        
        var button = ((NonLinearContrastStretchingView)_window.Content).FindControl<Button>("CancelCommand");
        button.Command.Execute(_window);
        
        Assert.That(raisedCount, Is.EqualTo(1));
    }
}