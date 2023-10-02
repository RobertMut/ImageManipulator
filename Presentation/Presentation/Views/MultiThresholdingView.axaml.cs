using System.Reactive.Linq;
using Avalonia.ReactiveUI;
using ImageManipulator.Application.ViewModels;
using ReactiveUI;

namespace ImageManipulator.Presentation.Views
{
    public partial class MultiThresholdingView : ReactiveUserControl<MultiThresholdingViewModel>
    {
        public MultiThresholdingView()
        {
            InitializeComponent();
            this.BindCommand(ViewModel, vm => vm.ThresholdingCommand, execute => execute.thresholdCommand);
            this.BindCommand(ViewModel, vm => vm.AcceptCommand, execute => execute.AcceptCommand);
            this.BindCommand(ViewModel, vm => vm.CancelCommand, execute => execute.CancelCommand);
        }
    }
}
