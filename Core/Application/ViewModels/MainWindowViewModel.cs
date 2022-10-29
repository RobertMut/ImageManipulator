using Avalonia;
using ImageManipulator.Application.Common.Helpers;
using ImageManipulator.Application.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive;
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
    public ReactiveCommand<Unit, Unit> ContrastStretchingCommand { get; }
    public ReactiveCommand<Unit, Unit> GammaCorrectionCommand { get; }
    public ReactiveCommand<Unit, Unit> HistogramEqualizationCommand { get; }
    public ReactiveCommand<Unit, Unit> TresholdingCommand { get; }
    public ReactiveCommand<Unit, Unit> MultiThresholdingCommand { get; }
    public ReactiveCommand<Unit, Unit> NegationCommand { get; }

    #endregion Commands

    public MainWindowViewModel(IFileService fileService, ICommonDialogService commonDialogService, IServiceProvider serviceProvider, IImagePointOperationsService imagePointOperationsService)
    {
        _fileService = fileService;
        _commonDialogService = commonDialogService;
        this.serviceProvider = serviceProvider;
        this.imagePointOperationsService = imagePointOperationsService;
        var emptyTab = new TabItem(serviceProvider.GetRequiredService<TabControlViewModel>());
        CurrentTab = emptyTab;
        ImageTabs.Add(emptyTab);

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
    }

    private void OpenGammaCorrectionWindow()
    {
        var gammaCorrection = serviceProvider.GetRequiredService<NonLinearContrastStretchingViewModel>();
        gammaCorrection.BeforeImage = _currentTab.ViewModel.Image;

        _commonDialogService.ShowDialog(gammaCorrection).ContinueWith(x =>
        {
            if (gammaCorrection.AfterImage != null && gammaCorrection.BeforeImage != gammaCorrection.AfterImage)
            {
                var tabIndex = ImageTabs.IndexOf(_currentTab);
                _currentTab.ViewModel.ResetTab();
                _currentTab.ViewModel.LoadImage(gammaCorrection.AfterImage, _currentTab.ViewModel.Path);
                ImageTabs[tabIndex] = _currentTab;
            }
        });
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

    private void OpenContrastStretchWindow()
    {
        var contrastStretching = serviceProvider.GetRequiredService<ContrastStretchingViewModel>();
        contrastStretching.histogramValues = _currentTab.ViewModel.Luminance;
        contrastStretching.BeforeImage = _currentTab.ViewModel.Image;

        _commonDialogService.ShowDialog(contrastStretching).ContinueWith(x =>
        {
            if (contrastStretching.AfterImage != null && contrastStretching.BeforeImage != contrastStretching.AfterImage)
            {
                var tabIndex = ImageTabs.IndexOf(CurrentTab);
                CurrentTab.ViewModel.ResetTab();
                CurrentTab.ViewModel.LoadImage(contrastStretching.AfterImage, CurrentTab.ViewModel.Path);
                ImageTabs[tabIndex] = CurrentTab;
            }
        });
    }

    private void OpenHistogramEqualizationWindow()
    {
        var histogramEqualization = serviceProvider.GetRequiredService<HistogramEqualizationViewModel>();
        histogramEqualization.BeforeImage = _currentTab.ViewModel.Image;

        _commonDialogService.ShowDialog(histogramEqualization).ContinueWith(x =>
        {
            if (histogramEqualization.AfterImage != null && histogramEqualization.BeforeImage != histogramEqualization.AfterImage)
            {
                var tabIndex = ImageTabs.IndexOf(_currentTab);
                CurrentTab.ViewModel.ResetTab();
                CurrentTab.ViewModel.LoadImage(histogramEqualization.AfterImage, CurrentTab.ViewModel.Path);
                ImageTabs[tabIndex] = CurrentTab;
            }
        });
    }

    private void NegateImage()
    {
        var bitmap = imagePointOperationsService.Negation(ImageConverterHelper.ConvertFromAvaloniaUIBitmap(CurrentTab.ViewModel.Image));
        int index = ImageTabs.IndexOf(CurrentTab);
        CurrentTab.ViewModel.ResetTab();
        CurrentTab.ViewModel.LoadImage(ImageConverterHelper.ConvertFromSystemDrawingBitmap(bitmap), CurrentTab.ViewModel.Path);
        ImageTabs[index] = CurrentTab;
    }

    private void OpenTresholdingWindow()
    {
        var tresholding = serviceProvider.GetRequiredService<ThresholdingViewModel>();
        tresholding.BeforeImage = _currentTab.ViewModel.Image;

        _commonDialogService.ShowDialog(tresholding).ContinueWith(x =>
        {
            if (tresholding.AfterImage != null && tresholding.BeforeImage != tresholding.AfterImage)
            {
                var tabIndex = ImageTabs.IndexOf(_currentTab);
                CurrentTab.ViewModel.ResetTab();
                CurrentTab.ViewModel.LoadImage(tresholding.AfterImage, CurrentTab.ViewModel.Path);
                ImageTabs[tabIndex] = CurrentTab;
            }
        });
    }

    private void OpenMultiTresholdingWindow()
    {
        var multiThresholding = serviceProvider.GetRequiredService<MultiThresholdingViewModel>();
        multiThresholding.BeforeImage = _currentTab.ViewModel.Image;

        _commonDialogService.ShowDialog(multiThresholding).ContinueWith(x =>
        {
            if (multiThresholding.AfterImage != null && multiThresholding.BeforeImage != multiThresholding.AfterImage)
            {
                var tabIndex = ImageTabs.IndexOf(_currentTab);
                CurrentTab.ViewModel.ResetTab();
                CurrentTab.ViewModel.LoadImage(multiThresholding.AfterImage, CurrentTab.ViewModel.Path);
                ImageTabs[tabIndex] = CurrentTab;
            }
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

    private void CloseApp()
    {
        Environment.Exit(1);
    }
}