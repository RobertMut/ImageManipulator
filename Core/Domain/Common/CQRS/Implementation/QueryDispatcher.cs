using System;
using System.Threading;
using System.Threading.Tasks;
using ImageManipulator.Domain.Common.CQRS.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ImageManipulator.Domain.Common.CQRS.Implementation;

public class QueryDispatcher : IQueryDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public QueryDispatcher(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

    public Task<TQueryResult> Dispatch<TQuery, TQueryResult>(TQuery query, CancellationToken cancellationToken)
    {
        IQueryHandler<TQuery, TQueryResult> handler = _serviceProvider.GetRequiredService<IQueryHandler<TQuery, TQueryResult>>();
        
        return handler.Handle(query, cancellationToken);
    }
}