using Staticsoft.Asp.Lambda;
using Staticsoft.Flow.Lambda;

namespace Staticsoft.TestServer.Lambda;

public class LambdaEntrypoint : LambdaEntrypoint<LambdaStartup>
{
    protected override TriggerCollection Triggers => base.Triggers
        .AddTrigger<MessageTrigger>()
        .AddTrigger<RequestTrigger>();
}