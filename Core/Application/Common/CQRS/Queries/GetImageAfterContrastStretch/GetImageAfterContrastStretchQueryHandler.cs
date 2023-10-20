using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Domain.Common.CQRS.Interfaces;

namespace ImageManipulator.Application.Common.CQRS.Queries.GetImageAfterContrastStretch;

public class GetImageAfterContrastStretchQueryHandler : GetImageQueryHandlerBase, IQueryHandler<GetImageAfterContrastStretchQuery, Bitmap>
{
    private readonly IImagePointOperationsService _imagePointOperationsService;

    public GetImageAfterContrastStretchQueryHandler(ITabService tabService, IImagePointOperationsService imagePointOperationsService) : base(tabService)
    {
        _imagePointOperationsService = imagePointOperationsService;
    }
    
    public async Task<Bitmap> Handle(GetImageAfterContrastStretchQuery query, CancellationToken cancellationToken)
    {
        var currentBitmap = await GetCurrentlyDisplayedBitmap();
        Bitmap? image = _imagePointOperationsService.StretchContrast(currentBitmap, query.Min, query.Max);

        return image;
    }
}