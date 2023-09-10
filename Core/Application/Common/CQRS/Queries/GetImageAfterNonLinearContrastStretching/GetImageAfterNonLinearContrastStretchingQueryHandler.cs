using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using ImageManipulator.Application.Common.CQRS.Queries.GetImageAfterContrastStretch;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Domain.Common.CQRS.Interfaces;
using ImageManipulator.Domain.Common.Helpers;

namespace ImageManipulator.Application.Common.CQRS.Queries.GetImageAfterNonLinearContrastStretching;

public class GetImageAfterNonLinearContrastStretchingQueryHandler : IQueryHandler<GetImageAfterNonLinearContrastStretchingQuery, Avalonia.Media.Imaging.Bitmap>
{
    private readonly ITabService _tabService;
    private readonly IImagePointOperationsService _imagePointOperationsService;

    public GetImageAfterNonLinearContrastStretchingQueryHandler(ITabService tabService,
        IImagePointOperationsService imagePointOperationsService)
    {
        _tabService = tabService;
        _imagePointOperationsService = imagePointOperationsService;
    }
    
    public async Task<Bitmap> Handle(GetImageAfterNonLinearContrastStretchingQuery query, CancellationToken cancellationToken)
    {
        var tab = _tabService.GetTab(_tabService.CurrentTabName);
        var bitmap = ImageConverterHelper.ConvertFromAvaloniaUIBitmap(tab.ViewModel.Image);
        var result =
            ImageConverterHelper.ConvertFromSystemDrawingBitmap(
                _imagePointOperationsService.NonLinearlyStretchContrast(bitmap, query.Gamma));
        
        return result;
    }
}