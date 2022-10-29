using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ImageManipulator.Application;
using ImageManipulator.Application.ViewModels;
using ImageManipulator.Infrastructure;
using ImageManipulator.Presentation.Views;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using Splat;

namespace ImageManipulator.Presentation;

public partial class App : Avalonia.Application
{
    private ServiceProvider _serviceProvider;
    public static Window CurrentWindow { get; private set; }

    public override void Initialize()
    {
        var serviceCollection = new ServiceCollection();
        Configure(serviceCollection);

        _serviceProvider = serviceCollection.BuildServiceProvider();

        Locator.CurrentMutable.RegisterLazySingleton(() => _serviceProvider.GetRequiredService<AppViewLocator>(), typeof(IViewLocator));
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
        serviceDescriptors.AddTransient<IServiceCollection, ServiceCollection>();
        serviceDescriptors.AddTransient<AppViewLocator>();
        serviceDescriptors.AddInfrastructure();
        serviceDescriptors.AddApplication();
        serviceDescriptors.AddSingleton<MainWindowViewModel>();
        serviceDescriptors.AddScoped<TabControlViewModel>();
        serviceDescriptors.AddScoped<ContrastStretchingViewModel>();
        serviceDescriptors.AddScoped<NonLinearContrastStretchingViewModel>();
        serviceDescriptors.AddScoped<HistogramEqualizationViewModel>();
        serviceDescriptors.AddScoped<ThresholdingViewModel>();
        serviceDescriptors.AddScoped<MultiThresholdingViewModel>();
    }
}