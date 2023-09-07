using ImageManipulator.Application.Common.Helpers;
using ImageManipulator.Application.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Bitmap = Avalonia.Media.Imaging.Bitmap;
using TabItem = ImageManipulator.Application.Common.Models.TabItem;

namespace ImageManipulator.Application.ViewModels;

public class MainWindowViewModel : ViewModelBase, IScreen
{
    private readonly ICommonDialogService _commonDialogService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IImagePointOperationsService _imagePointOperationsService;
    private readonly ITabService _tabService;
    private TabItem _currentTab;
    private ObservableCollection<TabItem> _imageTabs;

    public RoutingState Router { get; } = new();

    public ObservableCollection<TabItem> ImageTabs
    {
        get => _imageTabs;
        private set => this.RaiseAndSetIfChanged(ref _imageTabs, value);
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

        AddNewTab = ReactiveCommand.CreateFromObservable(() => Observable.StartAsync(NewEmptyTab));
        AddNewTab.IsExecuting.ToProperty(this, x => x.IsCommandActive, out isCommandActive);

        GetImageToTab = ReactiveCommand.CreateFromObservable(() => Observable.StartAsync(PrepareNewTab));
        GetImageToTab.IsExecuting.ToProperty(this, x => x.IsCommandActive, out isCommandActive);

        Exit = ReactiveCommand.CreateFromObservable(() => Observable.StartAsync(CloseApp));
        SaveImageCommand = ReactiveCommand.CreateFromObservable(() => Observable.StartAsync(SaveImage));
        SaveImageCommand.IsExecuting.ToProperty(this, x => x.IsCommandActive, out isCommandActive);
        
        SaveImageAsCommand = ReactiveCommand.CreateFromObservable(() => Observable.StartAsync(SaveImageAs));
        SaveImageAsCommand.IsExecuting.ToProperty(this, x => x.IsCommandActive, out isCommandActive);
        
        DuplicateCommand = ReactiveCommand.CreateFromObservable(() => Observable.StartAsync(Duplicate));
        DuplicateCommand.IsExecuting.ToProperty(this, x => x.IsCommandActive, out isCommandActive);

        ContrastStretchingCommand =
            ReactiveCommand.CreateFromObservable(() => Observable.StartAsync(OpenContrastStretchWindow));
        ContrastStretchingCommand.IsExecuting.ToProperty(this, x => x.IsCommandActive, out isCommandActive);

        GammaCorrectionCommand =
            ReactiveCommand.CreateFromObservable(() => Observable.StartAsync(OpenGammaCorrectionWindow));
        GammaCorrectionCommand.IsExecuting.ToProperty(this, x => x.IsCommandActive, out isCommandActive);

        HistogramEqualizationCommand =
            ReactiveCommand.CreateFromObservable(() => Observable.StartAsync(OpenHistogramEqualizationWindow));
        HistogramEqualizationCommand.IsExecuting.ToProperty(this, x => x.IsCommandActive, out isCommandActive);
        
        ThresholdCommand = ReactiveCommand.CreateFromObservable(() => Observable.StartAsync(OpenThresholdWindow));
        ThresholdCommand.IsExecuting.ToProperty(this, x => x.IsCommandActive, out isCommandActive);

        MultiThresholdingCommand =
            ReactiveCommand.CreateFromObservable(() => Observable.StartAsync(OpenMultiThresholdWindow));
        MultiThresholdingCommand.IsExecuting.ToProperty(this, x => x.IsCommandActive, out isCommandActive);

        NegationCommand = ReactiveCommand.CreateFromObservable(() => Observable.StartAsync(NegateImage));
        NegationCommand.IsExecuting.ToProperty(this, x => x.IsCommandActive, out isCommandActive);

        ArithmeticBitwiseCommand =
            ReactiveCommand.CreateFromObservable(() => Observable.StartAsync(OpenArithmeticBitwiseWindow));
        ArithmeticBitwiseCommand.IsExecuting.ToProperty(this, x => x.IsCommandActive, out isCommandActive);

        ImageConvolutionCommand =
            ReactiveCommand.CreateFromObservable(() => Observable.StartAsync(OpenImageConvolutionWindow));
        ImageConvolutionCommand.IsExecuting.ToProperty(this, x => x.IsCommandActive, out isCommandActive);
    }

    private async Task OpenImageConvolutionWindow()
    {
        var convolution = _serviceProvider.GetRequiredService<ImageConvolutionViewModel>();
        convolution.BeforeImage = _currentTab.ViewModel.Image;
        
        await _commonDialogService.ShowDialog(convolution);
        await ReloadImageAndReplaceTab(convolution.BeforeImage, convolution.AfterImage, CurrentTab);
    }

    private async Task OpenArithmeticBitwiseWindow()
    {
        var arithmeticBitwise = _serviceProvider.GetRequiredService<ArithmeticBitwiseOperationsViewModel>();
        arithmeticBitwise.BeforeImage = _currentTab.ViewModel.Image;
        
        await _commonDialogService.ShowDialog(arithmeticBitwise);
        await ReloadImageAndReplaceTab(arithmeticBitwise.BeforeImage, arithmeticBitwise.AfterImage, CurrentTab);
    }

    private async Task OpenGammaCorrectionWindow()
    {
        var gammaCorrection = _serviceProvider.GetRequiredService<NonLinearContrastStretchingViewModel>();
        gammaCorrection.BeforeImage = _currentTab.ViewModel.Image;
        
        await _commonDialogService.ShowDialog(gammaCorrection);
        await ReloadImageAndReplaceTab(gammaCorrection.BeforeImage, gammaCorrection.AfterImage, CurrentTab);
    }

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

    private async Task OpenContrastStretchWindow()
    {
        var contrastStretching = _serviceProvider.GetRequiredService<ContrastStretchingViewModel>();
        contrastStretching.histogramValues = _currentTab.ViewModel.Luminance;
        contrastStretching.BeforeImage = _currentTab.ViewModel.Image;

        await _commonDialogService.ShowDialog(contrastStretching);
        await ReloadImageAndReplaceTab(contrastStretching.BeforeImage, contrastStretching.AfterImage, CurrentTab);
    }

    private async Task OpenHistogramEqualizationWindow()
    {
        var histogramEqualization = _serviceProvider.GetRequiredService<HistogramEqualizationViewModel>();
        histogramEqualization.BeforeImage = _currentTab.ViewModel.Image;
        histogramEqualization.lut = _currentTab.ViewModel.ImageValues;

        await _commonDialogService.ShowDialog(histogramEqualization);
        await ReloadImageAndReplaceTab(histogramEqualization.BeforeImage, histogramEqualization.AfterImage, CurrentTab);
    }

    private async Task NegateImage()
    {
        var bitmap =
            _imagePointOperationsService.Negation(
                ImageConverterHelper.ConvertFromAvaloniaUIBitmap(CurrentTab.ViewModel.Image));

        await ReloadImageAndReplaceTab(CurrentTab.ViewModel.Image, ImageConverterHelper.ConvertFromSystemDrawingBitmap(bitmap), CurrentTab);
    }

    private async Task OpenThresholdWindow()
    {
        var thresholding = _serviceProvider.GetRequiredService<ThresholdingViewModel>();
        thresholding.BeforeImage = _currentTab.ViewModel.Image;

        await _commonDialogService.ShowDialog(thresholding);
        await ReloadImageAndReplaceTab(thresholding.BeforeImage, thresholding.AfterImage, CurrentTab);
    }

    private async Task OpenMultiThresholdWindow()
    {
        var multiThresholding = _serviceProvider.GetRequiredService<MultiThresholdingViewModel>();
        multiThresholding.BeforeImage = _currentTab.ViewModel.Image;

        await _commonDialogService.ShowDialog(multiThresholding);
        await ReloadImageAndReplaceTab(multiThresholding.BeforeImage, multiThresholding.AfterImage, CurrentTab);
    }

    private async Task SaveImage()
    {
        _currentTab.ViewModel.Image.Save(_currentTab.ViewModel.Path);
    }

    private async Task SaveImageAs()
    {
        await _commonDialogService.ShowSaveFileDialog(_currentTab.ViewModel.Image, _currentTab.ViewModel.Path);
    }

    private async Task Duplicate()
    {
        CurrentTab = _tabService.Duplicate(_currentTab.Name);
    }

    private async Task CloseApp() => Environment.Exit(1);
    
    private async Task ReloadImageAndReplaceTab(Bitmap? before, Bitmap? after, TabItem tabItem)
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

        CurrentTab = _tabService.Replace(tabItem.Name, new TabItem(newViewModel));
        ImageTabs = _tabService.GetTabItems();
    }
}