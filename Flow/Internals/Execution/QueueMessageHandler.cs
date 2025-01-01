using Microsoft.Extensions.Logging;
using Staticsoft.Flow.Handlers;
using Staticsoft.Messages.Abstractions;

namespace Staticsoft.Flow.Internals;

class QueueMessageHandler(
    Queue queue,
    MessageHandler message,
    ILogger<QueueMessageHandler> logger
) : FlowHandler
{
    readonly Queue Queue = queue;
    readonly MessageHandler Message = message;
    readonly ILogger<QueueMessageHandler> Logger = logger;

    public async Task Process(Queue.Message message)
    {
        try
        {
            await Message.Process(message);
            await Queue.Delete(message.Id);
        }
        catch (Exception ex)
        {
            Logger.LogError($"Unable to process message {message.Id} with body {message.Body}");
            Logger.LogError(ex, $"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            await Queue.ResetVisibility(message.Id, DateTime.UtcNow.AddSeconds(10));
        }
    }
}