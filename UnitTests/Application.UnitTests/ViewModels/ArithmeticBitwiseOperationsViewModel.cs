using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using Avalonia.Headless.NUnit;
using Avalonia.Platform.Storage;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Application.ViewModels;
using ImageManipulator.Domain.Common.CQRS.Interfaces;
using Moq;

namespace Application.UnitTests.ViewModels;

[ExcludeFromCodeCoverage]
[TestFixture]
public class ArithmeticBitwiseOperationsViewModelTests
{
    private ArithmeticBitwiseOperationsViewModel _arithmeticBitwiseOperationsViewModel;
    private Mock<ICommonDialogService> _commonDialogServiceMock;
    private Bitmap _testImage;

    [SetUp]
    public async Task SetUp()
    {
        _testImage = new Bitmap("Resources/image.png");
        _commonDialogServiceMock = new Mock<ICommonDialogService>();
        _arithmeticBitwiseOperationsViewModel =
            new ArithmeticBitwiseOperationsViewModel(Mock.Of<IQueryDispatcher>(), _commonDialogServiceMock.Object);
    }

    [AvaloniaTest]
    [TestCase(true)]
    [TestCase(false)]
    public async Task ArithmeticBitwiseOperationsViewModelExecute(bool isArithmeticSelected)
    {
        _arithmeticBitwiseOperationsViewModel.IsArithmeticSelected = isArithmeticSelected;
        _arithmeticBitwiseOperationsViewModel.SelectedElementaryOperation = 1;
        _arithmeticBitwiseOperationsViewModel.SelectedBitwiseOperation = 1;
        _arithmeticBitwiseOperationsViewModel.SelectedArithmeticOperation = 1;
        
        _arithmeticBitwiseOperationsViewModel.Execute.Execute();
    }
    
    [AvaloniaTest]
    public async Task ArithmeticBitwiseOperationsViewModelExecuteSelectImageCommand()
    {
        Mock<IStorageFile> storageFileMock = new Mock<IStorageFile>();
        storageFileMock.Setup(x => x.OpenReadAsync())
            .ReturnsAsync(() =>
            {
                MemoryStream ms = new MemoryStream();
                _testImage.Save(ms, ImageFormat.Png);

                return ms;
            });
        _commonDialogServiceMock.Setup(x => x.ShowFileDialogInNewWindow())
            .ReturnsAsync(storageFileMock.Object);

        _arithmeticBitwiseOperationsViewModel.SelectImage.Execute();

        _commonDialogServiceMock.Verify(x => x.ShowFileDialogInNewWindow(), Times.Once());
    }
}