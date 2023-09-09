using System.Drawing;
using ImageManipulator.Common.Enums;

namespace ImageManipulator.Application.Common.CQRS.Queries.GetPostConvolutionImage;

public class GetPostConvolutionImageCommand
{
    public bool Sobel { get; set; }
    public bool EdgeDetection { get; set; }
    public int Value { get; set; }
    public Color Color { get; set; }
    public SoftenSharpenEnum SoftenSharpenType { get; set; }
    public SobelEnum SobelType { get; set; }
    public MatrixSize MatrixSize { get; set; }
    public EdgeDetectionEnum EdgeDetectionType { get; set; }
    public ImageWrapEnum ImageWrapType { get; set; }
}