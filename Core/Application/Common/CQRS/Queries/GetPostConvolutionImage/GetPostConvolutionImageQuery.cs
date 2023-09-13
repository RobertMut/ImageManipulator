using System.Drawing;
using ImageManipulator.Common.Enums;

namespace ImageManipulator.Application.Common.CQRS.Queries.GetPostConvolutionImage;

public class GetPostConvolutionImageQuery
{
    public bool Sobel { get; set; }
    public bool EdgeDetection { get; set; }
    public int Value { get; set; }
    public Color Color { get; set; }
    public SoftenSharpenType SoftenSharpenType { get; set; }
    public SobelType SobelType { get; set; }
    public MatrixSize MatrixSize { get; set; }
    public EdgeDetectionType EdgeDetectionType { get; set; }
    public ImageWrapType ImageWrapType { get; set; }
}