using System;
using System.Drawing;
using System.Threading.Tasks;
using ImageManipulator.Application.Common.Enums;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Domain.Common.Helpers;

namespace ImageManipulator.Application.Common.CQRS.Queries;

public abstract class GetImageQueryHandlerBase
{
    protected ITabService tabService;

    protected GetImageQueryHandlerBase(ITabService tabService)
    {
        this.tabService = tabService;
    }

    protected async Task<Bitmap> GetCurrentlyDisplayedBitmap()
    {
        var tab = tabService.GetTab(tabService.CurrentTabName);
        Bitmap? bitmap = ImageConverterHelper.ConvertFromAvaloniaUIBitmap(tab.ViewModel.Image);

        if (bitmap is null)
        {
            throw new NullReferenceException("Actual displayed bitmap does not exist");
        }
        
        return bitmap;
    }
    
    protected object? ParameterSelector(GetImageArithmeticQueryBase query) => query.ElementaryOperationParameterType switch
    {
        ElementaryOperationParameterType.Value => query.OperationValue,
        ElementaryOperationParameterType.Color => query.OperationColor,
        ElementaryOperationParameterType.Image => ImageConverterHelper.ConvertFromAvaloniaUIBitmap(query.OperationImage),
        _ => throw new Exception("Invalid operation")
    };
}