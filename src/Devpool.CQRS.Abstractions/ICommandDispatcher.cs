namespace Devpool.CQRS.Abstractions;

public interface ICommandDispatcher
{
    Task DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken=default ) where TCommand : ICommand;
}