using ImageManipulator.Application.Common.Helpers;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Domain.Common.Extensions;
using ImageManipulator.Domain.Models;
using ReactiveUI;
using Splat;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ImageManipulator.Application.ViewModels
{
    public partial class TabControlViewModel : ReactiveObject
    {
        private readonly IGraphService graphService;
        private readonly IImageDataService imageDataService;
        private double[] _luminance;
        public double[][] imageValues;
        private Avalonia.Media.Imaging.Bitmap _image;
        private ObservableCollection<CanvasLineModel> _canvasLinesRGB;
        private ObservableCollection<CanvasLineModel> _canvasLinesLuminance;
        public ObservableCollection<CanvasLineModel> CanvasLinesRGB { get => _canvasLinesRGB; private set => this.RaiseAndSetIfChanged(ref _canvasLinesRGB, value); }
        public ObservableCollection<CanvasLineModel> CanvasLinesLuminance { get => _canvasLinesLuminance; private set => this.RaiseAndSetIfChanged(ref _canvasLinesLuminance, value); }
        public int Height { get; private set; }
        public Avalonia.Media.Imaging.Bitmap Image { get => _image; private set => this.RaiseAndSetIfChanged(ref _image, value); }
        public double[] Luminance { get => _luminance; }
        public string Path { get; private set; }

        /// <inheritdoc/>
        public IScreen HostScreen { get; }

        /// <inheritdoc/>
        public string UrlPathSegment { get; }

        public TabControlViewModel(IGraphService graphService, IImageDataService imageDataService)
        {
            HostScreen = Locator.Current.GetService<IScreen>();
            this.graphService = graphService;
            this.imageDataService = imageDataService;
            CanvasLinesRGB = new ObservableCollection<CanvasLineModel>();
            CanvasLinesLuminance = new ObservableCollection<CanvasLineModel>();
        }

        public TabControlViewModel LoadImage(Avalonia.Media.Imaging.Bitmap image, string path)
        {
            this.Height = (int)Avalonia.Application.Current.GetCurrentWindow().Bounds.Height - 100;
            Path = path;
            Image = image;
            PrepareGraph();

            return this;
        }

        public TabControlViewModel ResetTab()
        {
            CanvasLinesRGB = null;
            CanvasLinesLuminance = null;
            CanvasLinesRGB = new ObservableCollection<CanvasLineModel>();
            CanvasLinesLuminance = new ObservableCollection<CanvasLineModel>();
            Image = null;

            return this;
        }

        private void PrepareGraph()
        {
            var currentImg = ImageConverterHelper.ConvertFromAvaloniaUIBitmap(this.Image);
            var imageValues = imageDataService.CalculateLevels(this.Image);
            var luminance = imageDataService.CalculateLuminanceFromRGB(imageValues);
            var scale = new int[] { currentImg.Height, currentImg.Width }.OrderBy(x => x).ToArray();
            var rGBvaluesDictionary = new Dictionary<string, double[]>
                {
                    {"red", imageValues[0]},
                    {"green", imageValues[1]},
                    {"blue", imageValues[2]}
                };

            var luminanceValuesDictionary = new Dictionary<string, double[]>
            {
                {"gray", luminance}
            };

            var histogramValues = imageDataService.StretchHistogram(rGBvaluesDictionary, currentImg);

            this._luminance = luminance;
            this.imageValues = imageValues;
            CanvasLinesRGB = new ObservableCollection<CanvasLineModel>(graphService.DrawGraphFromInput(inputData: histogramValues
                , 300, 240, 5, 5, 1, (scale[1] / scale[0])*3));
            CanvasLinesLuminance = new ObservableCollection<CanvasLineModel>(graphService.DrawGraphFromInput(inputData: luminanceValuesDictionary
                , 300, 240, 5, 5, 1, 0.03));
        }
    }
}