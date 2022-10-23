using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Application.Common.Models;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using Bitmap = Avalonia.Media.Imaging.Bitmap;

namespace ImageManipulator.Application.ViewModels;

public class MainWindowViewModel : ViewModelBase, IScreen
{
    private readonly IFileService _fileService;
    private readonly ICommonDialogService _commonDialogService;
    private readonly IGraphService graphService;
    private readonly IImageDataService imageDataService;
    private readonly IServiceProvider serviceProvider;

    private ObservableCollection<TabItem> _tabs = new ObservableCollection<TabItem>();
    private TabItem _currentTab;

    public RoutingState Router { get; } = new RoutingState();

    public ObservableCollection<TabItem> ImageTabs
    {
        get => _tabs;
        set => this.RaiseAndSetIfChanged(ref _tabs, value);
    }

    public TabItem CurrentTab
    {
        get => _currentTab;
        set => this.RaiseAndSetIfChanged(ref _currentTab, value);
    }

    #region Commands

    public ReactiveCommand<Unit, Unit> AddNewTab { get; }

    public ReactiveCommand<Unit, Unit> GetImageToTab { get; }
    public ReactiveCommand<Unit, Unit> Exit { get; }
    public ReactiveCommand<Unit, Unit> SaveImageCommand { get; }
    public ReactiveCommand<Unit, Unit> SaveImageAsCommand { get; }
    public ReactiveCommand<Unit, Unit> DuplicateCommand { get; }

    #endregion Commands

    public MainWindowViewModel(IFileService fileService, ICommonDialogService commonDialogService, IGraphService graphService, IImageDataService imageDataService, IServiceProvider serviceProvider)
    {
        _fileService = fileService;
        _commonDialogService = commonDialogService;
        this.graphService = graphService;
        this.imageDataService = imageDataService;
        this.serviceProvider = serviceProvider;
        var emptyTab = new TabItem(serviceProvider.GetRequiredService<TabControlViewModel>());
        CurrentTab = emptyTab;
        ImageTabs.Add(emptyTab);

        AddNewTab = ReactiveCommand.Create(NewEmptyTab);
        GetImageToTab = ReactiveCommand.Create(PrepareNewTab);

        Exit = ReactiveCommand.Create(CloseApp);
        SaveImageCommand = ReactiveCommand.Create(SaveImage);
        SaveImageAsCommand = ReactiveCommand.Create(SaveImageAs);
        DuplicateCommand = ReactiveCommand.Create(Duplicate);
    }

    private void NewEmptyTab()
    {
        _tabs.Add(new TabItem(serviceProvider.GetRequiredService<TabControlViewModel>()));
    }

    private void PrepareNewTab()
    {
        string[] filePath = _commonDialogService.ShowFileDialogInNewWindow().Result;
        if (filePath != null)
        {
            var tab = _tabs.Where(x => x.Name == filePath[0]).FirstOrDefault();

            if (tab != null)
            {
                _tabs.Remove(tab);
            }

            if (filePath.Length > 1)
            {
                throw new NotImplementedException();
            }

            int tabIndex = _tabs.IndexOf(_currentTab);
            var openedImageBitmap = new Bitmap(filePath[0]);

            var tabToReplace = new TabItem(Path.GetFileName(filePath[0]), serviceProvider.GetRequiredService<TabControlViewModel>());
            tabToReplace.ViewModel.LoadImage(openedImageBitmap, filePath[0]);

            ImageTabs[tabIndex] = tabToReplace;
            CurrentTab = tabToReplace;
        }
    }

    private void SaveImage()
    {
        _currentTab.ViewModel.Image.Save(_currentTab.ViewModel.Path);
    }

    private void SaveImageAs()
    {
        _commonDialogService.ShowSaveFileDialog(_currentTab.ViewModel.Image, _currentTab.ViewModel.Path);
    }

    private void Duplicate()
    {
        ImageTabs.Add(_currentTab);
        CurrentTab = _currentTab;
    }

    private void CloseApp()
    {
        Environment.Exit(1);
    }
}