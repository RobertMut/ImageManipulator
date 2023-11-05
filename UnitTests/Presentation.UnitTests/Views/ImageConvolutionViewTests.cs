using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Avalonia.Layout;
using Avalonia.Media;
using ImageManipulator.Application.Common.CQRS.Queries.GetImageAfterHistogramEqualization;
using ImageManipulator.Application.Common.CQRS.Queries.GetPostConvolutionImage;
using ImageManipulator.Application.ViewModels;
using ImageManipulator.Domain.Common.CQRS.Interfaces;
using ImageManipulator.Presentation.Views;
using Moq;

namespace Presentation.UnitTests.Views;

[ExcludeFromCodeCoverage]
public class ImageConvolutionViewTests
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
            Content = new ImageConvolutionView()
            {
                ViewModel = new ImageConvolutionViewModel(_queryDispatcherMock.Object)
                {
                    ImageWrap = 2
                }
            }
        };
        
        _window.Show();
    }

    [AvaloniaTest]
    public async Task ConvolutionCommandExecutesDispatcher()
    {
        var button = ((ImageConvolutionView)_window.Content).FindControl<Button>("ConvolutionCommand");
        button.Command.Execute(default);
        
        _queryDispatcherMock.Verify(x =>
            x.Dispatch<GetPostConvolutionImageQuery, Bitmap>(
                It.IsAny<GetPostConvolutionImageQuery>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    [AvaloniaTest]
    public async Task ImageWrapSetAsExpected()
    {
        var combo = ((ImageConvolutionView)_window.Content).FindControl<ComboBox>("BorderCombo");
        
        Assert.That(combo.SelectedIndex, Is.EqualTo(2));
    }
    
    [AvaloniaTest]
    public async Task AcceptCommandClosesWindow()
    {
        int raisedCount = 0;
        _window.Closed += (sender, args) => raisedCount++;
        
        var button = ((ImageConvolutionView)_window.Content).FindControl<Button>("AcceptCommand");
        button.Command.Execute(_window);
        
        Assert.That(raisedCount, Is.EqualTo(1));
    }
    
    [AvaloniaTest]
    public async Task CloseCommandClosesWindow()
    {
        int raisedCount = 0;
        _window.Closed += (sender, args) => raisedCount++;
        
        var button = ((ImageConvolutionView)_window.Content).FindControl<Button>("CancelCommand");
        button.Command.Execute(_window);
        
        Assert.That(raisedCount, Is.EqualTo(1));
    }
}