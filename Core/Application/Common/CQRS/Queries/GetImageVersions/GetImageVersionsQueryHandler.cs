using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Domain.Common.CQRS.Interfaces;

namespace ImageManipulator.Application.Common.CQRS.Queries.GetImageVersions;

public class GetImageVersionsQueryHandler : IQueryHandler<GetImageVersionsQuery, IEnumerable<Bitmap>>
{
    private readonly IImageHistoryService _imageHistoryService;

    public GetImageVersionsQueryHandler(IImageHistoryService imageHistoryService)
    {
        _imageHistoryService = imageHistoryService;
    }

    public async Task<IEnumerable<Bitmap>> Handle(GetImageVersionsQuery query, CancellationToken cancellationToken)
    {
        return (await _imageHistoryService.GetVersions(query.Path)).Select(x => new Bitmap(x));
    }
}