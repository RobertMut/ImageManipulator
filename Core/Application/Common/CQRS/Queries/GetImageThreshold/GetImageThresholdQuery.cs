namespace ImageManipulator.Application.Common.CQRS.Queries.GetImageThreshold;

public class GetImageThresholdQuery
{
    public int[]? HistogramValues { get; set; }
}