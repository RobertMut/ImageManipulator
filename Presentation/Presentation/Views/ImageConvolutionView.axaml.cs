using Avalonia.ReactiveUI;
using ImageManipulator.Application.ViewModels;
using ReactiveUI;

namespace ImageManipulator.Presentation.Views
{
    public partial class ImageConvolutionView : ReactiveUserControl<ImageConvolutionViewModel>
    {
        public ImageConvolutionView()
        {
            InitializeComponent();
            this.BindCommand(ViewModel, vm => vm.Execute, v => v.convolutionCommand);
            this.WhenActivated(disposables => { });
        }
    }
}
