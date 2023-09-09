using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Common.Enums;
using ImageManipulator.Common.Interfaces;
using ImageManipulator.Common.Matrices;
using ImageManipulator.Domain.Common.CQRS.Interfaces;
using ImageManipulator.Domain.Common.Helpers;

namespace ImageManipulator.Application.Common.CQRS.Queries.GetPostConvolutionImage;

public class GetPostConvolutionImageCommandHandler : ICommandHandler<GetPostConvolutionImageCommand, Avalonia.Media.Imaging.Bitmap>
{
    private readonly ITabService _tabService;
    private readonly IImageBorderService _imageBorderService;
    private readonly IImageConvolutionService _imageConvolutionService;

    public GetPostConvolutionImageCommandHandler(ITabService tabService, IImageBorderService imageBorderService,
        IImageConvolutionService imageConvolutionService)
    {
        _tabService = tabService;
        _imageBorderService = imageBorderService;
        _imageConvolutionService = imageConvolutionService;
    }

    public async Task<Avalonia.Media.Imaging.Bitmap> Handle(GetPostConvolutionImageCommand command, CancellationToken cancellationToken)
    {
        var tab = _tabService.GetTab(_tabService.CurrentTabName);
        
        Bitmap? bitmap = ImageConverterHelper.ConvertFromAvaloniaUIBitmap(tab.ViewModel.Image);
        Bitmap modifiedBItmap = GetModifiedImage(command, bitmap ?? throw new InvalidOperationException("Bitmap was null"));
        var avaloniaBitmap =
            ImageConverterHelper.ConvertFromSystemDrawingBitmap(BorderAfter(modifiedBItmap, command.ImageWrapType,
                command.Value));
        
        return avaloniaBitmap;
    }
    
    private Bitmap BorderAfter(Bitmap bitmap, ImageWrapEnum imageWrapType, int weight)
    {
        return imageWrapType == ImageWrapEnum.BORDER_AFTER
            ? _imageBorderService.Execute(bitmap, ImageWrapEnum.BORDER_CONSTANT, 5, 5, 5, 5,
                Color.FromArgb(weight, weight, weight))
            : bitmap;
    }
    private Bitmap GetModifiedImage(GetPostConvolutionImageCommand command, Bitmap bitmap)
    {
        if (command.ImageWrapType > 0 && (int)command.ImageWrapType < 4)
        {
            bitmap = _imageBorderService.Execute(bitmap, command.ImageWrapType, 5, 5, 5, 5, command.Color);
        }

        if (!command.EdgeDetection)
        {
            var matrix = GetMatrix(command);

            return command is { SoftenSharpenType: < SoftenSharpenEnum.SharpenLaplace1, Sobel: false }
                ? _imageConvolutionService.Execute(bitmap, matrix, command.Value, true)
                : _imageConvolutionService.Execute(bitmap, matrix, command.Value);
        }

        if (command.EdgeDetectionType != EdgeDetectionEnum.Canny)
        {
            return _imageConvolutionService.Execute(bitmap,
                EdgeDetection.EdgeDetectionMatrices[command.EdgeDetectionType], command.Value);
        }

        bitmap = _imageConvolutionService.Execute(bitmap,
            EdgeDetection.EdgeDetectionMatrices[command.EdgeDetectionType], command.Value, true);
        
        var gradientMagnitude =
            _imageConvolutionService.ComputeGradient(bitmap, (gx, gy) => Math.Sqrt(gx * gx + gy * gy));
        
        return _imageConvolutionService.HysteresisThresholding(bitmap.Width, bitmap.Height, 100, 210,
            gradientMagnitude);
    }

    private double[,] GetMatrix(GetPostConvolutionImageCommand command)
    {
        IConvolutionMatrix matrices = MatrixSizeDictionary[command.MatrixSize]();

        if (command.Sobel)
        {
            return matrices.SobelMatrices[command.SobelType];
        }

        return command.SoftenSharpenType != SoftenSharpenEnum.SoftenAverageWithWeight
            ? matrices.SoftenSharpenMatrices[command.SoftenSharpenType]
            : matrices.SoftenAverageWithWeight(command.Value);
    }

    private static readonly Dictionary<MatrixSize, Func<IConvolutionMatrix>> MatrixSizeDictionary =
        new()
        {
            { MatrixSize.three, () => new ConvolutionMatrices3x3() },
            { MatrixSize.five, () => new ConvolutionMatrices5x5() },
            { MatrixSize.seven, () => new ConvolutionMatrices7x7() },
            { MatrixSize.nine, () => new ConvolutionMatrices9x9() }
        };
}