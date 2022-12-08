using Avalonia.Media.Imaging;
using ImageManipulator.Application.Common.Helpers;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Domain.Common.Enums;
using ImageManipulator.Domain.Common.Statics;
using ReactiveUI;
using System.Reactive;

namespace ImageManipulator.Application.ViewModels
{
    public class ImageConvolutionViewModel : ImageOperationDialogViewModelBase
    {
        private readonly IImageConvolutionService imageConvolutionService;
        private Bitmap _afterImage;
        private Bitmap _beforeImage;
        private double _value;
        private bool _isSobelSelected = false;
        private SoftenSharpenEnum _selectedSoftenSharpen;
        private SobelEnum _selectedSobel;

        public override Bitmap AfterImage { get => _afterImage; set => this.RaiseAndSetIfChanged(ref _afterImage, value); }
        public override Bitmap BeforeImage { get => _beforeImage; set => this.RaiseAndSetIfChanged(ref _beforeImage, value); }
        public double Value { get => _value; set => this.RaiseAndSetIfChanged(ref _value, value); }
        public int SelectedSoftenSharpen { get => (int)_selectedSoftenSharpen; set => this.RaiseAndSetIfChanged(ref _selectedSoftenSharpen, (SoftenSharpenEnum)value); }
        public int SelectedSobel { get => (int)_selectedSobel; set => this.RaiseAndSetIfChanged(ref _selectedSobel, (SobelEnum)value); }
        public bool IsSobelSelected { get => _isSobelSelected; set => this.RaiseAndSetIfChanged(ref _isSobelSelected, value); }

        #region Commands

        public ReactiveCommand<Unit, Unit> Execute { get; }
        public ReactiveCommand<Unit, Unit> SelectImage { get; }

        #endregion Commands

        public ImageConvolutionViewModel(IImageConvolutionService imageConvolutionService)
        {
            this.imageConvolutionService = imageConvolutionService;
            Execute = ReactiveCommand.Create(ExecuteOperationOnImage);
        }

        private void ExecuteOperationOnImage()
        {
            var image = ImageConverterHelper.ConvertFromAvaloniaUIBitmap(BeforeImage);

            if ((int)_selectedSoftenSharpen < 3)
            {
                AfterImage = ImageConverterHelper.ConvertFromSystemDrawingBitmap(imageConvolutionService.Execute(image, MatrixSelector(IsSobelSelected), Value, true));
            }

            AfterImage = ImageConverterHelper.ConvertFromSystemDrawingBitmap(
                imageConvolutionService.Execute(image,
                        MatrixSelector(IsSobelSelected),
                        Value));
        }

        private double[,] MatrixSelector(bool sobelSelected) => sobelSelected switch
        {
            true => ConvolutionMatrices.SobelMatrices[_selectedSobel],
            false => _selectedSoftenSharpen != SoftenSharpenEnum.SoftenAverage3x3WithWeight ? ConvolutionMatrices.SoftenSharpenMatrices[_selectedSoftenSharpen] : ConvolutionMatrices.SoftenAverage3x3WithWeight(_value)
        };
    }
}