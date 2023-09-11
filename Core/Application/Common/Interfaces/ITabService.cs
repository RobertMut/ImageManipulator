using System.Collections.ObjectModel;
using ImageManipulator.Application.ViewModels;
using TabItem = ImageManipulator.Application.Common.Models.TabItem;

namespace ImageManipulator.Application.Common.Interfaces;

public interface ITabService
{
    TabItem GetTab(string name);
    ObservableCollection<TabItem> GetTabItems();
    void RemoveTab(string name);
    TabItem Duplicate(string name);
    void SetTabs(ObservableCollection<TabItem> tabItems);
    TabItem AddExistingTab(string name, TabControlViewModel viewModel);
    TabItem AddEmpty(TabItem tabItem);
    TabItem Replace(string name, TabItem tabItem);
    string CurrentTabName { get; }
}