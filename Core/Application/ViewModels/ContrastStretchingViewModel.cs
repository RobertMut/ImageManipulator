using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.Input;
using DynamicData.Binding;
using ImageManipulator.Application.Common.CQRS.Queries.GetImageAfterContrastStretch;
using ImageManipulator.Application.Common.CQRS.Queries.GetImageThresholdLevels;
using ImageManipulator.Domain.Common.CQRS.Interfaces;
using ReactiveUI;

namespace ImageManipulator.Application.ViewModels;

public class ContrastStretchingViewModel : ImageOperationDialogViewModelBase
{
    private readonly IQueryDispatcher _queryDispatcher;
    private Bitmap? _beforeImage;
    private Bitmap? _afterImage;
    private ThresholdLevels? _threshold;
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
    
    public ThresholdLevels? Threshold { get => _threshold; set => this.RaiseAndSetIfChanged(ref _threshold, value); }

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
        _threshold = new ThresholdLevels { Upper = 0, Lower = 0 };
        
        ExecuteLinearStretching = ReactiveCommand.CreateFromObservable(() => Observable.StartAsync(StretchContrast));
        ExecuteLinearStretching.IsExecuting.ToProperty(this, x => x.IsCommandActive, out isCommandActive);
        CalculateSuggestions =
            ReactiveCommand.CreateFromObservable(() => Observable.StartAsync(CalculateSuggestedThresholds));
        CalculateSuggestions.IsExecuting.ToProperty(this, x => x.IsCommandActive, out isCommandActive);

        AcceptCommand = ReactiveCommand.CreateFromTask<Window>(Accept, this.WhenAnyValue(x => x.AfterImage).Select(x => x != null), RxApp.TaskpoolScheduler);
        CancelCommand = ReactiveCommand.CreateFromTask<Window>(Cancel);
    }

    private async Task CalculateSuggestedThresholds()
    {
        Threshold = await _queryDispatcher.Dispatch<GetImageThresholdLevelsQuery, ThresholdLevels>(new GetImageThresholdLevelsQuery()
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