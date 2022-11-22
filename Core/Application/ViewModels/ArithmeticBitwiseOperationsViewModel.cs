using Avalonia.Media;
using Avalonia.Media.Imaging;
using AvaloniaColorPicker;
using ImageManipulator.Application.Common.Enums;
using ImageManipulator.Application.Common.Helpers;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Common.Enums;
using ReactiveUI;
using System;
using System.Reactive;
using System.Reactive.Linq;

namespace ImageManipulator.Application.ViewModels
{
    public class ArithmeticBitwiseOperationsViewModel : ImageOperationDialogViewModelBase
    {
        private readonly IImageArithmeticService _imageArithmeticService;
        private readonly IImageBitwiseService _imageBitwiseService;
        private readonly ICommonDialogService commonDialogService;
        private Bitmap _afterImage;
        private Bitmap _beforeImage;
        private Bitmap _operationImage;
        private Color _pickedColor;
        private int? _value;
        private bool _isArithmeticSelected;
        private ElementaryOperationParameterEnum _elementaryOperation;
        private ArithmeticOperationType _arithmeticOperation;
        private BitwiseOperationType _bitwiseOperation;

        public override Bitmap AfterImage { get => _afterImage; set => this.RaiseAndSetIfChanged(ref _afterImage, value); }
        public override Bitmap BeforeImage { get => _beforeImage; set => this.RaiseAndSetIfChanged(ref _beforeImage, value); }
        public Bitmap OperationImage { get => _operationImage; set => this.RaiseAndSetIfChanged(ref _operationImage, value); }
        public Color PickedColor { get => _pickedColor; set => this.RaiseAndSetIfChanged(ref _pickedColor, value); }
        public string? Value { get => _value.ToString(); set => this.RaiseAndSetIfChanged(ref _value, Convert.ToInt32(value)); }
        public bool IsArithmeticSelected { get => _isArithmeticSelected; set => this.RaiseAndSetIfChanged(ref _isArithmeticSelected, value); }
        public int SelectedElementaryOperation { get => (int)_elementaryOperation; set => this.RaiseAndSetIfChanged(ref _elementaryOperation, (ElementaryOperationParameterEnum)value); }
        public int SelectedArithmeticOperation { get => (int)_arithmeticOperation; set => this.RaiseAndSetIfChanged(ref _arithmeticOperation, (ArithmeticOperationType)value); }
        public int SelectedBitwiseOperation { get => (int)_bitwiseOperation; set => this.RaiseAndSetIfChanged(ref _bitwiseOperation, (BitwiseOperationType)value); }

        #region Commands

        public ReactiveCommand<Unit, Unit> Execute { get; }
        public ReactiveCommand<Unit, Unit> SelectImage { get; }

        #endregion Commands

        public ArithmeticBitwiseOperationsViewModel(IImageArithmeticService imageArithmeticService, IImageBitwiseService imageBitwiseService, ICommonDialogService commonDialogService)
        {
            this._imageArithmeticService = imageArithmeticService;
            this._imageBitwiseService = imageBitwiseService;
            this.commonDialogService = commonDialogService;
            Execute = ReactiveCommand.Create(ExecuteOperationOnImage);
            SelectImage = ReactiveCommand.Create(SelectImageCommand);
        }

        private void ExecuteOperationOnImage()
        {
            if (_isArithmeticSelected)
            {
                AfterImage = ImageConverterHelper.ConvertFromSystemDrawingBitmap(
                    _imageArithmeticService.Execute(ImageConverterHelper.ConvertFromAvaloniaUIBitmap(BeforeImage),
                        ParameterSelector(_elementaryOperation),
                        _arithmeticOperation)
                    );
            }
            else
            {
                AfterImage = ImageConverterHelper.ConvertFromSystemDrawingBitmap(
                    _imageBitwiseService.Execute(ImageConverterHelper.ConvertFromAvaloniaUIBitmap(BeforeImage),
                        ParameterSelector(_elementaryOperation),
                        _bitwiseOperation)
                    );
            }
        }

        private void SelectImageCommand() => commonDialogService
            .ShowFileDialogInNewWindow()
            .ContinueWith(x =>
            {
                OperationImage = new Bitmap(
                    Observable.FromAsync<string[]>(() => x).FirstOrDefault()[0]
                    );
            });

        private object ParameterSelector(ElementaryOperationParameterEnum parameter) => parameter switch
        {
            ElementaryOperationParameterEnum.Value => _value.Value,
            ElementaryOperationParameterEnum.Color => _pickedColor,
            ElementaryOperationParameterEnum.Image => ImageConverterHelper.ConvertFromAvaloniaUIBitmap(OperationImage),
            _ => throw new Exception("Invalid operaton")
        };
    }
}