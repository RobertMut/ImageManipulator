using System.IO;
using Avalonia.Media.Imaging;
using System.Threading.Tasks;

namespace ImageManipulator.Application.Common.Interfaces
{
    public interface ICommonDialogService
    {
        Task<Stream> ShowFileDialogInNewWindow();

        Task ShowSaveFileDialog(Bitmap bitmap, string filePath);

        public Task ShowDialog<TViewModel>(TViewModel viewModel)
            where TViewModel : class;
    }
}