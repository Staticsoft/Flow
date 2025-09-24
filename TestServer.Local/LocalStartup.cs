
using Staticsoft.Flow.Memory;
using Staticsoft.Messages.Memory;
using Staticsoft.PartitionedStorage.Abstractions;
using Staticsoft.PartitionedStorage.Memory;

namespace Staticsoft.TestServer.Local;

public class LocalStartup : StartupBase
{
    protected override IServiceCollection RegisterServices(IServiceCollection services)
        => base.RegisterServices(services)
            .AddRazorPages().Services
            .AddSingleton<Partitions, MemoryPartitions>()
            .UseLocalFlow(
                _ => new FlowMessagePollOptions() { Paralellism = 100 }
            )
            .UseMemoryQueue(_ => new() { Invisibility = TimeSpan.FromMinutes(1) });

    public override void Configure(IApplicationBuilder app, IWebHostEnvironment _)
    {
        base.Configure(app, _);

        app.UseWebAssemblyDebugging();
        app.UseBlazorFrameworkFiles();
        app.UseStaticFiles();
    }

    protected override void ConfigureEndpoints(IEndpointRouteBuilder endpoints)
    {
        base.ConfigureEndpoints(endpoints);

        endpoints.MapRazorPages();
        endpoints.MapFallbackToFile("index.html");
    }
}
