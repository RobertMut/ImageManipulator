using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using ImageManipulator.Application.Common.CQRS.Queries.GetImageAfterMultiThreshold;
using ImageManipulator.Application.Common.CQRS.Queries.GetImageAfterThreshold;
using ImageManipulator.Application.Common.CQRS.Queries.GetPostConvolutionImage;
using ImageManipulator.Application.ViewModels;
using ImageManipulator.Domain.Common.CQRS.Interfaces;
using ImageManipulator.Presentation.Views;
using Moq;

namespace Presentation.UnitTests.Views;

[ExcludeFromCodeCoverage]
public class ThresholdViewTests
{
    private Mock<IQueryDispatcher> _queryDispatcherMock;
    private Window _window;
    [SetUp]
    public async Task SetUp()
    {
        _queryDispatcherMock = new Mock<IQueryDispatcher>();
        _queryDispatcherMock.Setup(x =>
            x.Dispatch<GetImageAfterThresholdQuery, Bitmap>(
                It.IsAny<GetImageAfterThresholdQuery>(), It.IsAny<CancellationToken>()));
        
        _window = new Window
        {
            Content = new ThresholdView
            {
                ViewModel = new ThresholdViewModel(_queryDispatcherMock.Object)
                {
                    LivePreview = true,
                    ReplaceColours = true,
                    EnteredThreshold = 15
                }
            }
        };
        
        _window.Show();
    }

    [AvaloniaTest]
    public async Task ThresholdCommandExecutesDispatcher()
    {
        var button = ((ThresholdView)_window.Content).FindControl<Button>("ThresholdCommand");
        button.Command.Execute(default);
        
        _queryDispatcherMock.Verify(x =>
            x.Dispatch<GetImageAfterThresholdQuery, Bitmap>(
                It.IsAny<GetImageAfterThresholdQuery>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    [AvaloniaTest]
    public async Task CheckBoxSetAsExpected()
    {
        var box = ((ThresholdView)_window.Content).FindControl<CheckBox>("LivePreview");
        
        Assert.That(box.IsChecked, Is.EqualTo(true));
    }
    
    [AvaloniaTest]
    public async Task SlidersValuesSetAsExpected()
    {
        var sliderValue = ((ThresholdView)_window.Content).FindControl<Slider>("LowerThreshold");
        
        Assert.That(sliderValue.Value, Is.EqualTo(15));
    }
    
    [AvaloniaTest]
    public async Task AcceptCommandClosesWindow()
    {
        int raisedCount = 0;
        _window.Closed += (_, _) => raisedCount++;
        
        var button = ((ThresholdView)_window.Content).FindControl<Button>("AcceptCommand");
        button.Command.Execute(_window);
        
        Assert.That(raisedCount, Is.EqualTo(1));
    }
    
    [AvaloniaTest]
    public async Task CloseCommandClosesWindow()
    {
        int raisedCount = 0;
        _window.Closed += (_, _) => raisedCount++;
        
        var button = ((ThresholdView)_window.Content).FindControl<Button>("CancelCommand");
        button.Command.Execute(_window);
        
        Assert.That(raisedCount, Is.EqualTo(1));
    }
}