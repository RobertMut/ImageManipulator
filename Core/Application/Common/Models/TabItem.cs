using Avalonia.Controls.Primitives;
using CommunityToolkit.Mvvm.ComponentModel;
using ImageManipulator.Application.ViewModels;
using ImageManipulator.Common.Helpers;

namespace ImageManipulator.Application.Common.Models
{
    public class TabItem : ObservableObject
    {
        public string Name { get; set; } 
        public TabControlViewModel ViewModel { get; }

        public TabItem(string name, TabControlViewModel viewModel)
        {
            Name = name;
            ViewModel = viewModel;
        }

        public TabItem(TabControlViewModel viewModel)
        {
            ViewModel = viewModel;
        }
    }
}
