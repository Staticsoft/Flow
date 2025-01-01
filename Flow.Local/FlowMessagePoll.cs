using Microsoft.Extensions.Hosting;
using Staticsoft.Flow.Handlers;
using Staticsoft.Messages.Abstractions;

namespace Staticsoft.Flow.Memory;

public class FlowMessagePoll(
    FlowMessagePollOptions options,
    Queue queue,
    FlowHandler handler
) : IHostedService
{
    readonly SemaphoreSlim Paralellism = new(options.Paralellism, options.Paralellism);
    readonly Queue Queue = queue;
    readonly FlowHandler Handler = handler;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(() => ReadQueue(cancellationToken), cancellationToken);
        return Task.CompletedTask;
    }

    async Task ReadQueue(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await Paralellism.WaitAsync(cancellationToken);
            var message = await Queue.Dequeue(cancellationToken);
            _ = Task.Run(() => ProcessMessage(message), cancellationToken);
        }
    }

    async Task ProcessMessage(Queue.Message message)
    {
        await Handler.Process(message);
        Paralellism.Release();
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}
