using Avalonia.Media.Imaging;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Common.Enums;
using ImageManipulator.Common.Interfaces;
using ImageManipulator.Common.Matrices;
using ReactiveUI;
using System;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using ImageManipulator.Domain.Common.Dictionaries;
using ImageManipulator.Domain.Common.Helpers;
using Splat;

namespace ImageManipulator.Application.ViewModels;

public class ImageConvolutionViewModel : ImageOperationDialogViewModelBase
{
    private readonly IImageConvolutionService imageConvolutionService;
    private readonly IImageBorderService imageBorderService;
    private readonly IImagePointOperationsService _imagePointOperationsService;
    private Bitmap? _afterImage;
    private Bitmap? _beforeImage;
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
    private bool _isEdgeDetectionSelected = false;
    private double _borderConstVal;
    private EdgeDetectionEnum _selectedEdgeDetection;

    public override Bitmap? AfterImage
    {
        get => _afterImage;
        set => this.RaiseAndSetIfChanged(ref _afterImage, value);
    }

    public override Bitmap? BeforeImage
    {
        get => _beforeImage;
        set => this.RaiseAndSetIfChanged(ref _beforeImage, value);
    }

    public double Value
    {
        get => _value;
        set => this.RaiseAndSetIfChanged(ref _value, value);
    }

    public bool IsWeightedSelected
    {
        get => _isWeightedSelected;
        set => this.RaiseAndSetIfChanged(ref _isWeightedSelected, value);
    }

    public int SelectedEdgeDetection
    {
        get => (int)_selectedEdgeDetection;
        set => ChangeAndSetWeightedBool(ref _selectedEdgeDetection, (EdgeDetectionEnum)value);
    }

    public int SelectedSoftenSharpen3x3
    {
        get => (int)_selectedSoftenSharpen;
        set => ChangeAndSetWeightedBool(ref _selectedSoftenSharpen, (SoftenSharpenEnum)value);
    }

    public int ImageWrap
    {
        get => (int)_imageWrap;
        set => this.RaiseAndSetIfChanged(ref _imageWrap, (ImageWrapEnum)value);
    }

    public int SelectedSobel3x3
    {
        get => (int)_selectedSobel;
        set => this.RaiseAndSetIfChanged(ref _selectedSobel, (SobelEnum)value);
    }

    public bool Is3x3Selected
    {
        get => _is3x3Selected;
        set => this.RaiseAndSetIfChanged(ref _is3x3Selected, value);
    }

    public bool Is5x5Selected
    {
        get => _is5x5Selected;
        set => this.RaiseAndSetIfChanged(ref _is5x5Selected, value);
    }

    public bool Is7x7Selected
    {
        get => _is7x7Selected;
        set => this.RaiseAndSetIfChanged(ref _is7x7Selected, value);
    }

    public bool Is9x9Selected
    {
        get => _is9x9Selected;
        set => this.RaiseAndSetIfChanged(ref _is9x9Selected, value);
    }

    public double BorderConstVal
    {
        get => _borderConstVal;
        set => this.RaiseAndSetIfChanged(ref _borderConstVal, value);
    }

    public bool IsSobelSelected
    {
        get => _isSobelSelected;
        set => this.RaiseAndSetIfChanged(ref _isSobelSelected, value);
    }

    public bool IsEdgeDetectionSelected
    {
        get => _isEdgeDetectionSelected;
        set => this.RaiseAndSetIfChanged(ref _isEdgeDetectionSelected, value);
    }

    #region Commands

    public RelayCommand<Window> CancelCommand { get; private set; }
    public RelayCommand<Window> AcceptCommand { get; private set; }
    public ReactiveCommand<Unit, Unit> Execute { get; }
    public ReactiveCommand<Unit, Unit> SelectImage { get; }

    #endregion Commands

    public ImageConvolutionViewModel(IImageConvolutionService imageConvolutionService,
        IImageBorderService imageBorderService, IImagePointOperationsService imagePointOperationsService)
    {
        this.imageConvolutionService = imageConvolutionService;
        this.imageBorderService = imageBorderService;
        _imagePointOperationsService = imagePointOperationsService;
        Execute = ReactiveCommand.CreateFromObservable(ExecuteOperationOnImage);
        Execute.IsExecuting.ToProperty(this, x => x.IsCommandActive, out isCommandActive);
        Execute.ThrownExceptions.Subscribe(ex =>
            this.Log().ErrorException("Error during stretching!", ex));
        AcceptCommand = new RelayCommand<Window>(this.Accept, x => AcceptCommandCanExecute());
        CancelCommand = new RelayCommand<Window>(this.Cancel);
    }

    private IObservable<Unit> ExecuteOperationOnImage() =>
        Observable.Start(() =>
        {
            AfterImage = Observable
                .Return<System.Drawing.Bitmap>(ImageConverterHelper.ConvertFromAvaloniaUIBitmap(_beforeImage))
                .Select(x =>
                    _imageWrap > 0 && (int)_imageWrap < 4
                        ? imageBorderService.Execute(x, _imageWrap, 5, 5, 5, 5,
                            System.Drawing.Color.FromArgb((int)_borderConstVal, (int)_borderConstVal,
                                (int)_borderConstVal))
                        : x)
                .Select(bitmap =>
                {
                    IConvolutionMatrix matrices = null;
                    double[,] matrix = null;

                    if (!_isEdgeDetectionSelected)
                    {
                        matrices = _is3x3Selected
                            ? new ConvolutionMatrices3x3()
                            : (_is5x5Selected
                                ? new ConvolutionMatrices5x5()
                                : (_is7x7Selected
                                    ? new ConvolutionMatrices7x7()
                                    : (_is9x9Selected
                                        ? new ConvolutionMatrices9x9()
                                        : throw new Exception("Operation not found!"))
                                ));

                        matrix = MatrixSelector(_isSobelSelected, matrices);
                    }

                    if ((int)_selectedSoftenSharpen < 3 && !_isSobelSelected && !_isEdgeDetectionSelected)
                    {
                        return imageConvolutionService.Execute(bitmap, matrix, _value, true);
                    }
                    else if (_isSobelSelected && !_isEdgeDetectionSelected)
                    {
                        return imageConvolutionService.Execute(bitmap, matrix, _value);
                    }
                    else if (_isEdgeDetectionSelected && _selectedEdgeDetection != EdgeDetectionEnum.Canny)
                    {
                        return imageConvolutionService.Execute(bitmap,
                            EdgeDetection.EdgeDetectionMatrices[_selectedEdgeDetection], _value);
                    }
                    else
                    {
                        var image = imageConvolutionService.Execute(bitmap,
                            EdgeDetection.EdgeDetectionMatrices[_selectedEdgeDetection], _value, true);
                        var gradientMagnitude =
                            imageConvolutionService.ComputeGradient(image,
                                (gx, gy) => Math.Sqrt(gx * gx + gy * gy));
                        var gradientDirection =
                            imageConvolutionService.ComputeGradient(image,
                                (gx, gy) => Math.Atan2(gy, gx) * 180 / Math.PI);

                        var nonMax = imageConvolutionService.NonMaxSupression(gradientMagnitude, gradientDirection);
                        var thresh = imageConvolutionService.HysteresisThresholding(image.Width, image.Height, 100,
                            210, gradientMagnitude);

                        return thresh;
                    }
                })
                .Select(bitmap => BorderAfter(bitmap))
                .Select(x => ImageConverterHelper.ConvertFromSystemDrawingBitmap(x)).FirstOrDefault();
        });

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