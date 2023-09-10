using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.Input;
using ImageManipulator.Application.Common.CQRS.Queries.GetImageAfterNonLinearContrastStretching;
using ImageManipulator.Domain.Common.CQRS.Interfaces;
using ReactiveUI;

namespace ImageManipulator.Application.ViewModels;

public class NonLinearContrastStretchingViewModel : ImageOperationDialogViewModelBase
{
    private readonly IQueryDispatcher _queryDispatcher;
    private Bitmap? _beforeImage;
    private Bitmap? _afterImage;
    private double _gamma;

    public override Bitmap? BeforeImage
    {
        get => _beforeImage;
        set => _ = this.RaiseAndSetIfChanged(ref _beforeImage, value);
    }

    public override Bitmap? AfterImage
    {
        get => _afterImage;
        set => this.RaiseAndSetIfChanged(ref _afterImage, value);
    }

    public double GammaValue
    {
        get => _gamma;
        set => this.RaiseAndSetIfChanged(ref _gamma, value);
    }

    #region Commands

    public ReactiveCommand<Unit, Unit> ExecuteNonLinearStretching { get; }

    #endregion

    public NonLinearContrastStretchingViewModel(IQueryDispatcher queryDispatcher)
    {
        _queryDispatcher = queryDispatcher;
        ExecuteNonLinearStretching =
            ReactiveCommand.CreateFromObservable(() => Observable.StartAsync(NonLinearlyStretchContrast));
        ExecuteNonLinearStretching.IsExecuting.ToProperty(this, x => x.IsCommandActive, out isCommandActive);

        AcceptCommand = new RelayCommand<Window>(Accept, x => AcceptCommandCanExecute());
        CancelCommand = new RelayCommand<Window>(Cancel);
    }

    private async Task NonLinearlyStretchContrast()
    {
        AfterImage = await _queryDispatcher.Dispatch<GetImageAfterNonLinearContrastStretchingQuery, Bitmap>(
            new GetImageAfterNonLinearContrastStretchingQuery()
            {
                Gamma = _gamma
            }, new CancellationToken());
    }
}