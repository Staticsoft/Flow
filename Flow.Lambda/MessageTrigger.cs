using Amazon.Lambda.APIGatewayEvents;
using Staticsoft.Asp.Lambda;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Staticsoft.Flow.Lambda;

public class MessageTrigger : TriggerSource
{
    public bool TryConvert(
        JsonElement request,
        JsonSerializerOptions options,
        [NotNullWhen(true)] out APIGatewayProxyRequest? proxyRequest
    )
    {
        proxyRequest = null;
        if (!request.TryGetProperty("Records", out var records)) return false;

        var message = JsonSerializer.Deserialize<Message>(records[0], options)!;
        proxyRequest = ToProxyRequest(message);
        return true;
    }

    static APIGatewayProxyRequest ToProxyRequest(Message message) => new()
    {
        Body = JsonSerializer.Serialize(new FlowMessage()
        {
            Body = message.body,
            Id = message.receiptHandle
        }),
        HttpMethod = "POST",
        Path = "/Flow",
        Resource = "/Flow"
    };

    class Message
    {
        public required string receiptHandle { get; init; }
        public required string body { get; init; }
    }
}
