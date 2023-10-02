using Avalonia.ReactiveUI;
using ImageManipulator.Application.ViewModels;
using ReactiveUI;

namespace ImageManipulator.Presentation.Views
{
    public partial class NonLinearContrastStretchingView : ReactiveUserControl<NonLinearContrastStretchingViewModel>
    {
        public NonLinearContrastStretchingView()
        {
            InitializeComponent();
            this.BindCommand(ViewModel, vm => vm.ExecuteNonLinearStretching, v => v.StretchCommand);
            this.BindCommand(ViewModel, vm => vm.AcceptCommand, execute => execute.AcceptCommand);
            this.BindCommand(ViewModel, vm => vm.CancelCommand, execute => execute.CancelCommand);
        }
    }
}
