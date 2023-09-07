using ImageManipulator.Application.Common.Helpers;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Domain.Common.Extensions;
using ReactiveUI;
using Splat;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace ImageManipulator.Application.ViewModels
{
    public class TabControlViewModel : ReactiveObject
    {
        private readonly IImageDataService _imageDataService;
        private Bitmap? _image;
        private ObservableCollection<ISeries> _canvasLinesLuminance;
        private ObservableCollection<ISeries> _canvasLinesRgb;
        
        public int[]? Luminance { get; set; }

        public int[]?[] ImageValues { get; set; }
        
        public ObservableCollection<ISeries> CanvasLinesRgb
        {
            get { return _canvasLinesRgb; }
            set => this.RaiseAndSetIfChanged(ref _canvasLinesRgb, value);
        }

        public ObservableCollection<ISeries> CanvasLinesLuminance
        {
            get { return _canvasLinesLuminance; }
            set => this.RaiseAndSetIfChanged(ref _canvasLinesLuminance, value);
        }

        public int Height { get; private set; }
        public Bitmap? Image { get => _image; private set => this.RaiseAndSetIfChanged(ref _image, value); }
        public string Path { get; private set; }

        /// <inheritdoc cref="IScreen" />
        public IScreen HostScreen { get; }

        public TabControlViewModel(IImageDataService imageDataService)
        {
            HostScreen = Locator.Current.GetService<IScreen>();
            this._imageDataService = imageDataService;
            ClearValues();
        }

        public async Task<TabControlViewModel> LoadImage(Bitmap? image, string path)
        {
            Height = (int)Avalonia.Application.Current.GetCurrentWindow().Bounds.Height - 100;
            Path = path;
            Image = image;
            await PrepareGraph();

            return this;
        }

        public TabControlViewModel ResetTab()
        {
            ClearValues();
            Image = null;

            return this;
        }

        private async Task PrepareGraph()
        {
            ImageValues = _imageDataService.CalculateLevels(Image);
            Luminance = _imageDataService.CalculateAverageForGrayGraph(ImageValues);
            
            var histogramValues = _imageDataService.GetHistogramValues(ImageValues, ImageConverterHelper.ConvertFromAvaloniaUIBitmap(Image));
            var grayScaleValues = _imageDataService.GetHistogramValues(new[] { Luminance }, ImageConverterHelper.ConvertFromAvaloniaUIBitmap(Image));
            _canvasLinesRgb = new ObservableCollection<ISeries>
            {
                new ColumnSeries<int>
                {
                    Name = "Red",
                    Values = histogramValues[0]
                },
                new ColumnSeries<int>
                {
                    Name = "Green",
                    Values = histogramValues[1]
                },
                new ColumnSeries<int>
                {
                    Name = "Blue",
                    Values = histogramValues[2]
                }
            };

            _canvasLinesLuminance = new ObservableCollection<ISeries>
            {
                new ColumnSeries<int>
                {
                    Values = grayScaleValues[0],
                    Name = "Luminance"
                }
            };
        }

        private void ClearValues()
        {
            _canvasLinesRgb = new ObservableCollection<ISeries>();
            _canvasLinesLuminance = new ObservableCollection<ISeries>();
        }
    }
}