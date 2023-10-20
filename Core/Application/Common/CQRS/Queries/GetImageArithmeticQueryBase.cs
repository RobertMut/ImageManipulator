using System.Drawing;
using ImageManipulator.Application.Common.Enums;
using Color = Avalonia.Media.Color;

namespace ImageManipulator.Application.Common.CQRS.Queries;

public abstract class GetImageArithmeticQueryBase
{
    public double OperationValue { get; set; }
    public Bitmap? OperationImage { get; set; }
    public Color OperationColor { get; set; }
    public ElementaryOperationParameterType ElementaryOperationParameterType { get; set; }
}