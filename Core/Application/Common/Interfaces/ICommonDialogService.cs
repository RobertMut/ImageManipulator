using System.Drawing;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;

namespace ImageManipulator.Application.Common.Interfaces
{
    public interface ICommonDialogService
    {
        Task<IStorageFile> ShowFileDialogInNewWindow();

        Task ShowSaveFileDialog(Bitmap? bitmap, string filePath);

        public Task ShowDialog<TViewModel>(TViewModel viewModel)
            where TViewModel : class;

        Task ShowException(string exceptionMessage);
    }
}