using System;
using ImageManipulator.Application.Common.Interfaces;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.Input;
using ImageManipulator.Domain.Common.Helpers;
using Splat;

namespace ImageManipulator.Application.ViewModels
{
    public class MultiThresholdingViewModel : ImageOperationDialogViewModelBase
    {
        private readonly IImagePointOperationsService imagePointOperationsService;
        private Bitmap? _beforeImage;
        private Bitmap? _afterImage;
        private int _enteredLowerThreshold;
        private int _enteredUpperThreshold;
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

        public int EnteredUpperThreshold { get => _enteredUpperThreshold; set => this.RaiseAndSetIfChanged(ref _enteredUpperThreshold, value); }
        public int EnteredLowerThreshold { get => _enteredLowerThreshold; set => this.RaiseAndSetIfChanged(ref _enteredLowerThreshold, value); }

        public bool ReplaceColours { get; set; }

        #region Commands

        public ReactiveCommand<Unit, Unit> ThresholdingCommand { get; }

        #endregion Commands

        public MultiThresholdingViewModel(IImagePointOperationsService imagePointOperationsService)
        {
            this.imagePointOperationsService = imagePointOperationsService;
            ThresholdingCommand = ReactiveCommand.CreateFromObservable(ExecuteThresholding);
            ThresholdingCommand.IsExecuting.ToProperty(this, x => x.IsCommandActive, out isCommandActive);
            ThresholdingCommand.ThrownExceptions.Subscribe(ex =>
                this.Log().ErrorException("Error during thresholding!", ex));
            AcceptCommand = new RelayCommand<Window>(Accept, x => AcceptCommandCanExecute());
            CancelCommand = new RelayCommand<Window>(Cancel);
        }

        private IObservable<Unit> ExecuteThresholding() =>
            Observable.Start(() =>
            {
                var stretchedImage = imagePointOperationsService.MultiThresholding(
                    ImageConverterHelper.ConvertFromAvaloniaUIBitmap(_beforeImage), _enteredLowerThreshold,
                    _enteredUpperThreshold, ReplaceColours);
                AfterImage = ImageConverterHelper.ConvertFromSystemDrawingBitmap(stretchedImage);
            });
    }
}