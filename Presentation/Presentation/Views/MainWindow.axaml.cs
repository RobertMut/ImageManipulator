using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ImageManipulator.Application.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;

namespace ImageManipulator.Presentation.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
        this.WhenActivated(disposables => {
            //this.OneWayBind(ViewModel,
            //    viewModel => viewModel.ImageTabs,
            //    views => views.TabNav.Items)//.Items
            //    .DisposeWith(disposables);
        });
        AvaloniaXamlLoader.Load(this);
    }
}
