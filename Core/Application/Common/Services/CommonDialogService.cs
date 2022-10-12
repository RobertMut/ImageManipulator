using Avalonia.Controls;
using ImageManipulator.Application.Common.Interfaces;
using System.Threading.Tasks;

namespace ImageManipulator.Application.Common.Services;

public class CommonDialogService : ICommonDialogService
{
    public Task<string[]> ShowFileDialogInNewWindow()
    {
        return new OpenFileDialog().ShowAsync(new Window());
    }
}
