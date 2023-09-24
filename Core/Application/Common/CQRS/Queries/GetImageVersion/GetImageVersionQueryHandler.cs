using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Domain.Common.CQRS.Interfaces;

namespace ImageManipulator.Application.Common.CQRS.Queries.GetImageVersion;

public class GetImageVersionQueryHandler : IQueryHandler<GetImageVersionQuery, Bitmap>
{
    private readonly IImageHistoryService _imageHistoryService;

    public GetImageVersionQueryHandler(IImageHistoryService imageHistoryService)
    {
        _imageHistoryService = imageHistoryService;
    }

    public async Task<Bitmap> Handle(GetImageVersionQuery query, CancellationToken cancellationToken)
    {
        return _imageHistoryService.RestoreVersion(query.Path, query.Version);
    }
}