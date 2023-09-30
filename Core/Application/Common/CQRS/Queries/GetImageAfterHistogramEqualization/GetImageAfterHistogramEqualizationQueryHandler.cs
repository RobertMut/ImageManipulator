﻿using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Domain.Common.CQRS.Interfaces;
using ImageManipulator.Domain.Common.Helpers;

namespace ImageManipulator.Application.Common.CQRS.Queries.GetImageAfterHistogramEqualization;

public class GetImageAfterHistogramEqualizationQueryHandler : GetImageQueryHandlerBase, IQueryHandler<GetImageAfterHistogramEqualizationQuery, Bitmap>
{
    private readonly IImagePointOperationsService _imagePointOperationsService;

    public GetImageAfterHistogramEqualizationQueryHandler(ITabService tabService,
        IImagePointOperationsService imagePointOperationsService) : base(tabService)
    {
        _imagePointOperationsService = imagePointOperationsService;
    }

    public async Task<Bitmap> Handle(GetImageAfterHistogramEqualizationQuery query, CancellationToken cancellationToken)
    {
        var currentBitmap = await GetCurrentlyDisplayedBitmap();
        var result = _imagePointOperationsService.HistogramEqualization(currentBitmap, query.LookupTable);
        
        return ImageConverterHelper.ConvertFromSystemDrawingBitmap(result);
    }
}