using Avalonia.ReactiveUI;
using ImageManipulator.Application.ViewModels;
using ReactiveUI;

namespace ImageManipulator.Presentation.Views
{
    public partial class ContrastStretchingView : ReactiveUserControl<ContrastStretchingViewModel>
    {
        public ContrastStretchingView()
        {
            InitializeComponent();
            this.BindCommand(ViewModel, vm => vm.ExecuteLinearStretching, v => v.StretchingCommand);
            this.BindCommand(ViewModel, vm => vm.AcceptCommand, execute => execute.AcceptCommand);
            this.BindCommand(ViewModel, vm => vm.CancelCommand, execute => execute.CancelCommand);
        }
    }
}
