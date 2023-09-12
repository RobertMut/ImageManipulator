using System.Threading;
using System.Threading.Tasks;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Domain.Common.CQRS.Interfaces;

namespace ImageManipulator.Application.Common.CQRS.Queries.GetImageValues;

public class GetImageValuesQueryHandler : IQueryHandler<GetImageValuesQuery, int[][]>
{
    private readonly IImageDataService _imageDataService;

    public GetImageValuesQueryHandler(IImageDataService imageDataService)
    {
        _imageDataService = imageDataService;
    }

    public async Task<int[][]> Handle(GetImageValuesQuery query, CancellationToken cancellationToken)
    {
        int[]?[] levels = _imageDataService.CalculateLevels(query.Image);

        if (query.Luminance)
        {
            levels = new[] { _imageDataService.CalculateAverageForGrayGraph(levels) };
        }

        return levels;
    }
}