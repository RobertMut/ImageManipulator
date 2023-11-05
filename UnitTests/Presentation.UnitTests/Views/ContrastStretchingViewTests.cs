using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using ImageManipulator.Application.Common.CQRS.Queries.GetImageAfterContrastStretch;
using ImageManipulator.Application.Common.CQRS.Queries.GetImageAfterMultiThreshold;
using ImageManipulator.Application.Common.CQRS.Queries.GetImageThresholdLevels;
using ImageManipulator.Application.Common.CQRS.Queries.GetPostConvolutionImage;
using ImageManipulator.Application.ViewModels;
using ImageManipulator.Domain.Common.CQRS.Interfaces;
using ImageManipulator.Presentation.Views;
using Moq;

namespace Presentation.UnitTests.Views;

[ExcludeFromCodeCoverage]
public class ContrastStretchingViewTests
{
    private Mock<IQueryDispatcher> _queryDispatcherMock;
    private Window _window;
    [SetUp]
    public async Task SetUp()
    {
        _queryDispatcherMock = new Mock<IQueryDispatcher>();
        _queryDispatcherMock.Setup(x =>
            x.Dispatch<GetImageThresholdLevelsQuery, ThresholdLevels>(
                It.IsAny<GetImageThresholdLevelsQuery>(), It.IsAny<CancellationToken>()));
        
        _window = new Window()
        {
            Content = new ContrastStretchingView()
            {
                ViewModel = new ContrastStretchingViewModel(_queryDispatcherMock.Object)
                {
                    EnteredUpperThreshold = 23,
                    EnteredLowerThreshold = 15,
                    HistogramValues = new[] { 10}
                }
            }
        };
        
        _window.Show();
    }

    [AvaloniaTest]
    public async Task ContrastStretchingExecutesDispatcherOnStretchingCommand()
    {
        _queryDispatcherMock.Setup(x =>
            x.Dispatch<GetImageAfterContrastStretchQuery, Bitmap>(
                It.IsAny<GetImageAfterContrastStretchQuery>(), It.IsAny<CancellationToken>()));
        
        var button = ((ContrastStretchingView)_window.Content).FindControl<Button>("StretchingCommand");
        button.Command.Execute(default);
        
        _queryDispatcherMock.Verify(x =>
            x.Dispatch<GetImageAfterContrastStretchQuery, Bitmap>(
                It.IsAny<GetImageAfterContrastStretchQuery>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    [AvaloniaTest]
    public async Task TextBoxSetAsExpected()
    {
        var lowerThreshold = ((ContrastStretchingView)_window.Content).FindControl<TextBlock>("SuggestedLowerThreshold");
        var upperThreshold = ((ContrastStretchingView)_window.Content).FindControl<TextBlock>("SuggestedUpperThreshold");

        Assert.That(lowerThreshold.Text, Is.EqualTo("Suggested lower threshold="));
        Assert.That(upperThreshold.Text, Is.EqualTo("Suggested upper threshold="));
    }
 
    [AvaloniaTest]
    public async Task ContrastStretchingExecutesDispatcherOnLaunch()
    {
        _queryDispatcherMock.Verify(x =>
            x.Dispatch<GetImageThresholdLevelsQuery, ThresholdLevels>(
                It.IsAny<GetImageThresholdLevelsQuery>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [AvaloniaTest]
    public async Task AcceptCommandClosesWindow()
    {
        int raisedCount = 0;
        _window.Closed += (_, _) => raisedCount++;
        
        var button = ((ContrastStretchingView)_window.Content).FindControl<Button>("AcceptCommand");
        button.Command.Execute(_window);
        
        Assert.That(raisedCount, Is.EqualTo(1));
    }
    
    [AvaloniaTest]
    public async Task CloseCommandClosesWindow()
    {
        int raisedCount = 0;
        _window.Closed += (_, _) => raisedCount++;
        
        var button = ((ContrastStretchingView)_window.Content).FindControl<Button>("CancelCommand");
        button.Command.Execute(_window);
        
        Assert.That(raisedCount, Is.EqualTo(1));
    }
}