using Avalonia.ReactiveUI;
using ImageManipulator.Application.ViewModels;
using ReactiveUI;

namespace ImageManipulator.Presentation.Views
{
    public partial class ThresholdView : ReactiveUserControl<ThresholdViewModel>
    {
        public ThresholdView()
        {
            InitializeComponent();
            this.BindCommand(ViewModel, vm => vm.ThresholdCommand, v => v.ThresholdCommand);
            this.BindCommand(ViewModel, vm => vm.AcceptCommand, execute => execute.AcceptCommand);
            this.BindCommand(ViewModel, vm => vm.CancelCommand, execute => execute.CancelCommand);
        }
    }
}
