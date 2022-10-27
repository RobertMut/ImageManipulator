using ImageManipulator.Application.ViewModels;
using ImageManipulator.Domain.Common.Helpers;

namespace ImageManipulator.Application.Common.Models
{
    public class TabItem
    {
        public string Name { get; } = $"Tab {NamingIterator.TabNameIterator++}";
        public TabControlViewModel ViewModel { get; }

        public TabItem(string name, TabControlViewModel viewModel)
        {
            Name = name;
            ViewModel = viewModel;
        }

        public TabItem(TabControlViewModel viewModel) { }
    }
}
