using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ImageManipulator.Application.ViewModels;
using ReactiveUI;

namespace ImageManipulator.Presentation.Views
{
    public partial class ContrastStretchingView : ReactiveUserControl<ContrastStretchingViewModel>
    {
        public ContrastStretchingView()
        {
            InitializeComponent();
            this.WhenActivated(disposables => { });
        }
    }
}
