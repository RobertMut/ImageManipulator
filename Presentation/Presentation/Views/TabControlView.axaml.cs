using Avalonia.ReactiveUI;
using ImageManipulator.Application.ViewModels;
using ReactiveUI;

namespace ImageManipulator.Presentation.Views
{
    public partial class TabControlView : ReactiveUserControl<TabControlViewModel>
    {
        public TabControlView()
        {
            InitializeComponent();
            this.WhenActivated(disposables => { });
        }
    }
}
