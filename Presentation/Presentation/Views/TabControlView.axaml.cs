using Avalonia.ReactiveUI;
using ImageManipulator.Application.ViewModels;

namespace ImageManipulator.Presentation.Views
{
    public partial class TabControlView : ReactiveUserControl<TabControlViewModel>
    {
        public TabControlView()
        {
            InitializeComponent();
        }
    }
}
