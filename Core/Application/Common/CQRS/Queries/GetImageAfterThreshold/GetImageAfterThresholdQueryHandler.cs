using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using ImageManipulator.Application.Common.CQRS.Queries.GetImageThreshold;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Domain.Common.CQRS.Interfaces;
using ImageManipulator.Domain.Common.Helpers;

namespace ImageManipulator.Application.Common.CQRS.Queries.GetImageAfterThreshold;

public class GetImageAfterThresholdQueryHandler : GetImageQueryHandlerBase, IQueryHandler<GetImageAfterThresholdQuery, Avalonia.Media.Imaging.Bitmap>
{
    private readonly IImagePointOperationsService _imagePointOperationsService;

    public GetImageAfterThresholdQueryHandler(ITabService tabService, IImagePointOperationsService imagePointOperationsService) : base(tabService)
    {
        _imagePointOperationsService = imagePointOperationsService;
    }

    public async Task<Bitmap> Handle(GetImageAfterThresholdQuery query, CancellationToken cancellationToken)
    {
        var bitmap = await GetCurrentlyDisplayedBitmap();
        var result = _imagePointOperationsService.Thresholding(bitmap, query.Threshold, query.ReplaceColour);
        
        return ImageConverterHelper.ConvertFromSystemDrawingBitmap(result);
    }
}