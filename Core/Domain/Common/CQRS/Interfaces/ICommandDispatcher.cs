using System.Threading;
using System.Threading.Tasks;

namespace ImageManipulator.Domain.Common.CQRS.Interfaces;

public interface ICommandDispatcher
{
    Task<TCommandResult> Dispatch<TCommand, TCommandResult>(TCommand command, CancellationToken cancellationToken);
}