using Avalonia.Media;
using Avalonia.Media.Imaging;
using ImageManipulator.Application.Common.Enums;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Common.Enums;
using ReactiveUI;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using ImageManipulator.Application.Common.CQRS.Queries.GetImageAfterArithmetic;
using ImageManipulator.Application.Common.CQRS.Queries.GetImageAfterBitwise;
using ImageManipulator.Domain.Common.CQRS.Interfaces;

namespace ImageManipulator.Application.ViewModels;

public class ArithmeticBitwiseOperationsViewModel : ImageOperationDialogViewModelBase
{
    private readonly IQueryDispatcher _queryDispatcher;
    private readonly ICommonDialogService _commonDialogService;
    private Bitmap? _afterImage;
    private Bitmap? _beforeImage;
    private Bitmap? _operationImage;
    private Color _pickedColor;
    private int? _value;
    private bool _isArithmeticSelected;
    private ElementaryOperationParameterType _elementaryOperation;
    private ArithmeticOperationType _arithmeticOperation;
    private BitwiseOperationType _bitwiseOperation;

    private ObservableAsPropertyHelper<bool> _isSelecting;

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

    public Bitmap? OperationImage
    {
        get => _operationImage;
        set => this.RaiseAndSetIfChanged(ref _operationImage, value);
    }

    public Color PickedColor
    {
        get => _pickedColor;
        set => this.RaiseAndSetIfChanged(ref _pickedColor, value);
    }

    public int? Value
    {
        get => _value;
        set => this.RaiseAndSetIfChanged(ref _value, value);
    }

    public bool IsArithmeticSelected
    {
        get => _isArithmeticSelected;
        set => this.RaiseAndSetIfChanged(ref _isArithmeticSelected, value);
    }

    public int SelectedElementaryOperation
    {
        get => (int)_elementaryOperation;
        set => this.RaiseAndSetIfChanged(ref _elementaryOperation, (ElementaryOperationParameterType)value);
    }

    public int SelectedArithmeticOperation
    {
        get => (int)_arithmeticOperation;
        set => this.RaiseAndSetIfChanged(ref _arithmeticOperation, (ArithmeticOperationType)value);
    }

    public int SelectedBitwiseOperation
    {
        get => (int)_bitwiseOperation;
        set => this.RaiseAndSetIfChanged(ref _bitwiseOperation, (BitwiseOperationType)value);
    }

    #region Commands

    public ReactiveCommand<Unit, Unit> Execute { get; }
    public ReactiveCommand<Unit, Unit> SelectImage { get; }

    public bool IsSelecting => _isSelecting.Value;

    #endregion Commands

    public ArithmeticBitwiseOperationsViewModel(IQueryDispatcher queryDispatcher,
        ICommonDialogService commonDialogService)
    {
        _queryDispatcher = queryDispatcher;
        this._commonDialogService = commonDialogService;
        SelectImage = ReactiveCommand.CreateFromObservable(() => Observable.StartAsync(SelectImageCommand));
        SelectImage.IsExecuting.ToProperty(this, x => x.IsSelecting, out _isSelecting);

        Execute = ReactiveCommand.CreateFromObservable(() => Observable.StartAsync(ExecuteOperationOnImage));
        Execute.IsExecuting.ToProperty(this, x => x.IsCommandActive, out isCommandActive);

        AcceptCommand = new RelayCommand<Window>(Accept, x => AcceptCommandCanExecute());
        CancelCommand = new RelayCommand<Window>(Cancel);
    }

    private async Task ExecuteOperationOnImage()
    {
        if (_isArithmeticSelected)
        {
            AfterImage = await _queryDispatcher.Dispatch<GetImageAfterArithmeticQuery, Avalonia.Media.Imaging.Bitmap>(
                new GetImageAfterArithmeticQuery
                {
                    OperationValue = _value.HasValue ? _value.Value : default,
                    OperationImage = _operationImage,
                    OperationColor = _pickedColor,
                    ElementaryOperationParameterType = _elementaryOperation,
                    ArithmeticOperationType = _arithmeticOperation
                }, new CancellationToken());
        }
        else
        {
            AfterImage = await _queryDispatcher.Dispatch<GetImageAfterBitwiseQuery, Bitmap>(
                new GetImageAfterBitwiseQuery
                {
                    OperationValue = _value.HasValue ? _value.Value : default,
                    OperationImage = _operationImage,
                    OperationColor = _pickedColor,
                    ElementaryOperationParameterType = _elementaryOperation,
                    BitwiseOperationType = _bitwiseOperation
                }, new CancellationToken());
        }
    }

    private async Task SelectImageCommand()
    {
        var storageFile = await _commonDialogService.ShowFileDialogInNewWindow();
        await using Stream fileStream = await storageFile.OpenReadAsync();
        OperationImage = new Bitmap(fileStream);
    }
}