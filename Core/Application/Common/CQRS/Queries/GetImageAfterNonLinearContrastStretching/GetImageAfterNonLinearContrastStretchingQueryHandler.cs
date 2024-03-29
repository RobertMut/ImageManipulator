﻿using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Domain.Common.CQRS.Interfaces;

namespace ImageManipulator.Application.Common.CQRS.Queries.GetImageAfterNonLinearContrastStretching;

public class GetImageAfterNonLinearContrastStretchingQueryHandler : GetImageQueryHandlerBase, IQueryHandler<GetImageAfterNonLinearContrastStretchingQuery, Bitmap>
{
    private readonly IImagePointOperationsService _imagePointOperationsService;

    public GetImageAfterNonLinearContrastStretchingQueryHandler(ITabService tabService,
        IImagePointOperationsService imagePointOperationsService) : base(tabService)
    {
        _imagePointOperationsService = imagePointOperationsService;
    }
    
    public async Task<Bitmap> Handle(GetImageAfterNonLinearContrastStretchingQuery query, CancellationToken cancellationToken)
    {
        var bitmap = await GetCurrentlyDisplayedBitmap();
        var result = _imagePointOperationsService.NonLinearlyStretchContrast(bitmap, query.Gamma);
        
        return result;
    }
}