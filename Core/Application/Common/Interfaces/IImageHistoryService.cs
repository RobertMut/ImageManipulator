using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace ImageManipulator.Application.Common.Interfaces;

public interface IImageHistoryService
{
    Task<IEnumerable<Image>> GetVersions(string filename);
    Task<Bitmap> StoreCurrentVersionAndGetThumbnail(Bitmap bitmap, string fileName);
    Bitmap RestoreVersion(string fileName, int version);
}