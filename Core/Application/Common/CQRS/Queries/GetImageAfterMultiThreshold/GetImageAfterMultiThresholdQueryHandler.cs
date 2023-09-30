using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Domain.Common.CQRS.Interfaces;
using ImageManipulator.Domain.Common.Helpers;

namespace ImageManipulator.Application.Common.CQRS.Queries.GetImageAfterMultiThreshold;

public class GetImageAfterMultiThresholdQueryHandler : GetImageQueryHandlerBase, IQueryHandler<GetImageAfterMultiThresholdQuery, Bitmap>
{
    private readonly IImagePointOperationsService _imagePointOperationsService;

    public GetImageAfterMultiThresholdQueryHandler(ITabService tabService, IImagePointOperationsService imagePointOperationsService) : base(tabService)
    {
        _imagePointOperationsService = imagePointOperationsService;
    }

    public async Task<Bitmap> Handle(GetImageAfterMultiThresholdQuery query, CancellationToken cancellationToken)
    {
        var bitmap = await GetCurrentlyDisplayedBitmap();
        var newBitmap = _imagePointOperationsService.MultiThresholding(bitmap, query.LowerThreshold, query.UpperThreshold,
            query.ReplaceColours);

        return ImageConverterHelper.ConvertFromSystemDrawingBitmap(newBitmap);
    }
}