using System.Reactive;
using ImageManipulator.Application.Common.Helpers;
using ImageManipulator.Application.Common.Interfaces;
using ReactiveUI;

namespace ImageManipulator.Application.ViewModels
{
    public class NonLinearContrastStretchingViewModel : ImageOperationDialogViewModelBase
    {
		private readonly IImagePointOperationsService imagePointOperationsService;
		private Avalonia.Media.Imaging.Bitmap _beforeImage;
        private Avalonia.Media.Imaging.Bitmap _afterImage;
		private double _gamma;

        public override Avalonia.Media.Imaging.Bitmap BeforeImage { get => _beforeImage; set
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

		public double GammaValue { get => _gamma; set => this.RaiseAndSetIfChanged(ref _gamma, value); }

        #region Commands 
        public ReactiveCommand<Unit, Unit> ExecuteNonLinearStretching { get; }

		#endregion

		public NonLinearContrastStretchingViewModel(IImagePointOperationsService imagePointOperationsService)
		{
			this.imagePointOperationsService = imagePointOperationsService;
			ExecuteNonLinearStretching = ReactiveCommand.Create(NonLinearlyStretchContrast);
		}


		private void NonLinearlyStretchContrast()
		{
			var stretchedImage = imagePointOperationsService.NonLinearlyStretchContrast(ImageConverterHelper.ConvertFromAvaloniaUIBitmap(_beforeImage), _gamma);
            AfterImage = ImageConverterHelper.ConvertFromSystemDrawingBitmap(stretchedImage);
		}
    }
}