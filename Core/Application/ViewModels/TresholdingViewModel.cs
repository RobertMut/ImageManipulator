using System.Drawing;
using System.Reactive;
using ImageManipulator.Application.Common.Helpers;
using ImageManipulator.Application.Common.Interfaces;
using ReactiveUI;

namespace ImageManipulator.Application.ViewModels
{
	public class TresholdingViewModel : ViewModelBase
    {
		private readonly IImagePointOperationsService imagePointOperationsService;
		private Avalonia.Media.Imaging.Bitmap _beforeImage;
        private Avalonia.Media.Imaging.Bitmap _afterImage;
		private int _enteredThreshold;

        public Avalonia.Media.Imaging.Bitmap BeforeImage { get => _beforeImage; set
			{
				_ = this.RaiseAndSetIfChanged(ref _beforeImage, value);
			}
		}

        public Avalonia.Media.Imaging.Bitmap AfterImage
        {
            get => _afterImage; set
            {
                this.RaiseAndSetIfChanged(ref _afterImage, value);
            }
        }
		public int EnteredThreshold { get => _enteredThreshold; set => this.RaiseAndSetIfChanged(ref _enteredThreshold, value); }
        #region Commands 
        public ReactiveCommand<Unit, Unit> TresholdingCommand { get; }

		#endregion

		public TresholdingViewModel(IImagePointOperationsService imagePointOperationsService)
		{
			this.imagePointOperationsService = imagePointOperationsService;
			TresholdingCommand = ReactiveCommand.Create(ExecuteTresholding);
		}

		private void ExecuteTresholding()
		{
			var stretchedImage = imagePointOperationsService.Tresholding(ImageConverterHelper.ConvertFromAvaloniaUIBitmap(_beforeImage), _enteredThreshold);
            AfterImage = ImageConverterHelper.ConvertFromSystemDrawingBitmap(stretchedImage);
		}
    }
}