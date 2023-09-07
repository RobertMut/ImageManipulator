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
            this.WhenActivated(disposables => { });
        }
    }
}
