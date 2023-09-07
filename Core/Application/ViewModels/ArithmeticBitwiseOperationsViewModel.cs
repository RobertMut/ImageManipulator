using Avalonia.Media;
using Avalonia.Media.Imaging;
using ImageManipulator.Application.Common.Enums;
using ImageManipulator.Application.Common.Helpers;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Common.Enums;
using ReactiveUI;
using System;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Input;
using Splat;

namespace ImageManipulator.Application.ViewModels;

public class ArithmeticBitwiseOperationsViewModel : ImageOperationDialogViewModelBase
{
    private readonly IImageArithmeticService _imageArithmeticService;
    private readonly IImageBitwiseService _imageBitwiseService;
    private readonly ICommonDialogService commonDialogService;
    private Bitmap? _afterImage;
    private Bitmap? _beforeImage;
    private Bitmap? _operationImage;
    private Color _pickedColor;
    private int? _value;
    private bool _isArithmeticSelected;
    private ElementaryOperationParameterEnum _elementaryOperation;
    private ArithmeticOperationType _arithmeticOperation;
    private BitwiseOperationType _bitwiseOperation;

    private ObservableAsPropertyHelper<bool> _isSelecting;
    
    public override Bitmap? AfterImage { get => _afterImage; set => this.RaiseAndSetIfChanged(ref _afterImage, value); }
    public override Bitmap? BeforeImage { get => _beforeImage; set => this.RaiseAndSetIfChanged(ref _beforeImage, value); }
    public Bitmap? OperationImage { get => _operationImage; set => this.RaiseAndSetIfChanged(ref _operationImage, value); }
    public Color PickedColor { get => _pickedColor; set => this.RaiseAndSetIfChanged(ref _pickedColor, value); }
    public int? Value { get => _value; set => this.RaiseAndSetIfChanged(ref _value, value); }
    public bool IsArithmeticSelected { get => _isArithmeticSelected; set => this.RaiseAndSetIfChanged(ref _isArithmeticSelected, value); }
    public int SelectedElementaryOperation { get => (int)_elementaryOperation; set => this.RaiseAndSetIfChanged(ref _elementaryOperation, (ElementaryOperationParameterEnum)value); }
    public int SelectedArithmeticOperation { get => (int)_arithmeticOperation; set => this.RaiseAndSetIfChanged(ref _arithmeticOperation, (ArithmeticOperationType)value); }
    public int SelectedBitwiseOperation { get => (int)_bitwiseOperation; set => this.RaiseAndSetIfChanged(ref _bitwiseOperation, (BitwiseOperationType)value); }

    #region Commands

    public ReactiveCommand<Unit, Unit> Execute { get; }
    public ReactiveCommand<Unit, Unit> SelectImage { get; }
    public bool IsSelecting
    {
        get { return _isSelecting.Value; }
    }

    #endregion Commands

    public ArithmeticBitwiseOperationsViewModel(IImageArithmeticService imageArithmeticService, IImageBitwiseService imageBitwiseService, ICommonDialogService commonDialogService)
    {
        this._imageArithmeticService = imageArithmeticService;
        this._imageBitwiseService = imageBitwiseService;
        this.commonDialogService = commonDialogService;
        SelectImage = ReactiveCommand.CreateFromObservable(SelectImageCommand);
        SelectImage.IsExecuting.ToProperty(this, x => x.IsSelecting, out _isSelecting);
        SelectImage.ThrownExceptions.Subscribe(Console.WriteLine);
        
        Execute = ReactiveCommand.CreateFromObservable(ExecuteOperationOnImage);
        Execute.IsExecuting.ToProperty(this, x => x.IsCommandActive, out isCommandActive);
        Execute.ThrownExceptions.Subscribe(ex =>
            this.Log().ErrorException("Error during thresholding!", ex));
        AcceptCommand = new RelayCommand<Window>(this.Accept, x => AcceptCommandCanExecute());
        CancelCommand = new RelayCommand<Window>(this.Cancel);
    }

    private IObservable<Unit> ExecuteOperationOnImage() =>
        Observable.Start(() =>
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
        });

    private IObservable<Unit> SelectImageCommand() => Observable.StartAsync(async () =>
    {
        var storageFile = await commonDialogService.ShowFileDialogInNewWindow();
        await using Stream fileStream = await storageFile.OpenReadAsync();
        OperationImage = new Bitmap(fileStream);
    });

    private object? ParameterSelector(ElementaryOperationParameterEnum parameter) => parameter switch
    {
        ElementaryOperationParameterEnum.Value => _value.Value,
        ElementaryOperationParameterEnum.Color => _pickedColor,
        ElementaryOperationParameterEnum.Image => ImageConverterHelper.ConvertFromAvaloniaUIBitmap(OperationImage),
        _ => throw new Exception("Invalid operaton")
    };
}