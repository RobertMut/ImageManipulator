using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Domain.Common.CQRS.Interfaces;

namespace ImageManipulator.Application.Common.CQRS.Queries.GetImageAfterBitwise;

public class GetImageAfterBitwiseQueryHandler : GetImageQueryHandlerBase, IQueryHandler<GetImageAfterBitwiseQuery, Bitmap>
{
    private readonly IImageBitwiseService _imageBitwiseService;

    public GetImageAfterBitwiseQueryHandler (ITabService tabService, IImageBitwiseService imageBitwiseService) : base(tabService)
    {
        _imageBitwiseService = imageBitwiseService;
    }
    
    public async Task<Bitmap> Handle(GetImageAfterBitwiseQuery query, CancellationToken cancellationToken)
    {
        var bitmap = await GetCurrentlyDisplayedBitmap();
        object? parameter = ParameterSelector(query);

        var newBitmap = _imageBitwiseService.Execute(bitmap, parameter, query.BitwiseOperationType);
        return newBitmap;
    }
}