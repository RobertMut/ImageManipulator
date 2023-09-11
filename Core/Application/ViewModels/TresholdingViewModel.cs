using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.Input;
using ImageManipulator.Application.Common.CQRS.Queries.GetImageThreshold;
using ImageManipulator.Domain.Common.CQRS.Interfaces;

namespace ImageManipulator.Application.ViewModels;

public class ThresholdingViewModel : ImageOperationDialogViewModelBase
{
    private readonly IQueryDispatcher _queryDispatcher;
    private Bitmap? _beforeImage;
    private Bitmap? _afterImage;
    private int _enteredThreshold;

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

    public bool ReplaceColours { get; set; }

    #region Commands

    public ReactiveCommand<Unit, Unit> TresholdingCommand { get; }

    #endregion Commands

    public ThresholdingViewModel(IQueryDispatcher queryDispatcher)
    {
        _queryDispatcher = queryDispatcher;
        TresholdingCommand = ReactiveCommand.CreateFromObservable(() => Observable.StartAsync(ExecuteTreshold));
        TresholdingCommand.IsExecuting.ToProperty(this, x => x.IsCommandActive, out isCommandActive);

        AcceptCommand = new RelayCommand<Window>(Accept, x => AcceptCommandCanExecute());
        CancelCommand = new RelayCommand<Window>(Cancel);
    }

    private async Task ExecuteTreshold()
    {
        AfterImage = await _queryDispatcher.Dispatch<GetImageAfterThresholdQuery, Bitmap>(
            new GetImageAfterThresholdQuery
            {
                Threshold = _enteredThreshold,
                ReplaceColour = ReplaceColours
            }, new CancellationToken());
    }
}