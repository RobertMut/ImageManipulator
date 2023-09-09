using System.Threading;
using System.Threading.Tasks;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Domain.Common.CQRS.Interfaces;

namespace ImageManipulator.Application.Common.CQRS.Queries.GetImageThreshold;

public class GetImageThresholdQueryHandler : IQueryHandler<GetImageThresholdQuery, Threshold>
{
    private readonly IImagePointOperationsService _imagePointOperationsService;

    public GetImageThresholdQueryHandler(IImagePointOperationsService imagePointOperationsService)
    {
        _imagePointOperationsService = imagePointOperationsService;
    }

    public async Task<Threshold> Handle(GetImageThresholdQuery query, CancellationToken cancellationToken) =>
        new()
        {
            Upper = _imagePointOperationsService.CalculateUpperImageThresholdPoint(query.HistogramValues),
            Lower = _imagePointOperationsService.CalculateLowerImageThresholdPoint(query.HistogramValues)
        };
}