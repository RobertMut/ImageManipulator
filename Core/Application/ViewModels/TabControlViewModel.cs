using ImageManipulator.Application.Common.Interfaces;
using ReactiveUI;
using Splat;
using System.Collections.Generic;
using Color = Avalonia.Media.Color;

namespace ImageManipulator.Application.ViewModels
{
    public partial class TabControlViewModel : ReactiveObject
    {
        private readonly IGraphService graphService;
        private readonly IImageDataService imageDataService;

        public Avalonia.Media.Imaging.Bitmap Image { get; private set; }

        public System.Drawing.Bitmap RGBGraph { get; private set; }
        public System.Drawing.Bitmap LuminanceGraph { get; private set; }

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
            Path = path;
            Image = image;
            PrepareGraph();
        }

        private void PrepareGraph()
        {
            var rgbValuesDictionary = imageDataService.CalculateHistogramForImage(this.Image);
            double[] luminance = imageDataService.CalculateLuminanceFromRGB(rgbValuesDictionary);

            RGBGraph = graphService.DrawGraphFromInput(inputData:
                new Dictionary<Color, double[]>
                {
                    {Color.FromRgb(128, 0, 0),  rgbValuesDictionary["red"]},
                    {Color.FromRgb(0, 255, 0),  rgbValuesDictionary["green"]},
                    {Color.FromRgb(0, 0, 255),  rgbValuesDictionary["blue"]}
                }, 300, 240, 5, 5, 1, 100);
            LuminanceGraph = graphService.DrawGraphFromInput(inputData: new Dictionary<Color, double[]>
            {
                {Color.FromRgb(100, 100, 100), luminance }
            }, 300, 240, 5, 5, 1, 100);
        }
    }
}