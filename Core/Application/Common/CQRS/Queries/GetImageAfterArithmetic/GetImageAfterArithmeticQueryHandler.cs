using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Domain.Common.CQRS.Interfaces;

namespace ImageManipulator.Application.Common.CQRS.Queries.GetImageAfterArithmetic;

public class GetImageAfterArithmeticQueryHandler : GetImageQueryHandlerBase, IQueryHandler<GetImageAfterArithmeticQuery, Bitmap>
{
    private readonly IImageArithmeticService _imageArithmeticService;

    public GetImageAfterArithmeticQueryHandler (ITabService tabService, IImageArithmeticService imageArithmeticService) : base(tabService)
    {
        _imageArithmeticService = imageArithmeticService;
    }
    
    public async Task<Bitmap> Handle(GetImageAfterArithmeticQuery query, CancellationToken cancellationToken)
    {
        var bitmap = await GetCurrentlyDisplayedBitmap();
        object? parameter = ParameterSelector(query);

        var newBitmap =_imageArithmeticService.Execute(bitmap, parameter, query.ArithmeticOperationType);
        
        return newBitmap;
    }
}