using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Domain.Common.CQRS.Interfaces;
using ImageManipulator.Domain.Common.Helpers;

namespace ImageManipulator.Application.Common.CQRS.Queries.GetImageAfterContrastStretch;

public class GetImageAfterContrastStretchQueryHandler : IQueryHandler<GetImageAfterContrastStretchQuery, Avalonia.Media.Imaging.Bitmap>
{
    private readonly ITabService _tabService;
    private readonly IImagePointOperationsService _imagePointOperationsService;

    public GetImageAfterContrastStretchQueryHandler(ITabService tabService, IImagePointOperationsService imagePointOperationsService)
    {
        _tabService = tabService;
        _imagePointOperationsService = imagePointOperationsService;
    }
    
    public async Task<Avalonia.Media.Imaging.Bitmap> Handle(GetImageAfterContrastStretchQuery query, CancellationToken cancellationToken)
    {
        var tab = _tabService.GetTab(_tabService.CurrentTabName);
        Bitmap? bitmap = ImageConverterHelper.ConvertFromAvaloniaUIBitmap(tab.ViewModel.Image);

        Bitmap? image = _imagePointOperationsService.StretchContrast(bitmap, query.Min, query.Max);

        return ImageConverterHelper.ConvertFromSystemDrawingBitmap(image);
    }
}