using System.Diagnostics;
using Devpool.CQRS.Abstractions;
using Microsoft.Extensions.Logging;

namespace Devpool.CQRS.CommandDecorators;

public class LoggingCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand> where TCommand : ICommand
{
    private readonly ICommandHandler<TCommand> _decorated;
    private readonly ILogger<LoggingCommandHandlerDecorator<TCommand>> _logger;

    public LoggingCommandHandlerDecorator(ICommandHandler<TCommand> decorated,
        ILogger<LoggingCommandHandlerDecorator<TCommand>> logger)
    {
        _decorated = decorated;
        _logger = logger;
    }

    public async Task HandleAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("{CommandTypeName} start...", command.GetType().Name);
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        await _decorated.HandleAsync(command, cancellationToken);
        stopwatch.Stop();
        _logger
            .LogInformation("{CommandTypeName} finished  in {ElapsedMilliseconds}ms", 
            command.GetType().Name,
            stopwatch.ElapsedMilliseconds);
    }
}