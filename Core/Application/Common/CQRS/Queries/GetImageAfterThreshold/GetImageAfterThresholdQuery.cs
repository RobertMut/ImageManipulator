namespace ImageManipulator.Application.Common.CQRS.Queries.GetImageThreshold;

public class GetImageAfterThresholdQuery
{
    public int Threshold { get; set; }
    public bool ReplaceColour { get; set; }
}