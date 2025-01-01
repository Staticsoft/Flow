using Microsoft.Extensions.DependencyInjection;
using Staticsoft.Contracts.ASP.Client;
using Staticsoft.HttpCommunication.Json;
using Staticsoft.Serialization.Net;
using Staticsoft.Testing.Integration;
using Staticsoft.TestServer;

namespace Staticsoft.Flow.Tests;

public class TestBase<Startup> : IntegrationTestBase<Startup>
    where Startup : class
{
    protected override IServiceCollection ServerServices(IServiceCollection services)
        => base.ServerServices(services);

    protected override IServiceCollection ClientServices(IServiceCollection services)
        => base.ClientServices(services)
            .UseClientAPI<Api>()
            .UseSystemJsonSerializer()
            .UseJsonHttpCommunication();

    protected Api Api
        => Client<Api>();
}
