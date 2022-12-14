using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ImageManipulator.Application.ViewModels;
using ReactiveUI;

namespace ImageManipulator.Presentation.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
        this.WhenActivated(disposables => {
        });
#if DEBUG
        this.AttachDevTools();
#endif
        AvaloniaXamlLoader.Load(this);
    }
}
