using Avalonia.Media;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Domain.Common.Extensions;
using ReactiveUI;
using Splat;
using System.Collections.Generic;
using System.Globalization;
using Color = Avalonia.Media.Color;

namespace ImageManipulator.Application.ViewModels
{
    public partial class TabControlViewModel : ReactiveObject
    {
        private readonly IGraphService graphService;
        private readonly IImageDataService imageDataService;
        private double[] _luminance;
        private Avalonia.Media.Imaging.Bitmap _image;
        private System.Drawing.Bitmap _rgbGraph;
        private System.Drawing.Bitmap _luminanceGraph;


        public int Height { get; private set; }
        public Avalonia.Media.Imaging.Bitmap Image { get => _image; private set => this.RaiseAndSetIfChanged(ref _image, value); }
        public double[] Luminance { get => _luminance; } 
        public System.Drawing.Bitmap RGBGraph { get => _rgbGraph; private set => this.RaiseAndSetIfChanged(ref _rgbGraph, value); }
        public System.Drawing.Bitmap LuminanceGraph { get => _luminanceGraph; private set => this.RaiseAndSetIfChanged(ref _luminanceGraph, value); }
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
        }

        public void LoadImage(Avalonia.Media.Imaging.Bitmap image, string path)
        {
            this.Height = (int)Avalonia.Application.Current.GetCurrentWindow().Bounds.Height-100;
            Path = path;
            Image = image;
            PrepareGraph();
        }

        public void ResetTab()
        {
            Image = null;
            RGBGraph = null;
            LuminanceGraph = null;
        }

        private void PrepareGraph()
        {
            var rgbValuesDictionary = imageDataService.CalculateHistogramForImage(this.Image);
            _luminance = imageDataService.CalculateLuminanceFromRGB(rgbValuesDictionary);

            RGBGraph = graphService.DrawGraphFromInput(inputData:
                new Dictionary<Color, double[]>
                {
                    {Color.FromRgb(128, 0, 0),  rgbValuesDictionary["red"]},
                    {Color.FromRgb(0, 255, 0),  rgbValuesDictionary["green"]},
                    {Color.FromRgb(0, 0, 255),  rgbValuesDictionary["blue"]}
                }, 300, 240, 5, 5, 1, 100);
            LuminanceGraph = graphService.DrawGraphFromInput(inputData: new Dictionary<Color, double[]>
            {
                {Color.FromRgb(100, 100, 100), _luminance }
            }, 300, 240, 5, 5, 1, 100);
        }


    }
}