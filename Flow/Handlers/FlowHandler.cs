using Staticsoft.Messages.Abstractions;

namespace Staticsoft.Flow.Handlers;

public interface FlowHandler
{
    Task Process(Queue.Message message);
}
