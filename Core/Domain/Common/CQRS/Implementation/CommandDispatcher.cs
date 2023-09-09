using System;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using ImageManipulator.Domain.Common.CQRS.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace ImageManipulator.Domain.Common.CQRS.Implementation;

public class CommandDispatcher : ICommandDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    
    public CommandDispatcher(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

    public Task<TCommandResult> Dispatch<TCommand, TCommandResult>(TCommand command, CancellationToken cancellationToken)
    {
        var handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand, TCommandResult>>();

        return handler.Handle(command, cancellationToken);
    }
}