namespace ImageManipulator.Application.Common.CQRS.Queries.GetImageVersion;

public class GetImageVersionQuery
{
    public string Path { get; set; }
    public int Version { get; set; }
}