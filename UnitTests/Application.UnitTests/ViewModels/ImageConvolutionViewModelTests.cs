using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using Avalonia.Headless.NUnit;
using Avalonia.Platform.Storage;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Application.Common.Models;
using ImageManipulator.Application.ViewModels;
using ImageManipulator.Domain.Common.CQRS.Interfaces;
using ImageManipulator.Presentation;
using Microsoft.Extensions.DependencyInjection;
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