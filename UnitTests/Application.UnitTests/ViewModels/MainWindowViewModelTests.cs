using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using Avalonia.Headless.NUnit;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Application.Common.Models;
using ImageManipulator.Application.ViewModels;
using ImageManipulator.Domain.Common.CQRS.Interfaces;
using Moq;

namespace Application.UnitTests.ViewModels;

[ExcludeFromCodeCoverage]
[TestFixture]
public class MainWindowViewModelTests
{
    private MainWindowViewModel _mainWindowViewModel;
    private Mock<ICommonDialogService> _commonDialogServiceMock;
    private Mock<IServiceProvider> _serviceProviderMock;
    private Mock<IImagePointOperationsService> _imagePointOperationsServiceMock;
    private Mock<ITabService> _tabServiceMock;
    private TabItem _sampleTab;
    private Bitmap _testImage;

    [SetUp]
    public async Task SetUp()
    {
        _commonDialogServiceMock = new Mock<ICommonDialogService>();
        _serviceProviderMock = new Mock<IServiceProvider>();
        _imagePointOperationsServiceMock = new Mock<IImagePointOperationsService>();
        _tabServiceMock = new Mock<ITabService>();
        _testImage = new Bitmap("Resources/image.png");
        _sampleTab = new TabItem("Tab 1",
            new TabControlViewModel(Mock.Of<IQueryDispatcher>(), Mock.Of<ICommandDispatcher>())
            {
                Path = "Path",
                Image = _testImage
            });
        _tabServiceMock.Setup(x => x.GetTabItems())
            .Returns(new ObservableCollection<TabItem> { _sampleTab });
        _tabServiceMock.Setup(x => x.GetTab(It.IsAny<string>()))
            .Returns(_sampleTab);
        _mainWindowViewModel = new MainWindowViewModel(_commonDialogServiceMock.Object, _serviceProviderMock.Object,
            _imagePointOperationsServiceMock.Object, _tabServiceMock.Object);
    }

    [AvaloniaTest]
    public async Task CloseCurrentTabClosesTab()
    {
        _tabServiceMock.Setup(x => x.RemoveTab(It.IsAny<string>()));
        
        _mainWindowViewModel.CloseTab.Execute();
        _tabServiceMock.Verify(x => x.GetTabItems(), Times.Exactly(2));
        _tabServiceMock.Verify(x => x.AddEmpty(It.IsAny<TabItem>()), Times.Never);
    }
    
    [AvaloniaTest]
    public async Task CloseCurrentTabCreatesTabOnClose()
    {
        _tabServiceMock.Setup(x => x.RemoveTab(It.IsAny<string>()));
        _tabServiceMock.Setup(x => x.AddEmpty(It.IsAny<TabItem>()))
            .Returns(_sampleTab);
        _tabServiceMock.Setup(x => x.GetTabItems())
            .Returns(new ObservableCollection<TabItem> {  });
        _serviceProviderMock.Setup(x => x.GetService(It.IsAny<Type>()))
            .Returns(new TabControlViewModel(Mock.Of<IQueryDispatcher>(), Mock.Of<ICommandDispatcher>())
            {
                Path = "test/path"
            });
        
        _mainWindowViewModel.CloseTab.Execute();
        _tabServiceMock.Verify(x => x.GetTabItems(), Times.Exactly(3));
        _tabServiceMock.Verify(x => x.AddEmpty(It.IsAny<TabItem>()), Times.Once);
    }
    
    [AvaloniaTest]
    public async Task SaveImageExecutes()
    {
        _mainWindowViewModel.SaveImageCommand.Execute();
    }
    
    [AvaloniaTest]
    public async Task SaveImageAsExecutes()
    {
        _mainWindowViewModel.SaveImageAsCommand.Execute();
    }
    
    [AvaloniaTest]
    public async Task DuplicateExecutes()
    {
        _tabServiceMock.Setup(x => x.Duplicate(It.IsAny<string>()))
            .Returns(_sampleTab);
        
        _mainWindowViewModel.DuplicateCommand.Execute();
    }
    
    [AvaloniaTest]
    public async Task ContrastStretchingExecutes()
    {
        _commonDialogServiceMock.Setup(x => x.ShowDialog(It.IsAny<ContrastStretchingViewModel>()));
        _serviceProviderMock.Setup(x => x.GetService(It.IsAny<Type>()))
            .Returns(new ContrastStretchingViewModel(Mock.Of<IQueryDispatcher>()));
        
        _mainWindowViewModel.ContrastStretchingCommand.Execute();

        _commonDialogServiceMock.Verify(x => x.ShowDialog(It.IsAny<ContrastStretchingViewModel>()), Times.Once);
        _serviceProviderMock.Verify(x => x.GetService(It.IsAny<Type>()), Times.Once);
    }
    
    [AvaloniaTest]
    public async Task HistogramEqualizationExecutes()
    {
        _commonDialogServiceMock.Setup(x => x.ShowDialog(It.IsAny<HistogramEqualizationViewModel>()));
        _serviceProviderMock.Setup(x => x.GetService(It.IsAny<Type>()))
            .Returns(new HistogramEqualizationViewModel(Mock.Of<IQueryDispatcher>()));
        
        _mainWindowViewModel.HistogramEqualizationCommand.Execute();

        _commonDialogServiceMock.Verify(x => x.ShowDialog(It.IsAny<HistogramEqualizationViewModel>()), Times.Once);
        _serviceProviderMock.Verify(x => x.GetService(It.IsAny<Type>()), Times.Once);
    }
    
    [AvaloniaTest]
    public async Task NonLinearContrastExecutes()
    {
        _commonDialogServiceMock.Setup(x => x.ShowDialog(It.IsAny<NonLinearContrastStretchingViewModel>()));
        _serviceProviderMock.Setup(x => x.GetService(It.IsAny<Type>()))
            .Returns(new NonLinearContrastStretchingViewModel(Mock.Of<IQueryDispatcher>()));
        
        _mainWindowViewModel.GammaCorrectionCommand.Execute();

        _commonDialogServiceMock.Verify(x => x.ShowDialog(It.IsAny<NonLinearContrastStretchingViewModel>()), Times.Once);
        _serviceProviderMock.Verify(x => x.GetService(It.IsAny<Type>()), Times.Once);
    }
    
    [AvaloniaTest]
    public async Task ArithmeticBitwiseExecutes()
    {
        _commonDialogServiceMock.Setup(x => x.ShowDialog(It.IsAny<ArithmeticBitwiseOperationsViewModel>()));
        _serviceProviderMock.Setup(x => x.GetService(It.IsAny<Type>()))
            .Returns(new ArithmeticBitwiseOperationsViewModel(Mock.Of<IQueryDispatcher>(), Mock.Of<ICommonDialogService>()));
        
        _mainWindowViewModel.ArithmeticBitwiseCommand.Execute();

        _commonDialogServiceMock.Verify(x => x.ShowDialog(It.IsAny<ArithmeticBitwiseOperationsViewModel>()), Times.Once);
        _serviceProviderMock.Verify(x => x.GetService(It.IsAny<Type>()), Times.Once);
    }
    
    [AvaloniaTest]
    public async Task ImageConvolutionExecutes()
    {
        _commonDialogServiceMock.Setup(x => x.ShowDialog(It.IsAny<ImageConvolutionViewModel>()));
        _serviceProviderMock.Setup(x => x.GetService(It.IsAny<Type>()))
            .Returns(new ImageConvolutionViewModel(Mock.Of<IQueryDispatcher>()));
        
        _mainWindowViewModel.ImageConvolutionCommand.Execute();

        _commonDialogServiceMock.Verify(x => x.ShowDialog(It.IsAny<ImageConvolutionViewModel>()), Times.Once);
        _serviceProviderMock.Verify(x => x.GetService(It.IsAny<Type>()), Times.Once);
    }
    
    [AvaloniaTest]
    public async Task MultiThresholdExecutes()
    {
        _commonDialogServiceMock.Setup(x => x.ShowDialog(It.IsAny<MultiThresholdViewModel>()));
        _serviceProviderMock.Setup(x => x.GetService(It.IsAny<Type>()))
            .Returns(new MultiThresholdViewModel(Mock.Of<IQueryDispatcher>()));
        
        _mainWindowViewModel.MultiThresholdingCommand.Execute();

        _commonDialogServiceMock.Verify(x => x.ShowDialog(It.IsAny<MultiThresholdViewModel>()), Times.Once);
        _serviceProviderMock.Verify(x => x.GetService(It.IsAny<Type>()), Times.Once);
    }
    
    [AvaloniaTest]
    public async Task ThresholdExecutes()
    {
        _commonDialogServiceMock.Setup(x => x.ShowDialog(It.IsAny<ThresholdViewModel>()));
        _serviceProviderMock.Setup(x => x.GetService(It.IsAny<Type>()))
            .Returns(new ThresholdViewModel(Mock.Of<IQueryDispatcher>()));
        
        _mainWindowViewModel.ThresholdCommand.Execute();

        _commonDialogServiceMock.Verify(x => x.ShowDialog(It.IsAny<ThresholdViewModel>()), Times.Once);
        _serviceProviderMock.Verify(x => x.GetService(It.IsAny<Type>()), Times.Once);
    }
    
    [AvaloniaTest]
    public async Task NegateExecutes()
    {
        _imagePointOperationsServiceMock.Setup(x => x.Negation(It.IsAny<Bitmap?>()))
            .Returns(_testImage);
        
        _mainWindowViewModel.NegationCommand.Execute();

        _imagePointOperationsServiceMock.Verify(x => x.Negation(It.IsAny<Bitmap?>()), Times.Once);
    }
}