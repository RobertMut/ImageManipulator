using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Domain.Common.Extensions;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;

namespace ImageManipulator.Application.Common.Services;

public class CommonDialogService : ICommonDialogService
{
    public async Task<Stream> ShowFileDialogInNewWindow()
    {
        var image = await new Window().StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select image..",
            AllowMultiple = false,
            FileTypeFilter = new[] { FilePickerFileTypes.ImageAll }
        });
        
        return await image[0].OpenReadAsync();
    }

    public async Task ShowSaveFileDialog(Bitmap bitmap, string filePath)
    {
        var window = new Window();

        var file = await window.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save Image..",
            SuggestedStartLocation = await window.StorageProvider.TryGetFolderFromPathAsync(Path.GetFullPath(filePath)),
            SuggestedFileName = Path.GetFileName(filePath),
            FileTypeChoices = new FilePickerFileType[] { FilePickerFileTypes.ImageAll },
            ShowOverwritePrompt = true
        });
        
        bitmap.Save(file.Path.ToString());
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