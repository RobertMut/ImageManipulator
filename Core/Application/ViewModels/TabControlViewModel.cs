using ImageManipulator.Domain.Common.Extensions;
using ReactiveUI;
using Splat;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using ImageManipulator.Application.Common.CQRS.Queries.GetImageValues;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Domain.Common.CQRS.Interfaces;
using ImageManipulator.Domain.Common.Helpers;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace ImageManipulator.Application.ViewModels
{
    public class TabControlViewModel : ReactiveObject
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly IImageHistoryService _imageHistoryService;
        private Bitmap? _image;
        private ObservableCollection<ISeries> _canvasLinesLuminance;
        private ObservableCollection<ISeries> _canvasLinesRgb;
        private ObservableCollection<Bitmap> _history;

        private ObservableCollection<Bitmap> History
        {
            get => _history;
            set => this.RaiseAndSetIfChanged(ref _history, value);
        }

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

        public TabControlViewModel(IQueryDispatcher queryDispatcher, IImageHistoryService imageHistoryService)
        {
            _queryDispatcher = queryDispatcher;
            _imageHistoryService = imageHistoryService;
            HostScreen = Locator.Current.GetService<IScreen>();
            History = new ObservableCollection<Bitmap>();
            ClearValues();
        }

        public async Task<TabControlViewModel> LoadImage(Bitmap? image, string path)
        {
            Height = (int)Avalonia.Application.Current.GetCurrentWindow().Bounds.Height - 100;
            Path = path;
            Image = image;
            _imageHistoryService.StoreCurrentVersionAndGetThumbnail(ImageConverterHelper.ConvertFromAvaloniaUIBitmap(image), path);
            var images = await _imageHistoryService.GetVersions(path);
            var history = images.Select(x => new System.Drawing.Bitmap(x))
                .Select(x => ImageConverterHelper.ConvertFromSystemDrawingBitmap(x));
            History = new ObservableCollection<Bitmap>(history);
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
            ImageValues = await _queryDispatcher.Dispatch<GetImageValuesQuery, int[][]>(new GetImageValuesQuery
            {
                Image = ImageConverterHelper.ConvertFromAvaloniaUIBitmap(Image)
            }, new CancellationToken());
            Luminance = (await _queryDispatcher.Dispatch<GetImageValuesQuery, int[][]>(new GetImageValuesQuery
            {
                Luminance = true,
                Image = ImageConverterHelper.ConvertFromAvaloniaUIBitmap(Image)
            }, new CancellationToken()))[0];

            _canvasLinesLuminance = new ObservableCollection<ISeries>()
            {
                new ColumnSeries<int>()
                {
                    Name = "Luminance",
                    Values = Luminance
                }
            };
            
            _canvasLinesRgb = new ObservableCollection<ISeries>()
            {
                new ColumnSeries<int>()
                {
                    Name = "Red",
                    Values = ImageValues[0]
                },
                new ColumnSeries<int>()
                {
                    Name = "Green",
                    Values = ImageValues[1]
                },
                new ColumnSeries<int>()
                {
                    Name = "Blue",
                    Values = ImageValues[2]
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