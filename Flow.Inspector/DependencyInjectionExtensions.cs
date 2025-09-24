using Microsoft.Extensions.DependencyInjection;

namespace Staticsoft.Flow.Inspector;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection UseFlowInspector(this IServiceCollection services)
        => services;
}
