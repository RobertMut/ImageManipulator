using Avalonia.ReactiveUI;
using ImageManipulator.Application.ViewModels;
using ReactiveUI;

namespace ImageManipulator.Presentation.Views
{
    public partial class ThresholdingView : ReactiveUserControl<ThresholdingViewModel>
    {
        public ThresholdingView()
        {
            InitializeComponent();
            this.BindCommand(ViewModel, vm => vm.TresholdingCommand, v => v.thresholdCommand);
            this.WhenActivated(_ => { });
        }
    }
}
