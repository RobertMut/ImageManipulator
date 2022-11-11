using ImageManipulator.Application.Common.Helpers;
using ImageManipulator.Application.Common.Interfaces;
using ReactiveUI;
using System.Reactive;

namespace ImageManipulator.Application.ViewModels
{
    public class MultiThresholdingViewModel : ImageOperationDialogViewModelBase
    {
        private readonly IImagePointOperationsService imagePointOperationsService;
        private Avalonia.Media.Imaging.Bitmap _beforeImage;
        private Avalonia.Media.Imaging.Bitmap _afterImage;
        private int _enteredLowerThreshold;
        private int _enteredUpperThreshold;
        public override Avalonia.Media.Imaging.Bitmap BeforeImage
        {
            get => _beforeImage; set
            {
                _ = this.RaiseAndSetIfChanged(ref _beforeImage, value);
            }
        }

        public override Avalonia.Media.Imaging.Bitmap AfterImage
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

        public ReactiveCommand<Unit, Unit> TresholdingCommand { get; }

        #endregion Commands

        public MultiThresholdingViewModel(IImagePointOperationsService imagePointOperationsService)
        {
            this.imagePointOperationsService = imagePointOperationsService;
            TresholdingCommand = ReactiveCommand.Create(ExecuteTresholding);
        }

        private void ExecuteTresholding()
        {
            var stretchedImage = imagePointOperationsService.MultiThresholding(ImageConverterHelper.ConvertFromAvaloniaUIBitmap(_beforeImage), _enteredLowerThreshold, _enteredUpperThreshold, ReplaceColours);
            AfterImage = ImageConverterHelper.ConvertFromSystemDrawingBitmap(stretchedImage);
        }
    }
}