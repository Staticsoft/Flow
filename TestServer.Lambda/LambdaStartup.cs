
using Amazon.DynamoDBv2;
using Amazon.SQS;
using Staticsoft.Flow;
using Staticsoft.Flow.Lambda;
using Staticsoft.Messages.Abstractions;
using Staticsoft.Messages.Sqs;
using Staticsoft.PartitionedStorage.Abstractions;
using Staticsoft.PartitionedStorage.AWS;

namespace Staticsoft.TestServer.Lambda;

public class LambdaStartup : StartupBase
{
    protected override IServiceCollection RegisterServices(IServiceCollection services)
        => base.RegisterServices(services)
            .AddSingleton<Partitions, DynamoDBPartitions>()
            .AddSingleton(DynamoDbOptions())
            .AddSingleton<AmazonDynamoDBClient>()
            .AddSingleton<IAmazonSQS, AmazonSQSClient>()
            .AddSingleton<Queue, SqsQueue>()
            .AddSingleton(SqsOptions())
            .UseFlow();

    static DynamoDBPartitionedStorageOptions DynamoDbOptions()
        => new() { TableNamePrefix = Configuration("DynamoDbTableNamePrefix") };

    static SqsQueueOptions SqsOptions()
        => new() { QueueUrl = Configuration("SqsQueueUrl") };

    static string Configuration(string name)
        => Environment.GetEnvironmentVariable(name)
        ?? throw new NullReferenceException($"Environment varialbe {name} is not set");

    protected override void ConfigureEndpoints(IEndpointRouteBuilder endpoints) => endpoints
        .UseLambdaFlow();
}