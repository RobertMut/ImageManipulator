using System.Drawing;

namespace ImageManipulator.Application.Common.CQRS.Command.AddImageVersion;

public class AddImageVersionCommand
{
    public Bitmap Image { get; set; }
    public string Path { get; set; }
}