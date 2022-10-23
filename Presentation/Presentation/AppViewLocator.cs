using Avalonia.Controls;
using Avalonia.Controls.Templates;
using ImageManipulator.Application.ViewModels;
using ImageManipulator.Presentation.Views;
using ReactiveUI;
using System;

namespace ImageManipulator.Presentation;

public class AppViewLocator : IDataTemplate, IViewLocator
{
    public IControl Build(object data)
    {
        var name = data.GetType().FullName!.Replace("ViewModel", "View");
        var type = Type.GetType(name);

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
        _ => throw new ArgumentOutOfRangeException(nameof(viewModel))
    };
}
