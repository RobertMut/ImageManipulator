using System.Threading;
using System.Threading.Tasks;

namespace ImageManipulator.Domain.Common.CQRS.Interfaces;

public interface IQueryHandler<in TQuery, TQueryResult>
{
    Task<TQueryResult> Handle(TQuery query, CancellationToken cancellationToken);
}