using Staticsoft.Flow.Handlers;
using Staticsoft.Messages.Abstractions;
using Staticsoft.PartitionedStorage.Abstractions;
using Staticsoft.Serialization.Abstractions;

namespace Staticsoft.Flow.Internals;

class JobFlow<Input, Output>(
    Queue queue,
    Partitions partitions,
    Serializer serializer,
    JobHandler<Input, Output> handler
) : Job<Input, Output>
{
    readonly Queue Queue = queue;
    readonly Partitions Partitions = partitions;
    readonly Serializer Serializer = serializer;
    readonly JobHandler<Input, Output> Handler = handler;

    public async Task<string> Create(Input input)
    {
        var jobId = $"{Guid.NewGuid()}";
        var job = new JobData()
        {
            CreatedAt = DateTime.UtcNow,
            Input = Serializer.Serialize(input),
            Handler = Handler.Name
        };
        await Partitions.Get<JobData>().Save(jobId, job);

        var jobReference = new JobDataReference()
        {
            JobId = jobId
        };
        await Queue.Enqueue(Serializer.Serialize(jobReference));
        return jobId;
    }

    public async Task<Output> GetResult(string jobId)
    {
        var jobItem = await Partitions.Get<JobData>().Get(jobId);
        return Serializer.Deserialize<Output>(jobItem.Data.Output);
    }

    public async Task<JobStatus> GetStatus(string jobId)
    {
        var job = await Partitions.Get<JobData>().Get(jobId);
        var operations = await Partitions.Get<OperationData>($"{nameof(JobData)}{jobId}").Scan();
        return new()
        {
            IsCompleted = job.Data.IsComplete,
            TotalSteps = operations.Length + 1,
            CompletedSteps = operations.Count(operation => operation.Data.IsComplete) + (job.Data.IsComplete ? 1 : 0)
        };
    }

    public Task<Output> Execute(Input input)
        => throw new Exception("Jobs cannot be executed directly");
}