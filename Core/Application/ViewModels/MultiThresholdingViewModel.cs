using System;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.Input;
using ImageManipulator.Application.Common.CQRS.Queries.GetImageAfterMultiThreshold;
using ImageManipulator.Domain.Common.CQRS.Interfaces;
using Splat;

namespace ImageManipulator.Application.ViewModels
{
    public class MultiThresholdingViewModel : ImageOperationDialogViewModelBase
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private Bitmap? _beforeImage;
        private Bitmap? _afterImage;
        private int _enteredLowerThreshold;
        private int _enteredUpperThreshold;
        public override Bitmap? BeforeImage
        {
            get => _beforeImage; set
            {
                _ = this.RaiseAndSetIfChanged(ref _beforeImage, value);
            }
        }

        public override Bitmap? AfterImage
        {
            get => _afterImage; set
            {
                this.RaiseAndSetIfChanged(ref _afterImage, value);
            }
        }

        public int EnteredUpperThreshold { get => _enteredUpperThreshold; set => this.RaiseAndSetIfChanged(ref _enteredUpperThreshold, value); }
        public int EnteredLowerThreshold { get => _enteredLowerThreshold; set => this.RaiseAndSetIfChanged(ref _enteredLowerThreshold, value); }

        public bool ReplaceColours { get; set; }

        #region Commands

        public ReactiveCommand<Unit, Unit> ThresholdingCommand { get; }

        #endregion Commands

        public MultiThresholdingViewModel(IQueryDispatcher queryDispatcher)
        {
            _queryDispatcher = queryDispatcher;
            ThresholdingCommand = ReactiveCommand.CreateFromObservable(() => Observable.StartAsync(ExecuteThresholding));
            ThresholdingCommand.IsExecuting.ToProperty(this, x => x.IsCommandActive, out isCommandActive);
            ThresholdingCommand.ThrownExceptions.Subscribe(ex =>
                this.Log().Error("Error during thresholding!", ex));
            AcceptCommand = new RelayCommand<Window>(Accept, x => AcceptCommandCanExecute());
            CancelCommand = new RelayCommand<Window>(Cancel);
        }

        private async Task ExecuteThresholding()
        {
            AfterImage = await _queryDispatcher.Dispatch<GetImageAfterMultiThresholdQuery, Bitmap>(new GetImageAfterMultiThresholdQuery
            {
                LowerThreshold = _enteredLowerThreshold,
                UpperThreshold = _enteredUpperThreshold,
                ReplaceColours = ReplaceColours
            }, new CancellationToken());
        }
    }
}