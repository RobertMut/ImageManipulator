using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.Input;
using ImageManipulator.Application.Common.CQRS.Queries.GetImageAfterContrastStretch;
using ImageManipulator.Application.Common.CQRS.Queries.GetImageThreshold;
using ImageManipulator.Domain.Common.CQRS.Interfaces;
using ReactiveUI;

namespace ImageManipulator.Application.ViewModels;

public class ContrastStretchingViewModel : ImageOperationDialogViewModelBase
{
    private readonly IQueryDispatcher _queryDispatcher;
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
    
    public Threshold? Threshold { get => _threshold; set => this.RaiseAndSetIfChanged(ref _threshold, value); }

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
        
        ExecuteLinearStretching = ReactiveCommand.CreateFromObservable(() => Observable.StartAsync(StretchContrast));
        ExecuteLinearStretching.IsExecuting.ToProperty(this, x => x.IsCommandActive, out isCommandActive);
        CalculateSuggestions =
            ReactiveCommand.CreateFromObservable(() => Observable.StartAsync(CalculateSuggestedThresholds));
        CalculateSuggestions.IsExecuting.ToProperty(this, x => x.IsCommandActive, out isCommandActive);

        AcceptCommand = new RelayCommand<Window>(Accept, x => AcceptCommandCanExecute());
        CancelCommand = new RelayCommand<Window>(Cancel);
    }

    private async Task CalculateSuggestedThresholds()
    {
        Threshold = await _queryDispatcher.Dispatch<GetImageThresholdQuery, Threshold>(new GetImageThresholdQuery()
        {
            HistogramValues = HistogramValues
        }, new CancellationToken());
    }

    private async Task StretchContrast()
    {
        AfterImage = await _queryDispatcher.Dispatch<GetImageAfterContrastStretchQuery, Bitmap>(
            new GetImageAfterContrastStretchQuery
            {
                Min = _enteredLowerThreshold,
                Max = _enteredUpperThreshold
            }, new CancellationToken());
    }
}