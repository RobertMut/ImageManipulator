using ImageManipulator.Application.Common.Interfaces;
using System.Drawing;
using System.Drawing.Imaging;

namespace ImageManipulator.Infrastructure.IO;

public class FileService : IFileService
{

    public Bitmap Open(string path) => new(path);

    public void Save(Bitmap image, string name) => image.Save(name);

    public void SaveAs(Bitmap image, string name, ImageFormat format) => image.Save(name, format);
}
