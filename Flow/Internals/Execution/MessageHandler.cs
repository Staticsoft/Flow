using Microsoft.Extensions.DependencyInjection;
using Staticsoft.Flow.Handlers;
using Staticsoft.Messages.Abstractions;
using Staticsoft.PartitionedStorage.Abstractions;
using Staticsoft.Serialization.Abstractions;
using System.Reflection;

namespace Staticsoft.Flow.Internals;

partial class MessageHandler(
    Partitions partitions,
    Serializer serializer,
    IEnumerable<OperationHandler> operationHandlers,
    Queue queue,
    IServiceProvider provider
)
{
    readonly Partitions Partitions = partitions;
    readonly Serializer Serializer = serializer;
    readonly Dictionary<string, Generic> OperationHandlers = operationHandlers.ToDictionary(
        handler => handler.Name,
        handler => new Generic(handler, serializer)
    );
    readonly Queue Queue = queue;
    readonly IServiceProvider Provider = provider;

    public Task Process(Queue.Message message) => ToReference(message).Type switch
    {
        DataReferenceType.Operation => ProcessOperation(ToOperationReference(message)),
        DataReferenceType.Job => ProcessJob(ToJobReference(message)),
        var unsupported => throw new NotSupportedException($"Unsupported reference type '{unsupported}'")
    };

    async Task ProcessOperation(OperationDataReference reference)
    {
        var operationItem = await Operations(reference).Get(reference.OperationId);
        var operation = operationItem.Data;

        var result = await OperationHandlers[operation.Handler].Execute(operation.Input);

        await Operations(reference).Save(
            reference.OperationId,
            operation with { IsComplete = true, Output = result },
            operationItem.Version
        );

        var updatedJob = await Retries.Storage(() => UpdateJobProgress(reference.JobId));
        if (updatedJob.Operations.Completed == updatedJob.Operations.Total)
        {
            var jobReference = new JobDataReference()
            {
                JobId = reference.JobId
            };
            await Queue.Enqueue(Serializer.Serialize(jobReference));
        }
    }

    async Task<JobData> UpdateJobProgress(string jobId)
    {
        var jobItem = await Partitions.Get<JobData>().Get(jobId);
        var job = jobItem.Data;

        var updatedJob = job with
        {
            Operations = job.Operations with
            {
                Completed = job.Operations.Completed + 1
            }
        };
        await Partitions.Get<JobData>().Save(jobId, updatedJob, jobItem.Version);

        return updatedJob;
    }

    async Task ProcessJob(JobDataReference reference)
    {
        var jobItem = await Partitions.Get<JobData>().Get(reference.JobId);
        var job = jobItem.Data;

        using var scope = Provider.CreateScope();
        var services = scope.ServiceProvider;
        services.GetRequiredService<JobContext>().JobId = reference.JobId;
        var handler = services
            .GetRequiredService<IEnumerable<JobHandler>>()
            .Single(handler => handler.Name == job.Handler);
        var jobHandler = new Generic(handler, Serializer);
        try
        {
            var result = await jobHandler.Execute(job.Input);

            await Partitions.Get<JobData>().Save(
                reference.JobId,
                job with { IsComplete = true, Output = result },
                jobItem.Version
            );
        }
        catch (OperationNotCompleteException)
        {
            await ScheduleJobOperations(reference, jobItem);
        }
    }

    async Task ScheduleJobOperations(JobDataReference reference, Item<JobData> jobItem)
    {
        var partitionId = $"{nameof(JobData)}{reference.JobId}";
        var partition = Partitions.Get<OperationData>(partitionId);

        var operations = await partition.Scan();

        await Partitions.Get<JobData>().Save(
            reference.JobId,
            jobItem.Data with
            {
                Operations = jobItem.Data.Operations with
                {
                    Total = operations.Length,
                    Completed = operations.Count(operation => operation.Data.IsComplete)
                }
            },
            jobItem.Version
        );

        var tasks = operations
            .Where(operation => !operation.Data.IsComplete)
            .Select(operation => new OperationDataReference()
            {
                JobId = reference.JobId,
                OperationId = operation.Id
            })
            .Select(reference => Queue.Enqueue(Serializer.Serialize(reference)));

        await Task.WhenAll(tasks);
    }

    Partition<OperationData> Operations(OperationDataReference reference)
        => Partitions.Get<OperationData>($"{nameof(JobData)}{reference.JobId}");

    DataReference ToReference(Queue.Message message)
        => Serializer.Deserialize<DataReference>(message.Body);

    OperationDataReference ToOperationReference(Queue.Message message)
        => Serializer.Deserialize<OperationDataReference>(message.Body);

    JobDataReference ToJobReference(Queue.Message message)
        => Serializer.Deserialize<JobDataReference>(message.Body);

    class Generic(
        Handler handler,
        Serializer serializer
    )
    {
        readonly Handler Handler = handler;
        readonly Serializer Serializer = serializer;
        readonly MethodInfo ExecuteMethod = GenericExecuteMethod.MakeGenericMethod(handler.InputType, handler.OutputType);

        public async Task<string> Execute(string message)
            => await (Task<string>)ExecuteMethod.Invoke(this, [Handler, message])!;

        async Task<string> Execute<Input, Output>(Handler<Input, Output> handler, string message)
        {
            var data = Serializer.Deserialize<Input>(message);
            var result = await handler.Execute(data);
            return Serializer.Serialize(result);
        }

        static readonly MethodInfo GenericExecuteMethod = typeof(Generic)
            .GetMethod(nameof(Execute), BindingFlags.NonPublic | BindingFlags.Instance)!;
    }
}
