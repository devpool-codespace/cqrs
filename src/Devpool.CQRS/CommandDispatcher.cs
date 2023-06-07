using Devpool.CQRS.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Devpool.CQRS;

public class CommandDispatcher : ICommandDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CommandDispatcher> _logger;

    public CommandDispatcher(IServiceProvider serviceProvider, ILogger<CommandDispatcher> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
        where TCommand : ICommand
    {
        try
        {
            var handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand>>();
            await handler.HandleAsync(command, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }
}