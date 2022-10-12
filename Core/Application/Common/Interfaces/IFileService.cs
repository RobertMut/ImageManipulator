using System.Drawing;
using System.Drawing.Imaging;

namespace ImageManipulator.Application.Common.Interfaces;

public interface IFileService
{
    Bitmap Open(string path);
    void Save(Bitmap image, string name);
    void SaveAs(Bitmap image, string name, ImageFormat format);
}
