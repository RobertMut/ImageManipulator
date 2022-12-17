using Avalonia;
using ImageManipulator.Application.Common.Helpers;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Bitmap = Avalonia.Media.Imaging.Bitmap;
using TabItem = ImageManipulator.Application.Common.Models.TabItem;

namespace ImageManipulator.Application.ViewModels;

public class MainWindowViewModel : ViewModelBase, IScreen
{
    private readonly IFileService _fileService;
    private readonly ICommonDialogService _commonDialogService;
    private readonly IServiceProvider serviceProvider;
    private readonly IImagePointOperationsService imagePointOperationsService;
    private ObservableCollection<TabItem> _tabs = new ObservableCollection<TabItem>();
    private TabItem _currentTab;
    private int _currentTabIndex = 0;

    public RoutingState Router { get; } = new RoutingState();

    public ObservableCollection<TabItem> ImageTabs
    {
        get => _tabs;
        set => this.RaiseAndSetIfChanged(ref _tabs, value);
    }

    public TabItem CurrentTab
    {
        get => _currentTab;
        set
        {
            _currentTabIndex = ImageTabs.IndexOf(value);
            this.RaiseAndSetIfChanged(ref _currentTab, value);
        }
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
    public ReactiveCommand<Unit, Unit> TresholdingCommand { get; }
    public ReactiveCommand<Unit, Unit> MultiThresholdingCommand { get; }
    public ReactiveCommand<Unit, Unit> NegationCommand { get; }
    public ReactiveCommand<Unit, Unit> ArithmeticBitwiseCommand { get; }
    public ReactiveCommand<Unit, Unit> ImageConvolutionCommand { get; }

    #endregion Commands

    public MainWindowViewModel(IFileService fileService, ICommonDialogService commonDialogService, IServiceProvider serviceProvider, IImagePointOperationsService imagePointOperationsService)
    {
        _fileService = fileService;
        _commonDialogService = commonDialogService;
        this.serviceProvider = serviceProvider;
        this.imagePointOperationsService = imagePointOperationsService;
        var emptyTab = new TabItem(serviceProvider.GetRequiredService<TabControlViewModel>());
        ImageTabs.Add(emptyTab);
        CurrentTab = emptyTab;

        AddNewTab = ReactiveCommand.Create(NewEmptyTab);
        GetImageToTab = ReactiveCommand.Create(PrepareNewTab);

        Exit = ReactiveCommand.Create(CloseApp);
        SaveImageCommand = ReactiveCommand.Create(SaveImage);
        SaveImageAsCommand = ReactiveCommand.Create(SaveImageAs);
        DuplicateCommand = ReactiveCommand.Create(Duplicate);
        ContrastStretchingCommand = ReactiveCommand.Create(OpenContrastStretchWindow);
        GammaCorrectionCommand = ReactiveCommand.Create(OpenGammaCorrectionWindow);
        HistogramEqualizationCommand = ReactiveCommand.Create(OpenHistogramEqualizationWindow);
        TresholdingCommand = ReactiveCommand.Create(OpenTresholdingWindow);
        MultiThresholdingCommand = ReactiveCommand.Create(OpenMultiTresholdingWindow);
        NegationCommand = ReactiveCommand.Create(NegateImage);
        ArithmeticBitwiseCommand = ReactiveCommand.Create(OpenArithmeticBitwiseWindow);
        ImageConvolutionCommand = ReactiveCommand.Create(OpenImageConvolutionWindow);
    }

    private void OpenImageConvolutionWindow()
    {
        var convolution = serviceProvider.GetRequiredService<ImageConvolutionViewModel>();
        convolution.BeforeImage = _currentTab.ViewModel.Image;

        _commonDialogService.ShowDialog(convolution).ContinueWith(x =>
        {
            ResetTabAndReloadImage(convolution.BeforeImage, convolution.AfterImage, CurrentTab.ViewModel);
            ReplaceTab(CurrentTab, _currentTabIndex);
        });
    }

    private void OpenArithmeticBitwiseWindow()
    {
        var arithmeticBitwise = serviceProvider.GetRequiredService<ArithmeticBitwiseOperationsViewModel>();
        arithmeticBitwise.BeforeImage = _currentTab.ViewModel.Image;

        _commonDialogService.ShowDialog(arithmeticBitwise).ContinueWith(x =>
        {
            ResetTabAndReloadImage(arithmeticBitwise.BeforeImage, arithmeticBitwise.AfterImage, CurrentTab.ViewModel);
            ReplaceTab(CurrentTab, _currentTabIndex);
        });
    }

    private void OpenGammaCorrectionWindow()
    {
        var gammaCorrection = serviceProvider.GetRequiredService<NonLinearContrastStretchingViewModel>();
        gammaCorrection.BeforeImage = _currentTab.ViewModel.Image;

        _commonDialogService.ShowDialog(gammaCorrection).ContinueWith(x =>
        {
            ResetTabAndReloadImage(gammaCorrection.BeforeImage, gammaCorrection.AfterImage, CurrentTab.ViewModel);
            ReplaceTab(CurrentTab, _currentTabIndex);
        });
    }

    private void NewEmptyTab() => _tabs.Add(new TabItem(serviceProvider.GetRequiredService<TabControlViewModel>()));

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

    private void OpenContrastStretchWindow()
    {
        var contrastStretching = serviceProvider.GetRequiredService<ContrastStretchingViewModel>();
        contrastStretching.histogramValues = _currentTab.ViewModel.Luminance;
        contrastStretching.BeforeImage = _currentTab.ViewModel.Image;

        _commonDialogService.ShowDialog(contrastStretching).ContinueWith(x =>
        {
            ResetTabAndReloadImage(contrastStretching.BeforeImage, contrastStretching.AfterImage, CurrentTab.ViewModel);
            ReplaceTab(CurrentTab, _currentTabIndex);
        });
    }

    private void OpenHistogramEqualizationWindow()
    {
        var histogramEqualization = serviceProvider.GetRequiredService<HistogramEqualizationViewModel>();
        histogramEqualization.BeforeImage = _currentTab.ViewModel.Image;
        histogramEqualization.lut = _currentTab.ViewModel.imageValues;

        _commonDialogService.ShowDialog(histogramEqualization).ContinueWith(x =>
        {
            ResetTabAndReloadImage(histogramEqualization.BeforeImage, histogramEqualization.AfterImage, CurrentTab.ViewModel);
            ReplaceTab(CurrentTab, _currentTabIndex);
        });
    }

    private void NegateImage()
    {
        var bitmap = imagePointOperationsService.Negation(ImageConverterHelper.ConvertFromAvaloniaUIBitmap(CurrentTab.ViewModel.Image));
        ResetTabAndReloadImage(CurrentTab.ViewModel.Image, ImageConverterHelper.ConvertFromSystemDrawingBitmap(bitmap), CurrentTab.ViewModel);
        ReplaceTab(CurrentTab, _currentTabIndex);
    }

    private void OpenTresholdingWindow()
    {
        var thresholding = serviceProvider.GetRequiredService<ThresholdingViewModel>();
        thresholding.BeforeImage = _currentTab.ViewModel.Image;

        _commonDialogService.ShowDialog(thresholding).ContinueWith(x =>
        {
            ResetTabAndReloadImage(thresholding.BeforeImage, thresholding.AfterImage, CurrentTab.ViewModel);
            ReplaceTab(CurrentTab, _currentTabIndex);
        });
    }

    private void OpenMultiTresholdingWindow()
    {
        var multiThresholding = serviceProvider.GetRequiredService<MultiThresholdingViewModel>();
        multiThresholding.BeforeImage = _currentTab.ViewModel.Image;

        _commonDialogService.ShowDialog(multiThresholding).ContinueWith(x =>
        {
            ResetTabAndReloadImage(multiThresholding.BeforeImage, multiThresholding.AfterImage, CurrentTab.ViewModel);
            ReplaceTab(CurrentTab, _currentTabIndex);
        });
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

    private void CloseApp() => Environment.Exit(1);

    private Func<TabItem, int, TabItem> ReplaceTab => (tabItem, currentTabIndex) =>
        ImageTabs.ReplaceAndReturn(currentTabIndex,
                                          tabItem);

    private Func<Bitmap, Bitmap, TabControlViewModel, Task<TabControlViewModel>> ResetTabAndReloadImage => async (beforeImage, afterImage, currentTab) =>
    afterImage != null && beforeImage != afterImage ?
        await currentTab.ResetTab().LoadImage(afterImage, currentTab.Path) :
        currentTab;
}