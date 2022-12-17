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
            this.WhenActivated(disposables => { });
        }
    }
}
