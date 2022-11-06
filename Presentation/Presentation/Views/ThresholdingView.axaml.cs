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
            this.WhenActivated(disposables => { });
        }
    }
}
