using Avalonia.Media.Imaging;
using DynamicData.Binding;
using ImageManipulator.Application.Common.Helpers;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Domain.Common.Enums;
using ImageManipulator.Domain.Common.Statics;
using ReactiveUI;
using System;
using System.Reactive;

namespace ImageManipulator.Application.ViewModels
{
    //TODO: REFACTOR!
    public class ImageConvolutionViewModel : ImageOperationDialogViewModelBase
    {
        private readonly IImageConvolutionService imageConvolutionService;
        private readonly IImageBorderService imageBorderService;
        private Bitmap _afterImage;
        private Bitmap _beforeImage;
        private double _value;
        private bool _isSobelSelected = false;
        private SoftenSharpen3x3Enum _selectedSoftenSharpen3x3;
        private SoftenSharpen5x5Enum _selectedSoftenSharpen5x5;
        private SoftenSharpen7x7Enum _selectedSoftenSharpen7x7;
        private SoftenSharpen9x9Enum _selectedSoftenSharpen9x9;
        private SobelEnum _selectedSobel3x3;
        private Sobel5x5Enum _selectedSobel5x5;
        private Sobel7x7Enum _selectedSobel7x7;
        private Sobel9x9Enum _selectedSobel9x9;
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
        public int SelectedSoftenSharpen3x3 { get => (int)_selectedSoftenSharpen3x3; set => ChangeAndSetWeightedBool(ref _selectedSoftenSharpen3x3, (SoftenSharpen3x3Enum)value); }
        public int SelectedSoftenSharpen5x5 { get => (int)_selectedSoftenSharpen5x5; set => ChangeAndSetWeightedBool(ref _selectedSoftenSharpen5x5, (SoftenSharpen5x5Enum)value); }
        public int SelectedSoftenSharpen7x7 { get => (int)_selectedSoftenSharpen7x7; set => ChangeAndSetWeightedBool(ref _selectedSoftenSharpen7x7, (SoftenSharpen7x7Enum)value); }
        public int SelectedSoftenSharpen9x9 { get => (int)_selectedSoftenSharpen9x9; set => ChangeAndSetWeightedBool(ref _selectedSoftenSharpen9x9, (SoftenSharpen9x9Enum)value); }
        public int ImageWrap { get => (int)_imageWrap; set => this.RaiseAndSetIfChanged(ref _imageWrap, (ImageWrapEnum)value); }
        public int SelectedSobel3x3 { get => (int)_selectedSobel3x3; set => this.RaiseAndSetIfChanged(ref _selectedSobel3x3, (SobelEnum)value); }
        public int SelectedSobel5x5 { get => (int)_selectedSobel5x5; set => this.RaiseAndSetIfChanged(ref _selectedSobel5x5, (Sobel5x5Enum)value); }
        public int SelectedSobel7x7 { get => (int)_selectedSobel7x7; set => this.RaiseAndSetIfChanged(ref _selectedSobel7x7, (Sobel7x7Enum)value); }
        public int SelectedSobel9x9 { get => (int)_selectedSobel9x9; set => this.RaiseAndSetIfChanged(ref _selectedSobel9x9, (Sobel9x9Enum)value); }
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

        //TODO: REFACTOR, unacceptable
        private void ExecuteOperationOnImage()
        {
            var image = ImageConverterHelper.ConvertFromAvaloniaUIBitmap(_beforeImage);

            if (_imageWrap > 0 && (int)_imageWrap < 4)
            {
                image = imageBorderService.Execute(image, _imageWrap, 5, 5, 5, 5, System.Drawing.Color.FromArgb((int)_borderConstVal, (int)_borderConstVal, (int)_borderConstVal));
            }

            if (_is3x3Selected)
            {
                if ((int)_selectedSoftenSharpen3x3 < 3 && !_isSobelSelected)
                {
                    var selector = MatrixSelector3x3(_isSobelSelected);
                    AfterImage = ImageConverterHelper.ConvertFromSystemDrawingBitmap(
                        BorderAfter(
                            imageConvolutionService.Execute(image, selector, _value, true)
                            )
                        );
                } else
                {
                    AfterImage = ImageConverterHelper.ConvertFromSystemDrawingBitmap(
                        BorderAfter(
                            imageConvolutionService.Execute(image,
                                    MatrixSelector3x3(_isSobelSelected),
                                    _value)
                            )
                        );
                }
            }
            else if (_is5x5Selected)
            {
                if ((int)_selectedSoftenSharpen5x5 < 3 && !_isSobelSelected)
                {
                    AfterImage = ImageConverterHelper.ConvertFromSystemDrawingBitmap(
                        BorderAfter(
                            imageConvolutionService.Execute(image, MatrixSelector5x5(_isSobelSelected), _value, true)
                            )
                        );
                } else
                {
                    AfterImage = ImageConverterHelper.ConvertFromSystemDrawingBitmap(
                        BorderAfter(
                            imageConvolutionService.Execute(image,
                                    MatrixSelector5x5(_isSobelSelected),
                                    _value)
                            )
                        );
                }
            }
            else if (_is7x7Selected)
            {
                if ((int)_selectedSoftenSharpen7x7 < 3 && !_isSobelSelected)
                {
                    AfterImage = ImageConverterHelper.ConvertFromSystemDrawingBitmap(
                        BorderAfter(
                            imageConvolutionService.Execute(image, MatrixSelector7x7(_isSobelSelected), _value, true)
                            )
                        );
                }
                else
                {
                    AfterImage = ImageConverterHelper.ConvertFromSystemDrawingBitmap(
                        BorderAfter(
                            imageConvolutionService.Execute(image,
                                    MatrixSelector7x7(_isSobelSelected),
                                    _value)
                            )
                        );
                }
            }
            else if (_is9x9Selected)
            {
                if ((int)_selectedSoftenSharpen9x9 < 3 && !_isSobelSelected)
                {
                    AfterImage = ImageConverterHelper.ConvertFromSystemDrawingBitmap(
                        BorderAfter(
                            imageConvolutionService.Execute(image, MatrixSelector9x9(_isSobelSelected), _value, true)
                            )
                        );
                } else
                {
                    AfterImage = ImageConverterHelper.ConvertFromSystemDrawingBitmap(
                        BorderAfter(
                        imageConvolutionService.Execute(image,
                                MatrixSelector9x9(_isSobelSelected),
                                _value)
                            )
                        );
                }
            }
        }

        private double[,] MatrixSelector3x3(bool sobelSelected) => sobelSelected switch
        {
            true => ConvolutionMatrices3x3.SobelMatrices[_selectedSobel3x3],
            false => _selectedSoftenSharpen3x3 != SoftenSharpen3x3Enum.SoftenAverage3x3WithWeight ? ConvolutionMatrices3x3.SoftenSharpenMatrices[_selectedSoftenSharpen3x3] : ConvolutionMatrices3x3.SoftenAverage3x3WithWeight(_value)
        };
        private double[,] MatrixSelector5x5(bool sobelSelected) => sobelSelected switch
        {
            true => ConvolutionMatrices5x5.SobelMatrices[_selectedSobel5x5],
            false => _selectedSoftenSharpen5x5 != SoftenSharpen5x5Enum.SoftenAverage5x5WithWeight ? ConvolutionMatrices5x5.SoftenSharpenMatrices[_selectedSoftenSharpen5x5] : ConvolutionMatrices5x5.SoftenAverage5x5WithWeight(_value)
        };
        private double[,] MatrixSelector7x7(bool sobelSelected) => sobelSelected switch
        {
            true => ConvolutionMatrices7x7.SobelMatrices[_selectedSobel7x7],
            false => _selectedSoftenSharpen7x7 != SoftenSharpen7x7Enum.SoftenAverage7x7WithWeight ? ConvolutionMatrices7x7.SoftenSharpenMatrices[_selectedSoftenSharpen7x7] : ConvolutionMatrices7x7.SoftenAverage7x7WithWeight(_value)
        };

        private double[,] MatrixSelector9x9(bool sobelSelected) => sobelSelected switch
        {
            true => ConvolutionMatrices9x9.SobelMatrices[_selectedSobel9x9],
            false => _selectedSoftenSharpen9x9 != SoftenSharpen9x9Enum.SoftenAverage9x9WithWeight ? ConvolutionMatrices9x9.SoftenSharpenMatrices[_selectedSoftenSharpen9x9] : ConvolutionMatrices9x9.SoftenAverage9x9WithWeight(_value)
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