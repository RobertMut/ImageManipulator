using System.Threading;
using System.Threading.Tasks;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Domain.Common.CQRS.Interfaces;

namespace ImageManipulator.Application.Common.CQRS.Queries.GetImageThresholdLevels;

public class GetImageThresholdLevelsQueryHandler : IQueryHandler<GetImageThresholdLevelsQuery, ThresholdLevels>
{
    private readonly IImagePointOperationsService _imagePointOperationsService;

    public GetImageThresholdLevelsQueryHandler(IImagePointOperationsService imagePointOperationsService)
    {
        _imagePointOperationsService = imagePointOperationsService;
    }

    public async Task<ThresholdLevels> Handle(GetImageThresholdLevelsQuery levelsQuery, CancellationToken cancellationToken) =>
        new()
        {
            Upper = _imagePointOperationsService.CalculateUpperImageThresholdPoint(levelsQuery.HistogramValues),
            Lower = _imagePointOperationsService.CalculateLowerImageThresholdPoint(levelsQuery.HistogramValues)
        };
}