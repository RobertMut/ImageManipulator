using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.Input;
using DynamicData.Binding;
using ImageManipulator.Application.Common.CQRS.Queries.GetImageAfterHistogramEqualization;
using ImageManipulator.Domain.Common.CQRS.Interfaces;
using ReactiveUI;

namespace ImageManipulator.Application.ViewModels;

public class HistogramEqualizationViewModel : ImageOperationDialogViewModelBase
{
    private readonly IQueryDispatcher _dispatcher;
    private Bitmap? _beforeImage;
    private Bitmap? _afterImage;
    private double _gamma;
    public int[]?[] Lut;

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

    #region Commands

    public ReactiveCommand<Unit, Unit> ExecuteEqualizeHistogram { get; }

    #endregion

    public HistogramEqualizationViewModel(IQueryDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
        ExecuteEqualizeHistogram = ReactiveCommand.CreateFromTask(Equalization);
        ExecuteEqualizeHistogram.IsExecuting.ToProperty(this, x => x.IsCommandActive, out isCommandActive);
        
        AcceptCommand = ReactiveCommand.CreateFromTask<Window>(Accept, this.WhenAnyValue(x => x.AfterImage).Select(x => x != null), RxApp.TaskpoolScheduler);
        CancelCommand = ReactiveCommand.CreateFromTask<Window>(Cancel);
    }

    private async Task Equalization()
    {
        AfterImage = await _dispatcher.Dispatch<GetImageAfterHistogramEqualizationQuery, Bitmap>(
            new GetImageAfterHistogramEqualizationQuery()
            {
                LookupTable = Lut
            }, new CancellationToken());
    }
}