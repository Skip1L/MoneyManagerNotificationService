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
        if (command == null)
            throw new ArgumentNullException(nameof(command));

        var handlerType = typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResult));
        var handler = _serviceProvider.GetService(handlerType);

        if (handler == null)
            throw new InvalidOperationException($"No handler found for {command.GetType().Name}");

        var method = handlerType.GetMethod("Handle");

        if (method == null)
            throw new InvalidOperationException($"Handle method not found on {handlerType.Name}.");

        var result = method.Invoke(handler, [command, cancellationToken]);

        if (result == null)
            throw new InvalidOperationException("The handler returned a null result.");

        return await (Task<TResult>)result;
    }
}
