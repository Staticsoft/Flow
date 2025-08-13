using Staticsoft.Contracts.ASP.Server;
using Staticsoft.Flow;
using Staticsoft.PartitionedStorage.Abstractions;
using Staticsoft.Serialization.Net;

namespace Staticsoft.TestServer;

public class StartupBase
{
    public void ConfigureServices(IServiceCollection services)
        => RegisterServices(services);

    public void Configure(IApplicationBuilder app, IWebHostEnvironment _) => app
        .UseRouting()
        .UseServerAPIRouting<Api>()
        .UseEndpoints(ConfigureEndpoints);

    protected virtual IServiceCollection RegisterServices(IServiceCollection services) => services
        .UseServerAPI<Api>(typeof(StartupBase).Assembly)
        .UseSystemJsonSerializer()
        .AddSingleton<ItemSerializer, JsonItemSerializer>()

        .UseJob<CalculateSumOfSquaresJob, CalculateSumOfSquaresInput, CalculateSumOfSquaresOutput>()
        .UseOperation<CalculateSquareOperation, CalculateSquareInput, CalculateSquareOutput>()

        .UseJob<UpdateEmailJob, UpdateEmailJobInput, UpdateEmailJobOutput>()
        .UseOperation<RecordInputOperation, RecordInputIdInput, RecordInputIdOutput>()
        .UseOperation<UpdateEmailOperation, UpdateEmailOperationInput, UpdateEmailOperationOutput>();

    protected virtual void ConfigureEndpoints(IEndpointRouteBuilder endpoints) { }
}
