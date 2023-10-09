namespace ImageManipulator.Application.Common.CQRS.Queries.GetImageAfterThreshold;

public class GetImageAfterThresholdQuery
{
    public int Threshold { get; set; }
    public bool ReplaceColour { get; set; }
}