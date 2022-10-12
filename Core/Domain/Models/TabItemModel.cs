using Avalonia.Media.Imaging;

namespace ImageManipulator.Domain.Models;

public class TabItemModel
{
    public string Header { get; }
    public Bitmap Image { get; set; }
    public string Path { get; }
    public Bitmap RGBGraph { get; }
    public Bitmap BrightnessGraph { get; }

    public TabItemModel(string header, string path = null, Bitmap image = null, Bitmap rGBGraph = null, Bitmap brightnessGraph = null)
    {
        Header = header;
        Path = path;
        Image = image;
        RGBGraph = rGBGraph;
        BrightnessGraph = brightnessGraph;
    }
}
