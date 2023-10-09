using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Domain.Common.CQRS.Interfaces;
using ImageManipulator.Domain.Common.Helpers;

namespace ImageManipulator.Application.Common.CQRS.Queries.GetImageAfterArithmetic;

public class GetImageAfterArithmeticQueryHandlerHandler : GetImageQueryHandlerBase, IQueryHandler<GetImageAfterArithmeticQuery, Bitmap>
{
    private readonly IImageArithmeticService _imageArithmeticService;

    public GetImageAfterArithmeticQueryHandlerHandler (ITabService tabService, IImageArithmeticService imageArithmeticService) : base(tabService)
    {
        _imageArithmeticService = imageArithmeticService;
    }
    
    public async Task<Bitmap> Handle(GetImageAfterArithmeticQuery query, CancellationToken cancellationToken)
    {
        var bitmap = await GetCurrentlyDisplayedBitmap();
        object? parameter = ParameterSelector(query);

        var newBitmap =
            ImageConverterHelper.ConvertFromSystemDrawingBitmap(
                _imageArithmeticService.Execute(bitmap, parameter, query.ArithmeticOperationType));
        return newBitmap;
    }
}