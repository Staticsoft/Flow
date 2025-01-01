using Microsoft.Extensions.DependencyInjection;

namespace Staticsoft.Flow.Memory;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection UseLocalFlow(
        this IServiceCollection services,
        Func<IServiceProvider, FlowMessagePollOptions> options
    ) => services
            .UseFlow()
            .AddHostedService<FlowMessagePoll>()
            .AddSingleton(options);
}
