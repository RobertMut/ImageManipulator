using System;
using ImageManipulator.Application.Common.Helpers;
using ImageManipulator.Application.Common.Interfaces;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.Input;
using Splat;

namespace ImageManipulator.Application.ViewModels;

public class ThresholdingViewModel : ImageOperationDialogViewModelBase
{
    private readonly IImagePointOperationsService imagePointOperationsService;
    private Bitmap? _beforeImage;
    private Bitmap? _afterImage;
    private int _enteredThreshold;
    public override Bitmap? BeforeImage
    {
        get => _beforeImage; set
        {
            _ = this.RaiseAndSetIfChanged(ref _beforeImage, value);
        }
    }

    public override Bitmap? AfterImage
    {
        get => _afterImage; set
        {
            this.RaiseAndSetIfChanged(ref _afterImage, value);
        }
    }

    public int EnteredThreshold { get => _enteredThreshold; set => this.RaiseAndSetIfChanged(ref _enteredThreshold, value); }
    public bool ReplaceColours { get; set; }

    #region Commands

    public ReactiveCommand<Unit, Unit> TresholdingCommand { get; }

    #endregion Commands

    public ThresholdingViewModel(IImagePointOperationsService imagePointOperationsService)
    {
        this.imagePointOperationsService = imagePointOperationsService;
        TresholdingCommand = ReactiveCommand.CreateFromObservable(ExecuteTresholding);
        TresholdingCommand.IsExecuting.ToProperty(this, x => x.IsCommandActive, out isCommandActive);
        TresholdingCommand.ThrownExceptions.Subscribe(ex =>
            this.Log().ErrorException("Error during stretching!", ex));
        AcceptCommand = new RelayCommand<Window>(this.Accept, x => AcceptCommandCanExecute());
        CancelCommand = new RelayCommand<Window>(this.Cancel);
    }

    private IObservable<Unit> ExecuteTresholding() =>
        Observable.Start(() =>
        {
            var stretchedImage = imagePointOperationsService.Thresholding(
                ImageConverterHelper.ConvertFromAvaloniaUIBitmap(_beforeImage), _enteredThreshold, ReplaceColours);
            AfterImage = ImageConverterHelper.ConvertFromSystemDrawingBitmap(stretchedImage);
        });
}