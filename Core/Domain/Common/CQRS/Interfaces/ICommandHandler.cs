using System.Threading;
using System.Threading.Tasks;

namespace ImageManipulator.Domain.Common.CQRS.Interfaces;

public interface ICommandHandler<in TCommand, TCommandResult>
{
    Task<TCommandResult> Handle(TCommand command, CancellationToken cancellationToken);
}