using System.Threading;
using System.Threading.Tasks;

namespace ImageManipulator.Domain.Common.CQRS.Interfaces;

public interface IQueryDispatcher
{
    Task<TQueryResult> Dispatch<TQuery, TQueryResult>(TQuery query, CancellationToken cancellationToken);
}