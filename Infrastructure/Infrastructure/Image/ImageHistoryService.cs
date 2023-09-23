using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImageManipulator.Application.Common.Interfaces;

namespace ImageManipulator.Infrastructure.Image;

public class ImageHistoryService : IImageHistoryService
{
    private readonly string _tempLocation;
    private readonly string _sessionGuid;
    private Dictionary<string, int> _history = new();

    public ImageHistoryService()
    {
        _tempLocation = Path.GetTempPath();
        _sessionGuid = Guid.NewGuid().ToString();
    }

    public async Task<IEnumerable<System.Drawing.Image>> GetVersions(string filename)
    {
        var fileNameWithoutPath = Path.GetFileName(filename);
        var files = Directory.GetFiles(_tempLocation).Where(x => x.Contains(_sessionGuid) && x.Contains(fileNameWithoutPath));

        return files.Select(x => System.Drawing.Image.FromFile(x));
    }

    public async Task<Bitmap> StoreCurrentVersionAndGetThumbnail(Bitmap bitmap, string fileName)
    {
        string fileNameWithoutPath = Path.GetFileName(fileName);
        
        _history[fileName] = _history.TryGetValue(fileName, out int value) ? ++value : 1;
        string generatedName = GenerateFileName(fileNameWithoutPath, value);
        
        bitmap.Save($"{_tempLocation}/{generatedName}");

        return GetThumbnail(bitmap);
    }

    public Bitmap RestoreVersion(string fileName, int version)
    {
        if (_history.TryGetValue(fileName, out int value))
        {
            if (version > value || version < 1)
            {
                throw new Exception("Provided version does not exists");
            }

            string fileNameWithoutPath = Path.GetFileName(fileName);

            return new Bitmap($"{_tempLocation}/{GenerateFileName(fileNameWithoutPath, version)}");
        }

        throw new Exception("File does not have any version");
    }

    private string GenerateFileName(string filename, int version) => $"{_sessionGuid}-{version}-{filename}";

    private Bitmap GetThumbnail(Bitmap bitmap)
    {
        float ratioX = bitmap.Width > 300 || bitmap.Height > 300 ? bitmap.Width / bitmap.Width / 3 : bitmap.Width;
        float ratioY = bitmap.Width > 300 || bitmap.Height > 300 ? bitmap.Height / bitmap.Height / 3 : bitmap.Height;
        float ratio = Math.Min(ratioX, ratioY);

        return new Bitmap(bitmap.GetThumbnailImage((int)(bitmap.Width * ratio), (int)(bitmap.Height * ratio), null, IntPtr.Zero));
    }
}