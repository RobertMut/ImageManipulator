using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Application.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using TabItem = ImageManipulator.Application.Common.Models.TabItem;

namespace ImageManipulator.Infrastructure.Tab;

public class TabService : ITabService
{
    private readonly IServiceProvider _serviceProvider;
    private Dictionary<string, TabItem> _tabItems;
    private int _nameIterator;
    
    private int NameIterator
    {
        get => _nameIterator++;
    }
    
    public TabService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _tabItems = new Dictionary<string, TabItem>();
        AddEmpty(new TabItem( $"Tab {NameIterator}", serviceProvider.GetRequiredService<TabControlViewModel>()));
    }
    
    public TabItem GetTab(string name)
    {
        _tabItems.TryGetValue(name, out TabItem value);

        if (value != null) return value;
        
        return default;
    }

    public ObservableCollection<TabItem> GetTabItems() => new(_tabItems.Values);

    public void SetTabs(ObservableCollection<TabItem> tabItems)
    {
        _tabItems = tabItems.ToDictionary(name => name.ViewModel.Path, tabItem => tabItem);
    }
    
    public void RemoveTab(string name) => _tabItems.Remove(name);

    public TabItem AddExistingTab(string name, TabControlViewModel viewModel)
    {
        if (_tabItems.ContainsKey(name))
        {
            name = GenerateNewName(name);
        }

        var tabItem = new TabItem(name, viewModel);

        _tabItems.Add(name, tabItem);

        return tabItem;
    }
    
    public TabItem AddEmpty(TabItem tabItem)
    {
        string name = $"Tab {NameIterator}";
        _tabItems.Add(name, new TabItem(name, _serviceProvider.GetRequiredService<TabControlViewModel>()));

        return tabItem;
    }

    public TabItem Duplicate(string name)
    {
        var tabItem = _tabItems[name];
        string newName = GenerateNewName(name);

        var newTabItem = new TabItem(newName, tabItem.ViewModel);
        _tabItems.Add(newName, newTabItem);

        return newTabItem;
    }

    public TabItem Replace(string name, TabItem tabItem)
    {
        if (_tabItems.TryGetValue(name, out TabItem existingTabItems))
        {
            _tabItems.Remove(name);
            _tabItems.Add(tabItem.ViewModel.Path, tabItem);

            return tabItem;
        }

        throw new NullReferenceException($"TabItem '{name}' does not exist");
    }

    private string GenerateNewName(string name)
    {
        string nameWithoutExtension = Path.GetFileNameWithoutExtension(name);
        string extension = Path.GetExtension(name);
        return $"{nameWithoutExtension}-new{extension}";
    }
}