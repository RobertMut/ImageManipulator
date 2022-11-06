using System.Drawing;
using System.Reactive;
using ImageManipulator.Application.Common.Helpers;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Application.Common.Services;
using ReactiveUI;

namespace ImageManipulator.Application.ViewModels
{
	public class HistogramEqualizationViewModel : ImageOperationDialogViewModelBase
    {
		private readonly IImagePointOperationsService imagePointOperationsService;
		private Avalonia.Media.Imaging.Bitmap _beforeImage;
        private Avalonia.Media.Imaging.Bitmap _afterImage;
		private double _gamma;
		public double[][] lut;

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
        public ReactiveCommand<Unit, Unit> ExecuteEqualizeHistogram { get; }

		#endregion

		public HistogramEqualizationViewModel(IImagePointOperationsService imagePointOperationsService)
		{
			this.imagePointOperationsService = imagePointOperationsService;
            ExecuteEqualizeHistogram = ReactiveCommand.Create(Equalization);
		}


		private void Equalization()
		{
			var stretchedImage = imagePointOperationsService.HistogramEqualization(ImageConverterHelper.ConvertFromAvaloniaUIBitmap(BeforeImage), lut);
            AfterImage = ImageConverterHelper.ConvertFromSystemDrawingBitmap(stretchedImage);
		}
    }
}