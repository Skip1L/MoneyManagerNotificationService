using Application.Commands;
using Application.Common;
using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

public class CommandDispatcher : ICommandDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public CommandDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResult> Dispatch<TResult>(ICommand<TResult> command, CancellationToken cancellationToken)
    {
        // Resolve the handler
        var handlerType = typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResult));
        var handler = _serviceProvider.GetRequiredService(handlerType);

        if (handler == null)
            throw new InvalidOperationException($"No handler found for {command.GetType().Name}");

        // Invoke the Handle method
        var method = handlerType.GetMethod("Handle");
        if (method == null)
            throw new InvalidOperationException("Handle method not found.");

        return await (Task<TResult>)method.Invoke(handler, new object[] { command, cancellationToken });
    }
}
