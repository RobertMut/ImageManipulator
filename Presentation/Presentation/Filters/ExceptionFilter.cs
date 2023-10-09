using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reactive.Concurrency;
using System.Text;
using ImageManipulator.Application.Common.Interfaces;
using ReactiveUI;

namespace ImageManipulator.Presentation.Filters;

public class ExceptionFilter : IObserver<Exception>
{
    private readonly ICommonDialogService _commonDialogService;
    private readonly IDictionary<Type, Action<Exception>?> _exceptionHandlers;

    public ExceptionFilter(ICommonDialogService commonDialogService)
    {
        _commonDialogService = commonDialogService;
        _exceptionHandlers = new Dictionary<Type, Action<Exception>?>()
        {
            {typeof(NullReferenceException), HandleNullReferenceException},
            {typeof(IOException), HandleIOException}
        };
    }

    public void OnCompleted()
    {
        RxApp.MainThreadScheduler.Schedule(() => { });
    }

    public void OnError(Exception error)
    {
        RxApp.MainThreadScheduler.Schedule(() => { throw error; });
    }

    public void OnNext(Exception value)
    {
#if DEBUG
        if (Debugger.IsAttached)
        {
            Debugger.Break();
        }
#endif
        
        if (_exceptionHandlers.TryGetValue(value.GetType(), out Action<Exception>? action))
        {
            RxApp.MainThreadScheduler.Schedule(() => action(value));
        }
        else
        {
            RxApp.MainThreadScheduler.Schedule(() => HandleUnknownException(value));
        }
    }
    
    private void HandleNullReferenceException(Exception obj)
    {
        string exceptionMessage = CreateException("Missing value", obj);
        RxApp.MainThreadScheduler.Schedule(() => _commonDialogService.ShowException(exceptionMessage));
    }
    
    private void HandleIOException(Exception obj)
    {
        string exceptionMessage = CreateException("Error occured during processing file", obj);
        RxApp.MainThreadScheduler.Schedule(() => _commonDialogService.ShowException(exceptionMessage));
    }

    private void HandleUnknownException(Exception obj)
    {
        string exceptionMessage = CreateException("Unknown exception occured", obj);
        RxApp.MainThreadScheduler.Schedule(() => _commonDialogService.ShowException(exceptionMessage));
    }
    
    private static string CreateException(string description, Exception e)
    {
        StringBuilder sb = new();
        
        sb.AppendLine("Exception occured!");
        sb.AppendLine(description);
        sb.AppendLine(e.Message);
        
        return sb.ToString();
    }
}