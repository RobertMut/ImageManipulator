using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using ImageManipulator.Application.Common.CQRS.Queries.GetImageAfterThreshold;
using ImageManipulator.Domain.Common.CQRS.Interfaces;

namespace ImageManipulator.Application.ViewModels;

public class ThresholdViewModel : ImageOperationDialogViewModelBase
{
    private readonly IQueryDispatcher _queryDispatcher;
    private Bitmap? _beforeImage;
    private Bitmap? _afterImage;
    private int _enteredThreshold;
    private bool _livePreview;

    public override Bitmap? BeforeImage
    {
        get => _beforeImage;
        set { _ = this.RaiseAndSetIfChanged(ref _beforeImage, value); }
    }

    public override Bitmap? AfterImage
    {
        get => _afterImage;
        set { this.RaiseAndSetIfChanged(ref _afterImage, value); }
    }

    public int EnteredThreshold
    {
        get => _enteredThreshold;
        set => this.RaiseAndSetIfChanged(ref _enteredThreshold, value);
    }
    
    public bool LivePreview
    {
        get => _livePreview;
        set => this.RaiseAndSetIfChanged(ref _livePreview, value);
    }

    public bool ReplaceColours { get; set; }

    #region Commands

    public ReactiveCommand<Unit, Unit> SliderInvokedThresholdCommand { get; }
    public ReactiveCommand<Unit, Unit> ThresholdCommand { get; }

    #endregion Commands

    public ThresholdViewModel(IQueryDispatcher queryDispatcher)
    {
        _queryDispatcher = queryDispatcher;
        ThresholdCommand = ReactiveCommand.CreateFromTask(ExecuteThreshold);
        ThresholdCommand.IsExecuting.ToProperty(this, x => x.IsCommandActive, out isCommandActive);
        SliderInvokedThresholdCommand = ReactiveCommand.CreateFromTask(ExecuteThreshold,
            this.WhenAnyValue(x => x.LivePreview));
        SliderInvokedThresholdCommand.IsExecuting.ToProperty(this, x => x.IsCommandActive, out isCommandActive);
        AcceptCommand = ReactiveCommand.CreateFromTask<Window>(Accept, this.WhenAnyValue(x => x.AfterImage).Select(x => x != null), RxApp.TaskpoolScheduler);
        CancelCommand = ReactiveCommand.CreateFromTask<Window>(Cancel);
    }
    
    private async Task ExecuteThreshold()
    {
        AfterImage = await _queryDispatcher.Dispatch<GetImageAfterThresholdQuery, Bitmap>(
            new GetImageAfterThresholdQuery
            {
                Threshold = _enteredThreshold,
                ReplaceColour = ReplaceColours
            }, new CancellationToken());
    }
}