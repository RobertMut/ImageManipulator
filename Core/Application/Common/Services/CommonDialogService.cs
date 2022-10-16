using Avalonia.Controls;
using Avalonia.Media.Imaging;
using ImageManipulator.Application.Common.Interfaces;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ImageManipulator.Application.Common.Services;

public class CommonDialogService : ICommonDialogService
{
    public Task<string[]> ShowFileDialogInNewWindow()
    {
        return new OpenFileDialog().ShowAsync(new Window());
    }

    public void ShowSaveFileDialog(Bitmap bitmap, string filePath)
    {
        var saveDialog = new SaveFileDialog();
        saveDialog.InitialFileName = Path.GetFileName(filePath);
        saveDialog.InitialDirectory = Path.GetFullPath(filePath);

        var filter = new FileDialogFilter();
        filter.Extensions.Add(Path.GetExtension(saveDialog.DefaultExtension));

        saveDialog.Filters = new System.Collections.Generic.List<FileDialogFilter>
        {
            filter
        };

        string file = saveDialog.ShowAsync(new Window()).Result;

        bitmap.Save(file);
    }
}