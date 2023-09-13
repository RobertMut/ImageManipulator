using Avalonia.Controls;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.Input;

namespace ImageManipulator.Application.ViewModels;

public abstract class ImageOperationDialogViewModelBase : ViewModelBase
{
    #region Commands
    
    public RelayCommand<Window>? CancelCommand { get; protected set; }
    
    public RelayCommand<Window>? AcceptCommand { get; protected set; }
    
    #endregion
    
    public abstract Bitmap? AfterImage { get; set; }
    public abstract Bitmap? BeforeImage { get; set; }

    protected bool AcceptCommandCanExecute() => AfterImage != null;

    protected void Cancel(Window? window)
    {
        AfterImage = BeforeImage;
        window.Close();
    }

    protected void Accept(Window? window) => window.Close();
}
