using System.Drawing;

namespace ImageManipulator.Application.Common.CQRS.Queries.GetImageValues;

public class GetImageValuesQuery
{
    public bool Luminance { get; set; }
    public Bitmap Image { get; set; }
}