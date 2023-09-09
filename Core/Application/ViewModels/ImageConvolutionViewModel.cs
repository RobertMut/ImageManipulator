using ImageManipulator.Common.Enums;
using ReactiveUI;
using System;
using System.Drawing;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using ImageManipulator.Application.Common.CQRS.Queries.GetPostConvolutionImage;
using ImageManipulator.Domain.Common.CQRS.Interfaces;
using Splat;
using Bitmap = Avalonia.Media.Imaging.Bitmap;

namespace ImageManipulator.Application.ViewModels;

public class ImageConvolutionViewModel : ImageOperationDialogViewModelBase
{
    private readonly IQueryDispatcher _dispatcher;
    private Bitmap? _afterImage;
    private Bitmap? _beforeImage;
    private int _value;
    private bool _isSobelSelected;
    private SoftenSharpenEnum _selectedSoftenSharpen;
    private SobelEnum _selectedSobel;
    private ImageWrapEnum _imageWrap;
    private bool _isWeightedSelected;
    private MatrixSize _matrixSize;
    private bool _isEdgeDetectionSelected;
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

    public int Value
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

    public int MatrixSize
    {
        get => (int)_matrixSize;
        set => this.RaiseAndSetIfChanged(ref _matrixSize, (MatrixSize)value);
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

    public ImageConvolutionViewModel(IQueryDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
        Execute = ReactiveCommand.CreateFromObservable(() => Observable.StartAsync(ExecuteOperationOnImage));
        Execute.IsExecuting.ToProperty(this, x => x.IsCommandActive, out isCommandActive);

        AcceptCommand = new RelayCommand<Window>(Accept, x => AcceptCommandCanExecute());
        CancelCommand = new RelayCommand<Window>(Cancel);
    }

    private async Task ExecuteOperationOnImage()
    {
        AfterImage = await _dispatcher.Dispatch<GetPostConvolutionImageCommand, Bitmap>(new GetPostConvolutionImageCommand
        {
            Sobel = _isSobelSelected,
            EdgeDetection = _isEdgeDetectionSelected,
            Color = Color.FromArgb(_value, _value, _value),
            Value = _value,
            SoftenSharpenType = _selectedSoftenSharpen,
            SobelType = _selectedSobel,
            MatrixSize = _matrixSize,
            EdgeDetectionType = _selectedEdgeDetection,
            ImageWrapType = _imageWrap
        }, new CancellationToken());
    }

    private T ChangeAndSetWeightedBool<T>(ref T existing, T value)
    {
        if (Convert.ToInt32(value) == 2) IsWeightedSelected = true;
        else IsWeightedSelected = false;
        return this.RaiseAndSetIfChanged(ref existing, value);
    }
}