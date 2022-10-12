using Avalonia.Controls;
using ImageManipulator.Application.ViewModels;
using System;

namespace ImageManipulator.Application.Common.Helpers
{
    public static class ViewModelResolverHelper
    {
        public static Window ResolveViewFromViewModel<T>(T vm) where T : ViewModelBase
        {
            var name = vm.GetType().AssemblyQualifiedName!.Replace("ViewModel", "View");
            var type = Type.GetType(name);
            return type != null ? (Window)Activator.CreateInstance(type)! : null;
        }
    }
}
