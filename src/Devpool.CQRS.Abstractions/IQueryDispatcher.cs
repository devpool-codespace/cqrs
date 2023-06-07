namespace Devpool.CQRS.Abstractions;

public interface IQueryDispatcher
{
    Task<TResponse> DispatchAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default);
}