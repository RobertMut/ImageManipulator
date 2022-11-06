using System.Drawing;
using System.Reactive;
using ImageManipulator.Application.Common.Helpers;
using ImageManipulator.Application.Common.Interfaces;
using ReactiveUI;

namespace ImageManipulator.Application.ViewModels
{
	public class ContrastStretchingViewModel : ImageOperationDialogViewModelBase
    {
		private readonly IImagePointOperationsService imagePointOperationsService;
		private readonly IImageDataService imageDataService;
		private Avalonia.Media.Imaging.Bitmap _beforeImage;
        private Avalonia.Media.Imaging.Bitmap _afterImage;
		private int _enteredLowerThreshold;
		private int _enteredUpperThreshold;

        public override Avalonia.Media.Imaging.Bitmap BeforeImage { get => _beforeImage; set
			{
				CalculateSuggestedThresholds();
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

        public double[] histogramValues { get; set; }
		public int LowerThreshold { get; set; }
		public int UpperThreshold { get; set; }
		public int EnteredLowerThreshold { get => _enteredLowerThreshold; set => this.RaiseAndSetIfChanged(ref _enteredLowerThreshold, value); }
        public int EnteredUpperThreshold { get => _enteredUpperThreshold; set => this.RaiseAndSetIfChanged(ref _enteredUpperThreshold, value); }
        #region Commands 
        public ReactiveCommand<Unit, Unit> ExecuteLinearStretching { get; }

		#endregion

		public ContrastStretchingViewModel(IImagePointOperationsService imagePointOperationsService)
		{
			this.imagePointOperationsService = imagePointOperationsService;
			ExecuteLinearStretching = ReactiveCommand.Create(LinearlyStreatchContrast);
		}

		private void CalculateSuggestedThresholds()
		{
            LowerThreshold = imagePointOperationsService.CalculateLowerImageThresholdPoint(histogramValues);
			UpperThreshold = imagePointOperationsService.CalculateUpperImageThresholdPoint(histogramValues);
        }

		private void LinearlyStreatchContrast()
		{
			var stretchedImage = imagePointOperationsService.StretchContrast(ImageConverterHelper.ConvertFromAvaloniaUIBitmap(_beforeImage), _enteredLowerThreshold, _enteredUpperThreshold);
            AfterImage = ImageConverterHelper.ConvertFromSystemDrawingBitmap(stretchedImage);
		}
    }
}