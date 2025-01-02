using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Staticsoft.Flow.Handlers;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Staticsoft.Flow.Lambda;

public static class FlowEndpoints
{
    public static IEndpointRouteBuilder UseLambdaFlow(this IEndpointRouteBuilder builder)
    {
        builder.MapPost("/Flow", async (
            HttpRequest request,
            FlowHandler flow
        ) =>
        {
            var body = await GetBody(request);
            var message = JsonSerializer.Deserialize<FlowMessage>(body)
                ?? throw new NotSupportedException($"Unexpected flow message body: {body}");

            await flow.Process(new() { Id = message.Id, Body = message.Body });
        });

        return builder;
    }

    static Task<string> GetBody(HttpRequest request)
    {
        using var reader = new StreamReader(request.Body);
        return reader.ReadToEndAsync();
    }
}