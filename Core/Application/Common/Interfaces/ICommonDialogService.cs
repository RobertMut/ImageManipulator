using Avalonia.Media.Imaging;
using System.Threading.Tasks;

namespace ImageManipulator.Application.Common.Interfaces
{
    public interface ICommonDialogService
    {
        Task<string[]> ShowFileDialogInNewWindow();

        void ShowSaveFileDialog(Bitmap bitmap, string filePath);

        public Task ShowDialog<TViewModel>(TViewModel viewModel)
            where TViewModel : class;
    }
}