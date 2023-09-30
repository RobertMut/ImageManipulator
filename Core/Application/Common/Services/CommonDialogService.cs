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
    public async Task<IStorageFile> ShowFileDialogInNewWindow()
    {
        IReadOnlyList<IStorageFile> storageFiles = await new Window()
        {
            WindowState = WindowState.Maximized
        }.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select image..",
            AllowMultiple = false,
            FileTypeFilter = new[] { FilePickerFileTypes.ImageAll }
        });

        return storageFiles[0];
    }

    public async Task ShowSaveFileDialog(Bitmap? bitmap, string filePath)
    {
        var window = new Window();

        var file = await window.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save Image..",
            SuggestedStartLocation = await window.StorageProvider.TryGetFolderFromPathAsync(Path.GetFullPath(filePath)),
            SuggestedFileName = Path.GetFileName(filePath),
            FileTypeChoices = new[] { FilePickerFileTypes.ImageAll },
            ShowOverwritePrompt = true
        });
        
        bitmap?.Save(file?.Path.ToString());
    }

    public Task ShowDialog<TViewModel>(TViewModel viewModel)
    where TViewModel : class
    {
        var dialog = new Window()
        {
            WindowState = WindowState.Maximized,
            DataContext = viewModel
        }.SetContent(viewModel);

        var task = new TaskCompletionSource<object>();
        
        dialog.Closed += (s, a) => task.SetResult(default);
        dialog.Show();
        dialog.Focus();
        
        return task.Task;
    }
}