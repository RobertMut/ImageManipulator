namespace ImageManipulator.Application.Common.CQRS.Queries.GetImageAfterHistogramEqualization;

public class GetImageAfterHistogramEqualizationQuery
{
    public int[]?[] LookupTable { get; set; }
}