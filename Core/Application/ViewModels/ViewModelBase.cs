using ReactiveUI;

namespace ImageManipulator.Application.ViewModels;

public class ViewModelBase : ReactiveObject
{    
    protected ObservableAsPropertyHelper<bool>? isCommandActive;

    protected bool IsCommandActive => isCommandActive.Value;
}
