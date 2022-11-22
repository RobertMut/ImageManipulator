using ImageManipulator.Application.Common.Helpers;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Domain.Common.Extensions;
using ImageManipulator.Domain.Models;
using ReactiveUI;
using Splat;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

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

        public async Task<TabControlViewModel> LoadImage(Avalonia.Media.Imaging.Bitmap image, string path)
        {
            this.Height = (int)Avalonia.Application.Current.GetCurrentWindow().Bounds.Height - 100;
            Path = path;
            Image = image;
            await PrepareGraph();

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

        private async Task PrepareGraph()
        {
            this.imageValues = imageDataService.CalculateLevels(this.Image);
            this._luminance = imageDataService.CalculateAverageForGrayGraph(imageValues);
            var scale = new int[] { this.Image.PixelSize.Height, this.Image.PixelSize.Width }.OrderBy(x => x).ToArray();
            var histogramValues = imageDataService.StretchHistogram(this.imageValues, ImageConverterHelper.ConvertFromAvaloniaUIBitmap(this.Image));
            var grayScaleValues = imageDataService.StretchHistogram(new[] { this._luminance }, ImageConverterHelper.ConvertFromAvaloniaUIBitmap(this.Image));

            var rGBvaluesDictionary = new Dictionary<string, double[]>
                {
                    {"red", histogramValues[0]},
                    {"green", histogramValues[1]},
                    {"blue", histogramValues[2]}
                };
            var grayScaleValuesDictionary = new Dictionary<string, double[]>
            {
                {"gray", grayScaleValues[0]}
            };

            CanvasLinesRGB = new ObservableCollection<CanvasLineModel>(graphService.DrawGraphFromInput(inputData: rGBvaluesDictionary
                , 300, 240, 5, 5, 1));
            CanvasLinesLuminance = new ObservableCollection<CanvasLineModel>(graphService.DrawGraphFromInput(inputData: grayScaleValuesDictionary
                , 300, 240, 5, 5, 1));
        }
    }
}