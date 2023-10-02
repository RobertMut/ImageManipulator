using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.Input;
using ReactiveUI;

namespace ImageManipulator.Application.ViewModels;

public abstract class ImageOperationDialogViewModelBase : ViewModelBase
{
    
    #region Commands
    
    public ReactiveCommand<Window, Unit>? CancelCommand { get; protected set; }
                           
    public ReactiveCommand<Window, Unit>? AcceptCommand { get; protected set; }
    
    #endregion
    
    public abstract Bitmap? AfterImage { get; set; }
    public abstract Bitmap? BeforeImage { get; set; }
    
    protected async Task Cancel(Window? window)
    {
        AfterImage = null;
        window.Close();
    }

    protected async Task Accept(Window? window) => window.Close();
}
