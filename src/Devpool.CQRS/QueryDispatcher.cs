using Devpool.CQRS.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Devpool.CQRS;

public class QueryDispatcher : IQueryDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public QueryDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task<TResponse> DispatchAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken)
    {
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResponse));
        dynamic handler = _serviceProvider.GetRequiredService(handlerType);
        
        if (handler == null)
        {
            throw new Exception($"Handler for query '{query.GetType()}' not found.");
        }

        return handler.HandleAsync((dynamic)query, cancellationToken);
    }
}
//d