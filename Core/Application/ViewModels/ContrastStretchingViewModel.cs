using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.Input;
using ImageManipulator.Application.Common.CQRS.Queries.GetImageThreshold;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Domain.Common.CQRS.Interfaces;
using ImageManipulator.Domain.Common.Helpers;
using ReactiveUI;

namespace ImageManipulator.Application.ViewModels;

public class ContrastStretchingViewModel : ImageOperationDialogViewModelBase
{
    private readonly IQueryDispatcher _queryDispatcher;
    private readonly IImagePointOperationsService _imagePointOperationsService;
    private Bitmap? _beforeImage;
    private Bitmap? _afterImage;
    private Threshold? _threshold;
    private int _enteredLowerThreshold;
    private int _enteredUpperThreshold;

    public override Bitmap? BeforeImage
    {
        get => _beforeImage;
        set => _ = this.RaiseAndSetIfChanged(ref _beforeImage, value);
    }

    public override Bitmap? AfterImage
    {
        get => _afterImage;
        set { this.RaiseAndSetIfChanged(ref _afterImage, value); }
    }

    public int[]? HistogramValues { get; set; }

    public int LowerThreshold
    {
        get => _threshold.Lower;
        set
        {
            int backing = _threshold.Lower;
            this.RaisePropertyChanged();
            _threshold.Lower = value;
        }
    }

    public int UpperThreshold
    {
        get => _threshold.Upper;
        set
        {
            int backing = _threshold.Upper;
            this.RaisePropertyChanged();
            _threshold.Upper = value;
        }
    }

    public int EnteredLowerThreshold
    {
        get => _enteredLowerThreshold;
        set => this.RaiseAndSetIfChanged(ref _enteredLowerThreshold, value);
    }

    public int EnteredUpperThreshold
    {
        get => _enteredUpperThreshold;
        set => this.RaiseAndSetIfChanged(ref _enteredUpperThreshold, value);
    }

    #region Commands

    public ReactiveCommand<Unit, Unit> ExecuteLinearStretching { get; }
    public ReactiveCommand<Unit, Unit> CalculateSuggestions { get; }

    #endregion

    public ContrastStretchingViewModel(IQueryDispatcher queryDispatcher)
    {
        _queryDispatcher = queryDispatcher;
        _threshold = new Threshold { Upper = 0, Lower = 0 };
        
        ExecuteLinearStretching = ReactiveCommand.CreateFromObservable(LinearlyStreatchContrast);
        ExecuteLinearStretching.IsExecuting.ToProperty(this, x => x.IsCommandActive, out isCommandActive);
        CalculateSuggestions =
            ReactiveCommand.CreateFromObservable(() => Observable.StartAsync(CalculateSuggestedThresholds));
        CalculateSuggestions.IsExecuting.ToProperty(this, x => x.IsCommandActive, out isCommandActive);

        AcceptCommand = new RelayCommand<Window>(Accept, x => AcceptCommandCanExecute());
        CancelCommand = new RelayCommand<Window>(Cancel);
    }

    public async Task CalculateSuggestedThresholds()
    {
        var threshold = await _queryDispatcher.Dispatch<GetImageThresholdQuery, Threshold>(new GetImageThresholdQuery()
        {
            HistogramValues = HistogramValues
        }, new CancellationToken());
        
        LowerThreshold = threshold.Lower;
        UpperThreshold = threshold.Upper;
    }

    private IObservable<Unit> LinearlyStreatchContrast() =>
        Observable.Start(() =>
        {
            var stretchedImage = _imagePointOperationsService.StretchContrast(
                ImageConverterHelper.ConvertFromAvaloniaUIBitmap(_beforeImage), _enteredLowerThreshold,
                _enteredUpperThreshold);
            AfterImage = ImageConverterHelper.ConvertFromSystemDrawingBitmap(stretchedImage);
        });
}