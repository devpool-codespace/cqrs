using Devpool.CQRS.Abstractions;
using Microsoft.Extensions.Logging;

namespace Devpool.CQRS.CommandDecorators;

public class ExceptionHandlingCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand> where TCommand : ICommand
{
    private readonly ICommandHandler<TCommand> _decorated;
    private readonly ILogger<ExceptionHandlingCommandHandlerDecorator<TCommand>> _logger;

    public ExceptionHandlingCommandHandlerDecorator(ICommandHandler<TCommand> decorated, ILogger<ExceptionHandlingCommandHandlerDecorator<TCommand>> logger)
    {
        _decorated = decorated;
        _logger = logger;
    }

    public async Task HandleAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            await _decorated.HandleAsync(command, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while handling command: {command.GetType().Name}");
            throw ex;
        }
    }
}