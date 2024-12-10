
using Staticsoft.Flow.Memory;
using Staticsoft.Messages.Memory;
using Staticsoft.PartitionedStorage.Abstractions;
using Staticsoft.PartitionedStorage.Memory;

namespace Staticsoft.TestServer.Local;

public class LocalStartup : StartupBase
{
    protected override IServiceCollection RegisterServices(IServiceCollection services)
        => base.RegisterServices(services)
            .AddSingleton<Partitions, MemoryPartitions>()
            .UseLocalFlow(
                _ => new FlowMessagePollOptions() { Paralellism = 100 }
            )
            .UseMemoryQueue(_ => new() { Invisibility = TimeSpan.FromMinutes(1) });
}
