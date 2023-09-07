using ReactiveUI;

namespace ImageManipulator.Application.ViewModels;

public class ViewModelBase : ReactiveObject
{    
    protected ObservableAsPropertyHelper<bool> isCommandActive;

    public bool IsCommandActive => isCommandActive.Value;
}
