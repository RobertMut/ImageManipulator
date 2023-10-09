using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Domain.Common.CQRS.Interfaces;
using ImageManipulator.Domain.Common.Helpers;

namespace ImageManipulator.Application.Common.CQRS.Queries.GetImageAfterBitwise;

public class GetImageAfterBitwiseQueryHandlerHandler : GetImageQueryHandlerBase, IQueryHandler<GetImageAfterBitwiseQuery, Bitmap>
{
    private readonly IImageBitwiseService _imageBitwiseService;

    public GetImageAfterBitwiseQueryHandlerHandler (ITabService tabService, IImageBitwiseService imageBitwiseService) : base(tabService)
    {
        _imageBitwiseService = imageBitwiseService;
    }
    
    public async Task<Bitmap> Handle(GetImageAfterBitwiseQuery query, CancellationToken cancellationToken)
    {
        var bitmap = await GetCurrentlyDisplayedBitmap();
        object? parameter = ParameterSelector(query);

        var newBitmap =
            ImageConverterHelper.ConvertFromSystemDrawingBitmap(
                _imageBitwiseService.Execute(bitmap, parameter, query.BitwiseOperationType));
        return newBitmap;
    }
}