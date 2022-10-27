using Avalonia.Controls;
using Avalonia.Controls.Templates;
using ImageManipulator.Application.ViewModels;
using ImageManipulator.Presentation.Views;
using ReactiveUI;
using System;
using System.Linq;
using System.Reflection;

namespace ImageManipulator.Presentation;

public class AppViewLocator : IDataTemplate, IViewLocator
{
    private const string PresentationAssembly = "ImageManipulator.Presentation.Views";
    private const string ApplicationAssembly = "ImageManipulator.Application";

    public IControl Build(object data)
    {
        var name = $"{PresentationAssembly}{data.GetType().FullName!.Replace("ViewModel", "View").Replace(ApplicationAssembly+".Views", null)}";
        var types = Assembly.GetEntryAssembly().GetTypes();

        var type = types.FirstOrDefault(x => x.FullName == name);
        if (type != null)
        {
            return (Control)Activator.CreateInstance(type)!;
        }

        return new TextBlock { Text = "Not Found: " + name };
    }

    public bool Match(object data)
    {
        return data is ViewModelBase;
    }

    IViewFor? IViewLocator.ResolveView<T>(T viewModel, string? contract) => viewModel switch
    {
        TabControlViewModel context => new TabControlView(),
        ContrastStretchingViewModel context => new ContrastStretchingView(),
        _ => throw new ArgumentOutOfRangeException(nameof(viewModel))
    };
}
