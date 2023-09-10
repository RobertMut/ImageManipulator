using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Domain.Common.CQRS.Interfaces;
using ImageManipulator.Domain.Common.Helpers;

namespace ImageManipulator.Application.Common.CQRS.Queries.GetImageAfterHistogramEqualization;

public class GetImageAfterHistogramEqualizationQueryHandler : IQueryHandler<GetImageAfterHistogramEqualizationQuery, Avalonia.Media.Imaging.Bitmap>
{
    private readonly ITabService _tabService;
    private readonly IImagePointOperationsService _imagePointOperationsService;

    public GetImageAfterHistogramEqualizationQueryHandler(ITabService tabService, IImagePointOperationsService imagePointOperationsService)
    {
        _tabService = tabService;
        _imagePointOperationsService = imagePointOperationsService;
    }

    public async Task<Bitmap> Handle(GetImageAfterHistogramEqualizationQuery query, CancellationToken cancellationToken)
    {
        var tab = _tabService.GetTab(_tabService.CurrentTabName);
        var result = _imagePointOperationsService.HistogramEqualization(
            ImageConverterHelper.ConvertFromAvaloniaUIBitmap(tab.ViewModel.Image), query.LookupTable);
        
        return ImageConverterHelper.ConvertFromSystemDrawingBitmap(result);
    }
}