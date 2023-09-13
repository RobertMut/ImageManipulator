using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace ImageManipulator.Domain.Common.Extensions
{
    public static class AvaloniaExtensions
    {
        public static Window SetContent<TViewModel>(this Window window, TViewModel viewModel) where TViewModel : class
        {
            window.Content = viewModel;
            return window;
        }

        public static Window? GetCurrentWindow(this Application? application)
        {
            if (application?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                return desktop.MainWindow;
            return null;
        }
    }
}
