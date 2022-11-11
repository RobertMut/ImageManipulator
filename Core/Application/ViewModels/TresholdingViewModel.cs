using ImageManipulator.Application.Common.Helpers;
using ImageManipulator.Application.Common.Interfaces;
using ReactiveUI;
using System.Reactive;

namespace ImageManipulator.Application.ViewModels
{
    public class ThresholdingViewModel : ImageOperationDialogViewModelBase
    {
        private readonly IImagePointOperationsService imagePointOperationsService;
        private Avalonia.Media.Imaging.Bitmap _beforeImage;
        private Avalonia.Media.Imaging.Bitmap _afterImage;
        private int _enteredThreshold;
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

        public int EnteredThreshold { get => _enteredThreshold; set => this.RaiseAndSetIfChanged(ref _enteredThreshold, value); }
        public bool ReplaceColours { get; set; }

        #region Commands

        public ReactiveCommand<Unit, Unit> TresholdingCommand { get; }

        #endregion Commands

        public ThresholdingViewModel(IImagePointOperationsService imagePointOperationsService)
        {
            this.imagePointOperationsService = imagePointOperationsService;
            TresholdingCommand = ReactiveCommand.Create(ExecuteTresholding);
        }

        private void ExecuteTresholding()
        {
            var stretchedImage = imagePointOperationsService.Thresholding(ImageConverterHelper.ConvertFromAvaloniaUIBitmap(_beforeImage), _enteredThreshold, ReplaceColours);
            AfterImage = ImageConverterHelper.ConvertFromSystemDrawingBitmap(stretchedImage);
        }
    }
}