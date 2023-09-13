using ImageManipulator.Common.Enums;

namespace ImageManipulator.Application.Common.CQRS.Queries.GetImageAfterArithmetic;

public class GetImageAfterArithmeticQuery : GetImageArithmeticQueryBase
{
    public ArithmeticOperationType ArithmeticOperationType { get; set; }
}