using Avalonia.ReactiveUI;
using ImageManipulator.Application.ViewModels;
using ReactiveUI;

namespace ImageManipulator.Presentation.Views
{
    public partial class MultiThresholdingView : ReactiveUserControl<MultiThresholdingViewModel>
    {
        public MultiThresholdingView()
        {
            InitializeComponent();
            this.WhenActivated(disposables => { });
        }
    }
}
