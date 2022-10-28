using Avalonia.ReactiveUI;
using ImageManipulator.Application.ViewModels;
using ReactiveUI;

namespace ImageManipulator.Presentation.Views
{
    public partial class TresholdingView : ReactiveUserControl<TresholdingViewModel>
    {
        public TresholdingView()
        {
            InitializeComponent();
            this.WhenActivated(disposables => { });
        }
    }
}
