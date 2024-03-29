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

namespace ImageManipulator.Application.Common.CQRS.Queries.GetPostConvolutionImage;

public class GetPostConvolutionImageCommandHandler : GetImageQueryHandlerBase, IQueryHandler<GetPostConvolutionImageQuery, Bitmap>
{
    private readonly IImageBorderService _imageBorderService;
    private readonly IImageConvolutionService _imageConvolutionService;

    public GetPostConvolutionImageCommandHandler(ITabService tabService, IImageBorderService imageBorderService,
        IImageConvolutionService imageConvolutionService) : base(tabService)
    {
        _imageBorderService = imageBorderService;
        _imageConvolutionService = imageConvolutionService;
    }

    public async Task<Bitmap> Handle(GetPostConvolutionImageQuery query, CancellationToken cancellationToken)
    {
        Bitmap bitmap = await GetCurrentlyDisplayedBitmap();
        
        Bitmap modifiedImage = GetModifiedImage(query, bitmap);
        
        return BorderAfter(modifiedImage, query.ImageWrapType,
            query.Value);
    }
    
    private Bitmap BorderAfter(Bitmap bitmap, ImageWrapType imageWrapType, int weight)
    {
        return imageWrapType == ImageWrapType.BORDER_AFTER
            ? _imageBorderService.Execute(bitmap, ImageWrapType.BORDER_CONSTANT, 5, 5, 5, 5,
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

            return query is { SoftenSharpenType: < SoftenSharpenType.Laplace1, Sobel: false }
                ? _imageConvolutionService.Execute(bitmap, matrix, true)
                : _imageConvolutionService.Execute(bitmap, matrix);
        }

        if (query.EdgeDetectionType != EdgeDetectionType.Canny)
        {
            return _imageConvolutionService.Execute(bitmap,
                EdgeDetection.EdgeDetectionMatrices[query.EdgeDetectionType]);
        }

        bitmap = _imageConvolutionService.Execute(bitmap,
            EdgeDetection.EdgeDetectionMatrices[query.EdgeDetectionType], true);
        
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

        return query.SoftenSharpenType != SoftenSharpenType.SoftenAverageWithWeight
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