using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ImageManipulator.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using ImageManipulator.Presentation.Views;
using ImageManipulator.Application.ViewModels;
using ImageManipulator.Application;

namespace ImageManipulator.Presentation;

public partial class App : Avalonia.Application
{
    private ServiceProvider _serviceProvider;

    public override void Initialize()
    {
        var serviceCollection = new ServiceCollection();
        Configure(serviceCollection);

        _serviceProvider = serviceCollection.BuildServiceProvider();

        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var viewModel = _serviceProvider.GetService<MainWindowViewModel>();
            desktop.MainWindow = new MainWindow
            {
                DataContext = viewModel,
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void Configure(IServiceCollection serviceDescriptors)
    {

        serviceDescriptors.AddInfrastructure();
        serviceDescriptors.AddApplication();
        serviceDescriptors.AddSingleton<MainWindowViewModel>();

    }
}