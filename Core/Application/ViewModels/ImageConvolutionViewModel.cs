using Avalonia.Media.Imaging;
using ImageManipulator.Application.Common.Helpers;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Common.Enums;
using ImageManipulator.Common.Interfaces;
using ImageManipulator.Common.Matrices;
using ReactiveUI;
using System;
using System.Reactive;
using System.Reactive.Linq;

namespace ImageManipulator.Application.ViewModels
{
    public class ImageConvolutionViewModel : ImageOperationDialogViewModelBase
    {
        private readonly IImageConvolutionService imageConvolutionService;
        private readonly IImageBorderService imageBorderService;
        private Bitmap _afterImage;
        private Bitmap _beforeImage;
        private double _value;
        private bool _isSobelSelected = false;
        private SoftenSharpenEnum _selectedSoftenSharpen;
        private SobelEnum _selectedSobel;
        private ImageWrapEnum _imageWrap;
        private bool _isWeightedSelected;
        private bool _is3x3Selected;
        private bool _is5x5Selected;
        private bool _is7x7Selected;
        private bool _is9x9Selected;
        private double _borderConstVal;

        public override Bitmap AfterImage { get => _afterImage; set => this.RaiseAndSetIfChanged(ref _afterImage, value); }
        public override Bitmap BeforeImage { get => _beforeImage; set => this.RaiseAndSetIfChanged(ref _beforeImage, value); }
        public double Value { get => _value; set => this.RaiseAndSetIfChanged(ref _value, value); }
        public bool IsWeightedSelected { get => _isWeightedSelected; set => this.RaiseAndSetIfChanged(ref _isWeightedSelected, value); }
        public int SelectedSoftenSharpen3x3 { get => (int)_selectedSoftenSharpen; set => ChangeAndSetWeightedBool(ref _selectedSoftenSharpen, (SoftenSharpenEnum)value); }
        public int ImageWrap { get => (int)_imageWrap; set => this.RaiseAndSetIfChanged(ref _imageWrap, (ImageWrapEnum)value); }
        public int SelectedSobel3x3 { get => (int)_selectedSobel; set => this.RaiseAndSetIfChanged(ref _selectedSobel, (SobelEnum)value); }
        public bool Is3x3Selected { get => _is3x3Selected; set => this.RaiseAndSetIfChanged(ref _is3x3Selected, value); }
        public bool Is5x5Selected { get => _is5x5Selected; set => this.RaiseAndSetIfChanged(ref _is5x5Selected, value); }
        public bool Is7x7Selected { get => _is7x7Selected; set => this.RaiseAndSetIfChanged(ref _is7x7Selected, value); }
        public bool Is9x9Selected { get => _is9x9Selected; set => this.RaiseAndSetIfChanged(ref _is9x9Selected, value); }
        public double BorderConstVal { get => _borderConstVal; set => this.RaiseAndSetIfChanged(ref _borderConstVal, value); }

        public bool IsSobelSelected { get => _isSobelSelected; set => this.RaiseAndSetIfChanged(ref _isSobelSelected, value); }

        #region Commands

        public ReactiveCommand<Unit, Unit> Execute { get; }
        public ReactiveCommand<Unit, Unit> SelectImage { get; }
        #endregion Commands

        public ImageConvolutionViewModel(IImageConvolutionService imageConvolutionService, IImageBorderService imageBorderService)
        {
            this.imageConvolutionService = imageConvolutionService;
            this.imageBorderService = imageBorderService;
            Execute = ReactiveCommand.Create(ExecuteOperationOnImage);
        }

        private void ExecuteOperationOnImage()
        {
            AfterImage = Observable.Return<System.Drawing.Bitmap>(ImageConverterHelper.ConvertFromAvaloniaUIBitmap(_beforeImage))
                .Select(x => _imageWrap > 0 && (int)_imageWrap < 4 ? imageBorderService.Execute(x, _imageWrap, 5, 5, 5, 5, System.Drawing.Color.FromArgb((int)_borderConstVal, (int)_borderConstVal, (int)_borderConstVal)) : x)
                .Select(bitmap =>
                {
                    IConvolutionMatrix matrices = _is3x3Selected ? new ConvolutionMatrices3x3() :
                    (_is5x5Selected ? new ConvolutionMatrices5x5() :
                        (_is7x7Selected ? new ConvolutionMatrices7x7() :
                            (_is9x9Selected ? new ConvolutionMatrices9x9() : throw new Exception("Operation not found!"))
                        ));

                    var matrix = MatrixSelector(_isSobelSelected, matrices);

                    if ((int)_selectedSoftenSharpen < 3 && !_isSobelSelected)
                    {
                        return imageConvolutionService.Execute(bitmap, matrix, _value, true);
                    }
                    else
                    {
                        return imageConvolutionService.Execute(bitmap, matrix, _value);
                    }
                })
                .Select(bitmap => BorderAfter(bitmap))
                .Select(x => ImageConverterHelper.ConvertFromSystemDrawingBitmap(x)).FirstOrDefault();
        }

        private double[,] MatrixSelector(bool sobelSelected, IConvolutionMatrix matrix) => sobelSelected switch
        {
            true => matrix.SobelMatrices[_selectedSobel],
            false => _selectedSoftenSharpen != SoftenSharpenEnum.SoftenAverageWithWeight ? matrix.SoftenSharpenMatrices[_selectedSoftenSharpen] : matrix.SoftenAverageWithWeight(_value)
        };

        private T ChangeAndSetWeightedBool<T>(ref T existing, T value)
        {
            if (Convert.ToInt32(value) == 2) IsWeightedSelected = true;
            else IsWeightedSelected = false;
            return this.RaiseAndSetIfChanged(ref existing, value);
        }

        private System.Drawing.Bitmap BorderAfter(System.Drawing.Bitmap bitmap)
        {
            return _imageWrap == ImageWrapEnum.BORDER_AFTER ? imageBorderService.Execute(bitmap, ImageWrapEnum.BORDER_CONSTANT, 5, 5, 5, 5, System.Drawing.Color.FromArgb((int)_borderConstVal, (int)_borderConstVal, (int)_borderConstVal)) : bitmap;
        }
    }
}