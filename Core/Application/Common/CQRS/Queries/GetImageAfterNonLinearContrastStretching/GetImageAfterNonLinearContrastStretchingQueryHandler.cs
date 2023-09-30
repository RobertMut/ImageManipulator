using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Domain.Common.CQRS.Interfaces;
using ImageManipulator.Domain.Common.Helpers;

namespace ImageManipulator.Application.Common.CQRS.Queries.GetImageAfterNonLinearContrastStretching;

public class GetImageAfterNonLinearContrastStretchingQueryHandler : GetImageQueryHandlerBase, IQueryHandler<GetImageAfterNonLinearContrastStretchingQuery, Bitmap>
{
    private readonly IImagePointOperationsService _imagePointOperationsService;

    public GetImageAfterNonLinearContrastStretchingQueryHandler(ITabService tabService,
        IImagePointOperationsService imagePointOperationsService) : base(tabService)
    {
        _imagePointOperationsService = imagePointOperationsService;
    }
    
    public async Task<Bitmap> Handle(GetImageAfterNonLinearContrastStretchingQuery query, CancellationToken cancellationToken)
    {
        var bitmap = await GetCurrentlyDisplayedBitmap();
        var result =
            ImageConverterHelper.ConvertFromSystemDrawingBitmap(
                _imagePointOperationsService.NonLinearlyStretchContrast(bitmap, query.Gamma));
        
        return result;
    }
}