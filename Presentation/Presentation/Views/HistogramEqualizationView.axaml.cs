using Avalonia.Markup.Xaml;
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
            this.WhenActivated(disposables => { });
        }
    }
}
