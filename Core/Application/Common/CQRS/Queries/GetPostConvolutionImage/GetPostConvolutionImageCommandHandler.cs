﻿using System;
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

public class GetPostConvolutionImageCommandHandler : IQueryHandler<GetPostConvolutionImageQuery, Avalonia.Media.Imaging.Bitmap>
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

    public async Task<Avalonia.Media.Imaging.Bitmap> Handle(GetPostConvolutionImageQuery query, CancellationToken cancellationToken)
    {
        var tab = _tabService.GetTab(_tabService.CurrentTabName);
        
        Bitmap? bitmap = ImageConverterHelper.ConvertFromAvaloniaUIBitmap(tab.ViewModel.Image);
        Bitmap modifiedBItmap = GetModifiedImage(query, bitmap ?? throw new InvalidOperationException("Bitmap was null"));
        var avaloniaBitmap =
            ImageConverterHelper.ConvertFromSystemDrawingBitmap(BorderAfter(modifiedBItmap, query.ImageWrapType,
                query.Value));
        
        return avaloniaBitmap;
    }
    
    private Bitmap BorderAfter(Bitmap bitmap, ImageWrapEnum imageWrapType, int weight)
    {
        return imageWrapType == ImageWrapEnum.BORDER_AFTER
            ? _imageBorderService.Execute(bitmap, ImageWrapEnum.BORDER_CONSTANT, 5, 5, 5, 5,
                Color.FromArgb(weight, weight, weight))
            : bitmap;
    }
    private Bitmap GetModifiedImage(GetPostConvolutionImageQuery query, Bitmap bitmap)
    {
        if (query.ImageWrapType > 0 && (int)query.ImageWrapType < 4)
        {
            bitmap = _imageBorderService.Execute(bitmap, query.ImageWrapType, 5, 5, 5, 5, query.Color);
        }

        if (!query.EdgeDetection)
        {
            var matrix = GetMatrix(query);

            return query is { SoftenSharpenType: < SoftenSharpenEnum.SharpenLaplace1, Sobel: false }
                ? _imageConvolutionService.Execute(bitmap, matrix, query.Value, true)
                : _imageConvolutionService.Execute(bitmap, matrix, query.Value);
        }

        if (query.EdgeDetectionType != EdgeDetectionEnum.Canny)
        {
            return _imageConvolutionService.Execute(bitmap,
                EdgeDetection.EdgeDetectionMatrices[query.EdgeDetectionType], query.Value);
        }

        bitmap = _imageConvolutionService.Execute(bitmap,
            EdgeDetection.EdgeDetectionMatrices[query.EdgeDetectionType], query.Value, true);
        
        var gradientMagnitude =
            _imageConvolutionService.ComputeGradient(bitmap, (gx, gy) => Math.Sqrt(gx * gx + gy * gy));
        
        return _imageConvolutionService.HysteresisThresholding(bitmap.Width, bitmap.Height, 100, 210,
            gradientMagnitude);
    }

    private double[,] GetMatrix(GetPostConvolutionImageQuery query)
    {
        IConvolutionMatrix matrices = MatrixSizeDictionary[query.MatrixSize]();

        if (query.Sobel)
        {
            return matrices.SobelMatrices[query.SobelType];
        }

        return query.SoftenSharpenType != SoftenSharpenEnum.SoftenAverageWithWeight
            ? matrices.SoftenSharpenMatrices[query.SoftenSharpenType]
            : matrices.SoftenAverageWithWeight(query.Value);
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