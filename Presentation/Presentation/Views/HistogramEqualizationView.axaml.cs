using Avalonia.ReactiveUI;
using ImageManipulator.Application.ViewModels;
using ReactiveUI;

namespace ImageManipulator.Presentation.Views
{
    public partial class HistogramEqualizationView : ReactiveUserControl<HistogramEqualizationViewModel>
    {
        public HistogramEqualizationView()
        {
            InitializeComponent();
            this.BindCommand(ViewModel, vm => vm.ExecuteEqualizeHistogram, v => v.equalizeCommand);
            this.BindCommand(ViewModel, vm => vm.AcceptCommand, execute => execute.AcceptCommand);
            this.BindCommand(ViewModel, vm => vm.CancelCommand, execute => execute.CancelCommand);
        }
    }
}
