using System.Collections.Generic;
using ImageManipulator.Domain.Common.Extensions;
using ReactiveUI;
using Splat;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using ImageManipulator.Application.Common.CQRS.Command.AddImageVersion;
using ImageManipulator.Application.Common.CQRS.Queries.GetImageValues;
using ImageManipulator.Application.Common.CQRS.Queries.GetImageVersion;
using ImageManipulator.Application.Common.CQRS.Queries.GetImageVersions;
using ImageManipulator.Domain.Common.CQRS.Interfaces;
using ImageManipulator.Domain.Common.Helpers;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace ImageManipulator.Application.ViewModels
{
    public class TabControlViewModel : ViewModelBase
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ICommandDispatcher _commandDispatcher;
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
        public Bitmap? Image { get => _image; set => this.RaiseAndSetIfChanged(ref _image, value); }
        public string? Path { get; set; }

        /// <inheritdoc cref="IScreen" />
        public IScreen HostScreen { get; }
        
        public ReactiveCommand<int, Unit> GetVersion { get; }

            
        public TabControlViewModel(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher)
        {
            _queryDispatcher = queryDispatcher;
            _commandDispatcher = commandDispatcher;
            HostScreen = Locator.Current.GetService<IScreen>();
            History = new ObservableCollection<Bitmap>();
            
            GetVersion = ReactiveCommand.CreateFromObservable<int, Unit>((version) => 
                Observable.StartAsync(() => GetVersionCommand(version)));
            GetVersion.IsExecuting.ToProperty(this, x => x.IsCommandActive, out isCommandActive);
            
            ClearValues();
        }

        private async Task GetVersionCommand(int version)
        {
            if (version > -1)
            {
                var image = await _queryDispatcher.Dispatch<GetImageVersionQuery, System.Drawing.Bitmap>(new GetImageVersionQuery
                {
                    Path = Path,
                    Version = version
                }, new CancellationToken());
                
                await LoadImage(image, Path);
            }
        }

        public async Task<TabControlViewModel> LoadImage(Bitmap? image, string path)
        {
            Height = (int)Avalonia.Application.Current.GetCurrentWindow().Bounds.Height - 100;
            Path = path;
            Image = image;
            await StoreImage();
            var history = await GetVersionThumbnails();
            History = new ObservableCollection<Bitmap>(history);
            await PrepareGraph();

            return this;
        }

        private async Task<IEnumerable<Bitmap?>> GetVersionThumbnails() =>
            (await _queryDispatcher.Dispatch<GetImageVersionsQuery, IEnumerable<System.Drawing.Bitmap>>(
                new GetImageVersionsQuery
                {
                    Path = Path
                }, new()));

        private async Task StoreImage() =>
            await _commandDispatcher.Dispatch<AddImageVersionCommand, System.Drawing.Bitmap>(new AddImageVersionCommand
            {
                Image = Image,
                Path = Path
            }, new CancellationToken());

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
                Image = Image
            }, new CancellationToken());
            Luminance = (await _queryDispatcher.Dispatch<GetImageValuesQuery, int[][]>(new GetImageValuesQuery
            {
                Luminance = true,
                Image = Image
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