using Avalonia.ReactiveUI;
using ImageManipulator.Application.ViewModels;
using ReactiveUI;

namespace ImageManipulator.Presentation.Views
{
    public partial class MultiThresholdView : ReactiveUserControl<MultiThresholdViewModel>
    {
        public MultiThresholdView()
        {
            InitializeComponent();
            this.BindCommand(ViewModel, vm => vm.ThresholdingCommand, execute => execute.thresholdCommand);
            this.BindCommand(ViewModel, vm => vm.AcceptCommand, execute => execute.AcceptCommand);
            this.BindCommand(ViewModel, vm => vm.CancelCommand, execute => execute.CancelCommand);
        }
    }
}
