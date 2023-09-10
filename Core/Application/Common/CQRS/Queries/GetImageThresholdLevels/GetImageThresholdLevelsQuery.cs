namespace ImageManipulator.Application.Common.CQRS.Queries.GetImageThresholdLevels;

public class GetImageThresholdLevelsQuery
{
    public int[]? HistogramValues { get; set; }
}