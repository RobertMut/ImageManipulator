using ImageManipulator.Common.Enums;

namespace ImageManipulator.Application.Common.CQRS.Queries.GetImageAfterBitwise;

public class GetImageAfterBitwiseQuery : GetImageArithmeticQueryBase
{
    public BitwiseOperationType BitwiseOperationType { get; set; }
}