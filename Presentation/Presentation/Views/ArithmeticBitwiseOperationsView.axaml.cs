using Avalonia.Markup.Xaml;
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
            this.BindCommand(ViewModel, vm => vm.Execute, v => v.operationCommand);
            this.WhenActivated(disposables => { });
        }
    }
}
