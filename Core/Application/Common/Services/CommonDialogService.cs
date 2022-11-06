using Avalonia.Controls;
using Avalonia.Media.Imaging;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Application.ViewModels;
using ImageManipulator.Domain.Common.Extensions;
using System.IO;
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

    public Task ShowDialog<TViewModel>(TViewModel viewModel)
    where TViewModel : class
    {
        var dialog = new Window().SetContent(viewModel);
        dialog.DataContext = viewModel;
        var task = new TaskCompletionSource<object>();
        dialog.Closed += (s, a) => task.SetResult(null);
        dialog.Show();
        dialog.Focus();
        return task.Task;
    }
}