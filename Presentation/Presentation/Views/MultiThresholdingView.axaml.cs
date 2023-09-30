using Avalonia.ReactiveUI;
using ImageManipulator.Application.ViewModels;
using ReactiveUI;

namespace ImageManipulator.Presentation.Views
{
    public partial class MultiThresholdingView : ReactiveUserControl<MultiThresholdingViewModel>
    {
        public MultiThresholdingView()
        {
            this.InitializeComponent();
            this.BindCommand(ViewModel, vm => vm.ThresholdingCommand, execute => execute.thresholdCommand);
            this.WhenActivated(_ => { });
        }
    }
}
