using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using ImageManipulator.Application.Common.CQRS.Queries.GetImageAfterMultiThreshold;
using ImageManipulator.Application.Common.CQRS.Queries.GetPostConvolutionImage;
using ImageManipulator.Application.ViewModels;
using ImageManipulator.Domain.Common.CQRS.Interfaces;
using ImageManipulator.Presentation.Views;
using Moq;

namespace Presentation.UnitTests.Views;

[ExcludeFromCodeCoverage]
public class MultiThresholdViewTests
{
    private Mock<IQueryDispatcher> _queryDispatcherMock;
    private Window _window;
    [SetUp]
    public async Task SetUp()
    {
        _queryDispatcherMock = new Mock<IQueryDispatcher>();
        _queryDispatcherMock.Setup(x =>
            x.Dispatch<GetPostConvolutionImageQuery, Bitmap>(
                It.IsAny<GetPostConvolutionImageQuery>(), It.IsAny<CancellationToken>()));
        
        _window = new Window()
        {
            Content = new MultiThresholdView()
            {
                ViewModel = new MultiThresholdViewModel(_queryDispatcherMock.Object)
                {
                    EnteredUpperThreshold = 23,
                    EnteredLowerThreshold = 15,
                    LivePreview = true,
                    ReplaceColours = true
                }
            }
        };
        
        _window.Show();
    }

    [AvaloniaTest]
    public async Task ConvolutionCommandExecutesDispatcher()
    {
        var button = ((MultiThresholdView)_window.Content).FindControl<Button>("ThresholdCommand");
        button.Command.Execute(default);
        
        _queryDispatcherMock.Verify(x =>
            x.Dispatch<GetImageAfterMultiThresholdQuery, Bitmap>(
                It.IsAny<GetImageAfterMultiThresholdQuery>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    [AvaloniaTest]
    public async Task CheckBoxSetAsExpected()
    {
        var box = ((MultiThresholdView)_window.Content).FindControl<CheckBox>("LivePreview");
        
        Assert.That(box.IsChecked, Is.EqualTo(true));
    }
    
    [AvaloniaTest]
    public async Task SlidersValuesSetAsExpected()
    {
        var lowerValueSlider = ((MultiThresholdView)_window.Content).FindControl<Slider>("lowerThreshold");
        var upperValueSlider = ((MultiThresholdView)_window.Content).FindControl<Slider>("upperThreshold");
        
        Assert.That(lowerValueSlider.Value, Is.EqualTo(15));
        Assert.That(upperValueSlider.Value, Is.EqualTo(23));
    }
    
    [AvaloniaTest]
    public async Task AcceptCommandClosesWindow()
    {
        int raisedCount = 0;
        _window.Closed += (_, _) => raisedCount++;
        
        var button = ((MultiThresholdView)_window.Content).FindControl<Button>("AcceptCommand");
        button.Command.Execute(_window);
        
        Assert.That(raisedCount, Is.EqualTo(1));
    }
    
    [AvaloniaTest]
    public async Task CloseCommandClosesWindow()
    {
        int raisedCount = 0;
        _window.Closed += (_, _) => raisedCount++;
        
        var button = ((MultiThresholdView)_window.Content).FindControl<Button>("CancelCommand");
        button.Command.Execute(_window);
        
        Assert.That(raisedCount, Is.EqualTo(1));
    }
}