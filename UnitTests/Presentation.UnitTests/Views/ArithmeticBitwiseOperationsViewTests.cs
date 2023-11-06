using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using ImageManipulator.Application.Common.CQRS.Queries.GetImageAfterArithmetic;
using ImageManipulator.Application.Common.CQRS.Queries.GetImageAfterBitwise;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Application.ViewModels;
using ImageManipulator.Domain.Common.CQRS.Interfaces;
using ImageManipulator.Presentation.Views;
using Moq;
using UnitTests.Core;

namespace Presentation.UnitTests.Views;

[ExcludeFromCodeCoverage]
public class ArithmeticBitwiseOperationsViewTests
{
    private Mock<IQueryDispatcher> _queryDispatcherMock;
    private Mock<ICommonDialogService> _commonDialogServiceMock;
    private Window _window;
    
    [SetUp]
    public async Task SetUp()
    {
        _queryDispatcherMock = new Mock<IQueryDispatcher>();
        _commonDialogServiceMock = new Mock<ICommonDialogService>();
        _queryDispatcherMock.Setup(x =>
            x.Dispatch<GetImageAfterArithmeticQuery, Bitmap>(
                It.IsAny<GetImageAfterArithmeticQuery>(), It.IsAny<CancellationToken>()));
        _queryDispatcherMock.Setup(x =>
            x.Dispatch<GetImageAfterBitwiseQuery, Bitmap>(
                It.IsAny<GetImageAfterBitwiseQuery>(), It.IsAny<CancellationToken>()));
        
        _window = new Window()
        {
            Content = new ArithmeticBitwiseOperationsView()
            {
                ViewModel = new ArithmeticBitwiseOperationsViewModel(_queryDispatcherMock.Object, 
                    _commonDialogServiceMock.Object)
                {
                    OperationImage = new Bitmap(100, 100),
                    PickedColor = Colors.Blue,
                    Value = 1,
                    IsArithmeticSelected = false,
                    SelectedElementaryOperation = 4,
                    SelectedArithmeticOperation = 2,
                    SelectedBitwiseOperation = 3
                }
            }
        };
        
        _window.Show();
    }

    [AvaloniaTest]
    [TestCase(true)]
    [TestCase(false)]
    public async Task EqualizeCommandExecutesBitwiseDispatcher(bool isArithmeticSelected)
    {
        ArithmeticBitwiseOperationsView? view = ((ArithmeticBitwiseOperationsView)_window.Content);
        view.ViewModel.IsArithmeticSelected = isArithmeticSelected;
        
        var button = view.FindControl<Button>("OperationCommand");
        button.Command.Execute(default);
        
        _queryDispatcherMock.Verify(x =>
            x.Dispatch<GetImageAfterBitwiseQuery, Bitmap>(
                It.IsAny<GetImageAfterBitwiseQuery>(), It.IsAny<CancellationToken>()),
            isArithmeticSelected ? Times.Never : Times.Once);
        _queryDispatcherMock.Verify(x =>
                x.Dispatch<GetImageAfterArithmeticQuery, Bitmap>(
                    It.IsAny<GetImageAfterArithmeticQuery>(), It.IsAny<CancellationToken>()),
            !isArithmeticSelected ? Times.Never : Times.Once);
    }

    [AvaloniaTest]
    public async Task SelectImageCommandExecutes()
    {
        var testImage = new Bitmap("Resources/image.png");

        Mock<IStorageFile> storageFileMock = new Mock<IStorageFile>();
        storageFileMock.Setup(x => x.OpenReadAsync())
            .ReturnsAsync(() =>
            {
                MemoryStream ms = new MemoryStream();
                testImage.Save(ms, ImageFormat.Png);

                return ms;
            });
        
        _commonDialogServiceMock.Setup(x => x.ShowFileDialogInNewWindow())
            .ReturnsAsync(storageFileMock.Object);
        
        var button = ((ArithmeticBitwiseOperationsView)_window.Content).FindControl<Button>("SelectImageButton");
        
        button.Command.Execute(null);

        _commonDialogServiceMock.Verify(x => x.ShowFileDialogInNewWindow(), Times.Once);
        ((ArithmeticBitwiseOperationsView)_window.Content).ViewModel.OperationImage.Compare(testImage);
    }
    
    [AvaloniaTest]
    public async Task ColorPickerSetAsExpected()
    {
        var picker = ((ArithmeticBitwiseOperationsView)_window.Content).FindControl<AvaloniaColorPicker.ColorButton>("ColorPicker");
        
        Assert.That(picker.Color, Is.EqualTo(Colors.Blue));
    }
    
    [AvaloniaTest]
    public async Task AcceptCommandClosesWindow()
    {
        int raisedCount = 0;
        _window.Closed += (_, _) => raisedCount++;
        
        var button = ((ArithmeticBitwiseOperationsView)_window.Content).FindControl<Button>("AcceptCommand");
        button.Command.Execute(_window);
        
        Assert.That(raisedCount, Is.EqualTo(1));
    }
    
    [AvaloniaTest]
    public async Task CloseCommandClosesWindow()
    {
        int raisedCount = 0;
        _window.Closed += (_, _) => raisedCount++;
        
        var button = ((ArithmeticBitwiseOperationsView)_window.Content).FindControl<Button>("CancelCommand");
        button.Command.Execute(_window);
        
        Assert.That(raisedCount, Is.EqualTo(1));
    }
}