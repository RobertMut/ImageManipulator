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
            this.WhenActivated(disposables => { });
        }
    }
}
