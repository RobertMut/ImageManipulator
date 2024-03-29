﻿using ImageManipulator.Application.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using TabItem = ImageManipulator.Application.Common.Models.TabItem;

namespace ImageManipulator.Application.ViewModels;

public class MainWindowViewModel : ViewModelBase, IScreen
{
    private readonly ICommonDialogService _commonDialogService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IImagePointOperationsService _imagePointOperationsService;
    private readonly ITabService _tabService;
    private TabItem? _currentTab;
    private ObservableCollection<TabItem> _imageTabs;

    public RoutingState Router { get; } = new();

    public ObservableCollection<TabItem> ImageTabs
    {
        get => _imageTabs;
        private set => this.RaiseAndSetIfChanged(ref _imageTabs, value);
    }

    public TabItem? CurrentTab
    {
        get
        {
            if(_currentTab is not null)
                _tabService.CurrentTabName = _currentTab.ViewModel.Path;
            return _currentTab;
        }
        set
        {
            this.RaiseAndSetIfChanged(ref _currentTab, value);
        }
    }

    #region Commands
    public ReactiveCommand<Unit, Unit> CloseTab { get; }
    public ReactiveCommand<Unit, Unit> AddNewTab { get; }
    public ReactiveCommand<Unit, Unit> GetImageToTab { get; }
    public ReactiveCommand<Unit, Unit> Exit { get; }
    public ReactiveCommand<Unit, Unit> SaveImageCommand { get; }
    public ReactiveCommand<Unit, Unit> SaveImageAsCommand { get; }
    public ReactiveCommand<Unit, Unit> DuplicateCommand { get; }
    public ReactiveCommand<Unit, Unit> ContrastStretchingCommand { get; }
    public ReactiveCommand<Unit, Unit> GammaCorrectionCommand { get; }
    public ReactiveCommand<Unit, Unit> HistogramEqualizationCommand { get; }
    public ReactiveCommand<Unit, Unit> ThresholdCommand { get; }
    public ReactiveCommand<Unit, Unit> MultiThresholdingCommand { get; }
    public ReactiveCommand<Unit, Unit> NegationCommand { get; }
    public ReactiveCommand<Unit, Unit> ArithmeticBitwiseCommand { get; }
    public ReactiveCommand<Unit, Unit> ImageConvolutionCommand { get; }

    #endregion Commands

    public MainWindowViewModel(ICommonDialogService commonDialogService, IServiceProvider serviceProvider,
        IImagePointOperationsService imagePointOperationsService, ITabService tabService)
    {
        _commonDialogService = commonDialogService;
        _serviceProvider = serviceProvider;
        _imagePointOperationsService = imagePointOperationsService;
        _tabService = tabService;
        _currentTab = _tabService.GetTab("Tab 1");
        _imageTabs = _tabService.GetTabItems();

        AddNewTab = ReactiveCommand.CreateFromTask(NewEmptyTab);
        AddNewTab.IsExecuting.ToProperty(this, x => x.IsCommandActive, out isCommandActive);

        GetImageToTab = ReactiveCommand.CreateFromTask(PrepareNewTab);
        GetImageToTab.IsExecuting.ToProperty(this, x => x.IsCommandActive, out isCommandActive);

        CloseTab = ReactiveCommand.CreateFromTask(CloseCurrentTab, this.WhenAnyValue(x => x.ImageTabs).Select(x => x is
        {
            Count: > 1
        }), RxApp.TaskpoolScheduler);
        
        Exit = ReactiveCommand.CreateFromTask(CloseApp);
        SaveImageCommand = ReactiveCommand.CreateFromTask(SaveImage);
        SaveImageCommand.IsExecuting.ToProperty(this, x => x.IsCommandActive, out isCommandActive);
        
        SaveImageAsCommand = ReactiveCommand.CreateFromTask(SaveImageAs);
        SaveImageAsCommand.IsExecuting.ToProperty(this, x => x.IsCommandActive, out isCommandActive);
        
        DuplicateCommand = ReactiveCommand.CreateFromTask(Duplicate);
        DuplicateCommand.IsExecuting.ToProperty(this, x => x.IsCommandActive, out isCommandActive);

        ContrastStretchingCommand =
            ReactiveCommand.CreateFromTask(OpenContrastStretchWindow);
        ContrastStretchingCommand.IsExecuting.ToProperty(this, x => x.IsCommandActive, out isCommandActive);

        GammaCorrectionCommand =
            ReactiveCommand.CreateFromTask(OpenGammaCorrectionWindow);
        GammaCorrectionCommand.IsExecuting.ToProperty(this, x => x.IsCommandActive, out isCommandActive);

        HistogramEqualizationCommand =
            ReactiveCommand.CreateFromTask(OpenHistogramEqualizationWindow);
        HistogramEqualizationCommand.IsExecuting.ToProperty(this, x => x.IsCommandActive, out isCommandActive);
        
        ThresholdCommand = ReactiveCommand.CreateFromTask(OpenThresholdWindow);
        ThresholdCommand.IsExecuting.ToProperty(this, x => x.IsCommandActive, out isCommandActive);

        MultiThresholdingCommand =
            ReactiveCommand.CreateFromTask(OpenMultiThresholdWindow);
        MultiThresholdingCommand.IsExecuting.ToProperty(this, x => x.IsCommandActive, out isCommandActive);

        NegationCommand = ReactiveCommand.CreateFromTask(NegateImage);
        NegationCommand.IsExecuting.ToProperty(this, x => x.IsCommandActive, out isCommandActive);

        ArithmeticBitwiseCommand =
            ReactiveCommand.CreateFromTask(OpenArithmeticBitwiseWindow);
        ArithmeticBitwiseCommand.IsExecuting.ToProperty(this, x => x.IsCommandActive, out isCommandActive);

        ImageConvolutionCommand =
            ReactiveCommand.CreateFromTask(OpenImageConvolutionWindow);
        ImageConvolutionCommand.IsExecuting.ToProperty(this, x => x.IsCommandActive, out isCommandActive);
    }

    private async Task CloseCurrentTab()
    {
        _tabService.RemoveTab(CurrentTab.ViewModel.Path ?? CurrentTab.Name);
        ImageTabs = _tabService.GetTabItems();
        TabItem? supposedCurrentTab = _imageTabs.FirstOrDefault();
        
        if (supposedCurrentTab != null)
        {
            CurrentTab = supposedCurrentTab;
        }
        else
        {
            await NewEmptyTab();
        }
    }

    private async Task OpenImageConvolutionWindow() => await ShowWindow<ImageConvolutionViewModel>();

    private async Task OpenArithmeticBitwiseWindow() => await ShowWindow<ArithmeticBitwiseOperationsViewModel>();

    private async Task OpenGammaCorrectionWindow() => await ShowWindow<NonLinearContrastStretchingViewModel>();

    private async Task NewEmptyTab()
    {
        _tabService.AddEmpty(new TabItem(_serviceProvider.GetRequiredService<TabControlViewModel>()));
        ImageTabs = _tabService.GetTabItems();
    }

    private async Task PrepareNewTab()
    {
        IStorageFile file = await _commonDialogService.ShowFileDialogInNewWindow();
        await using Stream fileStream = await file.OpenReadAsync();

        if (fileStream.Length != 0)
        {
            var openedImageBitmap = new Bitmap(fileStream);
            var newTab = new TabItem(Path.GetFileName(file.Path.AbsolutePath),
                _serviceProvider.GetRequiredService<TabControlViewModel>());
            await newTab.ViewModel.LoadImage(openedImageBitmap, file.Path.AbsolutePath);

            CurrentTab = _tabService.Replace(_currentTab.Name, newTab);
            ImageTabs = _tabService.GetTabItems();
        }
    }

    private async Task OpenContrastStretchWindow() =>
        await ShowWindow<ContrastStretchingViewModel>(x => x.HistogramValues = _currentTab.ViewModel.Luminance);

    private async Task OpenHistogramEqualizationWindow() =>
        await ShowWindow<HistogramEqualizationViewModel>(x => x.Lut = _currentTab.ViewModel.ImageValues);

    private async Task NegateImage()
    {
        var bitmap =
            _imagePointOperationsService.Negation(CurrentTab.ViewModel.Image);

        await ReloadImageAndReplaceTab(CurrentTab.ViewModel.Image, bitmap, CurrentTab);
    }

    private async Task OpenThresholdWindow()
    {
        await ShowWindow<ThresholdViewModel>();
    }

    private async Task OpenMultiThresholdWindow()
    {
        await ShowWindow<MultiThresholdViewModel>();
    }

    private async Task SaveImage()
    {
        _currentTab.ViewModel.Image.Save(Path.GetFileName(_currentTab.ViewModel.Path));
    }

    private async Task SaveImageAs()
    {
        await _commonDialogService.ShowSaveFileDialog(_currentTab.ViewModel.Image, _currentTab.ViewModel.Path);
    }

    private async Task Duplicate()
    {
        CurrentTab = _tabService.Duplicate(_currentTab.ViewModel.Path);
    }

    private async Task CloseApp() => Environment.Exit(1);
    
    private async Task ReloadImageAndReplaceTab(Bitmap? before, Bitmap? after, TabItem? tabItem)
    {
        TabControlViewModel newViewModel;
        if (after != null && before != after)
        {
            newViewModel = await tabItem.ViewModel.ResetTab().LoadImage(after, tabItem.ViewModel.Path);
        }
        else
        {
            newViewModel = tabItem.ViewModel;
        }
        
        CurrentTab = _tabService.Replace(tabItem.ViewModel.Path, new TabItem(tabItem.Name, newViewModel));
        ImageTabs = _tabService.GetTabItems();
    }

    private async Task ShowWindow<T>(Action<T>? action = null) where T: ImageOperationDialogViewModelBase
    {
        var service = _serviceProvider.GetRequiredService<T>();

        if (action is not null)
        {
            action(service);
        }
        
        service.BeforeImage = _currentTab.ViewModel.Image;

        await _commonDialogService.ShowDialog(service);
        await ReloadImageAndReplaceTab(service.BeforeImage, service.AfterImage, CurrentTab);
    }
}