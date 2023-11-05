using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using Avalonia.Headless.NUnit;
using ImageManipulator.Application.ViewModels;
using ImageManipulator.Domain.Common.CQRS.Interfaces;
using Moq;

namespace Application.UnitTests.ViewModels;

[ExcludeFromCodeCoverage]
[TestFixture]
public class ImageConvolutionViewModelTests
{
    private ImageConvolutionViewModel _imageConvolutionViewModel;
    private Bitmap _testImage;

    [SetUp]
    public async Task SetUp()
    {
        _testImage = new Bitmap("Resources/image.png");
        _imageConvolutionViewModel = new ImageConvolutionViewModel(Mock.Of<IQueryDispatcher>());
    }

    [AvaloniaTest]
    public async Task ImageConvolutionViewModelExecute()
    {
        _imageConvolutionViewModel.SelectedSoftenSharpen = 1;
        _imageConvolutionViewModel.SelectedEdgeDetection = 1;
        _imageConvolutionViewModel.BeforeImage = _testImage;
        _imageConvolutionViewModel.BorderConstVal = 1;
        _imageConvolutionViewModel.IsWeightedSelected = true;
        _imageConvolutionViewModel.IsSobelSelected = true;
        _imageConvolutionViewModel.IsEdgeDetectionSelected = true;
        
        _imageConvolutionViewModel.Execute.Execute();
    }
}