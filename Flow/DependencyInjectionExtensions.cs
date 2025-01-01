using Microsoft.Extensions.DependencyInjection;
using Staticsoft.Flow.Handlers;
using Staticsoft.Flow.Internals;

namespace Staticsoft.Flow;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection UseFlow(this IServiceCollection services) => services
        .AddSingleton<FlowHandler, QueueMessageHandler>()
        .AddSingleton<MessageHandler>()
        .AddScoped<JobContext>()
        .AddScoped(typeof(Job<,>), typeof(JobFlow<,>))
        .AddScoped(typeof(Operation<,>), typeof(OperationFlow<,>));

    public static IServiceCollection UseJob<Job, Input, Output>(this IServiceCollection services)
        where Job : class, Job<Input, Output>
        => services
            .AddScoped<Job>()
            .AddScoped<JobHandler>(provider => provider.GetRequiredService<Job>())
            .AddScoped<JobHandler<Input, Output>>(provider => provider.GetRequiredService<Job>());

    public static IServiceCollection UseOperation<Operation, Input, Output>(this IServiceCollection services)
        where Operation : class, Operation<Input, Output>
        => services
            .AddSingleton<Operation>()
            .AddSingleton<OperationHandler>(provider => provider.GetRequiredService<Operation>())
            .AddSingleton<OperationHandler<Input, Output>>(provider => provider.GetRequiredService<Operation>());
}
