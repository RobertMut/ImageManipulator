using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Domain.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
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

    private ObservableCollection<TabItemModel> _tabs;

    private int _tabIndex = 1;
    private TabItemModel _currentTab;

    public RoutingState Router { get; } = new RoutingState();

    public ObservableCollection<TabItemModel> ImageTabs
    {
        get => _tabs;
        set => this.RaiseAndSetIfChanged(ref _tabs, value);
    }

    public TabItemModel CurrentTab
    {
        get => _currentTab;
        set => this.RaiseAndSetIfChanged(ref _currentTab, value);
    }

    #region Commands

    public ReactiveCommand<Unit, Unit> AddNewTab { get; }

    public ReactiveCommand<Unit, Unit> GetImageToTab { get; }
    public ReactiveCommand<Unit, Unit> Exit { get; }

    #endregion Commands

    public MainWindowViewModel(IFileService fileService, ICommonDialogService commonDialogService, IGraphService graphService, IImageDataService imageDataService)
    {
        _fileService = fileService;
        _commonDialogService = commonDialogService;
        _tabs = new ObservableCollection<TabItemModel>();
        this.graphService = graphService;
        this.imageDataService = imageDataService;

        _tabs.Add(new TabItemModel(header: $"New {_tabIndex++}"));

        AddNewTab = ReactiveCommand.Create(NewEmptyTab);
        GetImageToTab = ReactiveCommand.Create(OpenNewImageToTab);
        Exit = ReactiveCommand.Create(CloseApp);
    }

    private void NewEmptyTab()
    {
        ImageTabs.Add(new TabItemModel(header: $"New {_tabIndex++}"));
    }

    private void OpenNewImageToTab()
    {
        string[] filePath = _commonDialogService.ShowFileDialogInNewWindow().Result;
        if (filePath != null)
        {
            var tab = _tabs.Where(x => x.Path == filePath[0]).FirstOrDefault();
            
            if (tab != null) {
                _tabs.Remove(tab);
            }

            if (filePath.Length > 1)
            {
                throw new NotImplementedException();
            }

            int tabIndex = _tabs.IndexOf(_currentTab);
            var openedImageBitmap = new Bitmap(filePath[0]);

            var rgbValuesDictionary = imageDataService.CalculateHistogramForImage(openedImageBitmap);
            double[] luminance = imageDataService.CalculateLuminanceFromRGB(rgbValuesDictionary);

            var graph = graphService.DrawGraphFromInput(inputData:
                new Dictionary<Color, double[]>
                {
                    {Color.Red,  rgbValuesDictionary["red"]},
                    {Color.Green,  rgbValuesDictionary["green"]},
                    {Color.Blue,  rgbValuesDictionary["blue"]}
                }, 300, 250, 20, 20, 1, 250/2, true, "RGB Histogram", "Values");
            var brightnessGraph = graphService.DrawGraphFromInput(inputData: new Dictionary<Color, double[]>
            {
                {Color.Black, luminance }
            }, 300, 200, 20, 20, 1, 100, true, "Luminance Histogram", "Values");

            var tabToReplace = new TabItemModel(Path.GetFileName(filePath[0]), filePath[0], openedImageBitmap, graph, brightnessGraph);

            ImageTabs[tabIndex] = tabToReplace;
            CurrentTab = tabToReplace;
        }
    }
    private void CloseApp()
    {

    }
}