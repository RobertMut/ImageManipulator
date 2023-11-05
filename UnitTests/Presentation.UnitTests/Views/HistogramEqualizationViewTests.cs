using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Avalonia.Layout;
using Avalonia.Media;
using ImageManipulator.Application.Common.CQRS.Queries.GetImageAfterHistogramEqualization;
using ImageManipulator.Application.ViewModels;
using ImageManipulator.Domain.Common.CQRS.Interfaces;
using ImageManipulator.Presentation.Views;
using Moq;

namespace Presentation.UnitTests.Views;

[ExcludeFromCodeCoverage]
public class HistogramEqualizationViewTests
{
    private Mock<IQueryDispatcher> _queryDispatcherMock;
    private Window _window;
    
    [SetUp]
    public async Task SetUp()
    {
        _queryDispatcherMock = new Mock<IQueryDispatcher>();
        _queryDispatcherMock.Setup(x =>
            x.Dispatch<GetImageAfterHistogramEqualizationQuery, Bitmap>(
                It.IsAny<GetImageAfterHistogramEqualizationQuery>(), It.IsAny<CancellationToken>()));
        
        _window = new Window()
        {
            Content = new HistogramEqualizationView()
            {
                ViewModel = new HistogramEqualizationViewModel(_queryDispatcherMock.Object)
            }
        };
        
        _window.Show();
    }

    [AvaloniaTest]
    public async Task EqualizeCommandExecutesDispatcher()
    {
        var button = ((HistogramEqualizationView)_window.Content).FindControl<Button>("EqualizeCommand");
        button.Command.Execute(default);
        
        _queryDispatcherMock.Verify(x =>
            x.Dispatch<GetImageAfterHistogramEqualizationQuery, Bitmap>(
                It.IsAny<GetImageAfterHistogramEqualizationQuery>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    [AvaloniaTest]
    public async Task AcceptCommandClosesWindow()
    {
        int raisedCount = 0;
        _window.Closed += (sender, args) => raisedCount++;
        
        var button = ((HistogramEqualizationView)_window.Content).FindControl<Button>("AcceptCommand");
        button.Command.Execute(_window);
        
        Assert.That(raisedCount, Is.EqualTo(1));
    }
    
    [AvaloniaTest]
    public async Task CloseCommandClosesWindow()
    {
        int raisedCount = 0;
        _window.Closed += (sender, args) => raisedCount++;
        
        var button = ((HistogramEqualizationView)_window.Content).FindControl<Button>("CancelCommand");
        button.Command.Execute(_window);
        
        Assert.That(raisedCount, Is.EqualTo(1));
    }
}