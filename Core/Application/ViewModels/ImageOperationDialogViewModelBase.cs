using ReactiveUI;

namespace ImageManipulator.Application.ViewModels;

public abstract class ImageOperationDialogViewModelBase : ViewModelBase
{
    public abstract Avalonia.Media.Imaging.Bitmap AfterImage { get; set; }
    public abstract Avalonia.Media.Imaging.Bitmap BeforeImage { get; set; }
}
