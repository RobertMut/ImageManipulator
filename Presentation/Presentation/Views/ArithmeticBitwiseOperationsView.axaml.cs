using Avalonia.ReactiveUI;
using ImageManipulator.Application.ViewModels;
using ReactiveUI;

namespace ImageManipulator.Presentation.Views
{
    public partial class ArithmeticBitwiseOperationsView : ReactiveUserControl<ArithmeticBitwiseOperationsViewModel>
    {
        public ArithmeticBitwiseOperationsView()
        {
            InitializeComponent();
            this.BindCommand(ViewModel, vm => vm.Execute, v => v.OperationCommand);
            this.BindCommand(ViewModel, vm => vm.AcceptCommand, execute => execute.AcceptCommand);
            this.BindCommand(ViewModel, vm => vm.CancelCommand, execute => execute.CancelCommand);
        }
    }
}
