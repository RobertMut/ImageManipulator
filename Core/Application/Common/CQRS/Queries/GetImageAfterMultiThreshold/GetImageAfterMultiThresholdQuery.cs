namespace ImageManipulator.Application.Common.CQRS.Queries.GetImageAfterMultiThreshold;

public class GetImageAfterMultiThresholdQuery
{
    public int LowerThreshold { get; set; }
    public int UpperThreshold { get; set; }
    public bool ReplaceColours { get; set; }
}