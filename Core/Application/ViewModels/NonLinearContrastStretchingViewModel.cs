using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.Input;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Domain.Common.Helpers;
using ReactiveUI;

namespace ImageManipulator.Application.ViewModels;

public class NonLinearContrastStretchingViewModel : ImageOperationDialogViewModelBase
{
    private readonly IImagePointOperationsService _imagePointOperationsService;
    private Bitmap? _beforeImage;
    private Bitmap? _afterImage;
    private double _gamma;

    public override Bitmap? BeforeImage
    {
        get => _beforeImage;
        set => _ = this.RaiseAndSetIfChanged(ref _beforeImage, value);
    }

    public override Bitmap? AfterImage
    {
        get => _afterImage;
        set => this.RaiseAndSetIfChanged(ref _afterImage, value);
    }

    public double GammaValue
    {
        get => _gamma;
        set => this.RaiseAndSetIfChanged(ref _gamma, value);
    }

    #region Commands

    public ReactiveCommand<Unit, Unit> ExecuteNonLinearStretching { get; }

    #endregion

    public NonLinearContrastStretchingViewModel(IImagePointOperationsService imagePointOperationsService)
    {
        _imagePointOperationsService = imagePointOperationsService;
        ExecuteNonLinearStretching =
            ReactiveCommand.CreateFromObservable(() => Observable.StartAsync(NonLinearlyStretchContrast));
        ExecuteNonLinearStretching.IsExecuting.ToProperty(this, x => x.IsCommandActive, out isCommandActive);

        AcceptCommand = new RelayCommand<Window>(Accept, x => AcceptCommandCanExecute());
        CancelCommand = new RelayCommand<Window>(Cancel);
    }

    private async Task NonLinearlyStretchContrast()
    {
        var stretchedImage =
            _imagePointOperationsService.NonLinearlyStretchContrast(
                ImageConverterHelper.ConvertFromAvaloniaUIBitmap(_beforeImage), _gamma);
        AfterImage = ImageConverterHelper.ConvertFromSystemDrawingBitmap(stretchedImage);
    }
}