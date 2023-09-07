using System.Reactive;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.Input;
using ReactiveUI;

namespace ImageManipulator.Application.ViewModels;

public abstract class ImageOperationDialogViewModelBase : ViewModelBase
{
    #region PropertyHelpers

    protected ObservableAsPropertyHelper<bool> _isExecuting;
    public bool IsExecuting => _isExecuting.Value;

    #endregion
    
    #region Commands
    
    public RelayCommand<Window> CancelCommand { get; protected set; }
    
    public RelayCommand<Window> AcceptCommand { get; protected set; }
    
    public ReactiveCommand<Unit, Unit> Execute { get; }
    
    #endregion
    
    public abstract Bitmap? AfterImage { get; set; }
    public abstract Bitmap? BeforeImage { get; set; }
    
    public bool AcceptCommandCanExecute() => AfterImage != null;

    protected void Cancel(Window window)
    {
        AfterImage = BeforeImage;
        window.Close();
    }

    protected void Accept(Window window) => window.Close();
}
