using Staticsoft.Messages.Abstractions;
using Staticsoft.PartitionedStorage.Abstractions;
using Staticsoft.Serialization.Abstractions;

namespace Staticsoft.Flow.Internals;

class ExternalInputFlow(
    Partitions partitions,
    Serializer serializer,
    Queue queue
) : ExternalInput
{
    readonly Partitions Partitions = partitions;
    readonly Serializer Serializer = serializer;
    readonly Queue Queue = queue;

    public Task Provide<Data>(string externalId, Data input)
        => externalId.Split(':') switch
        {
            [var jobId, var id] => Provide(jobId, id, input),
            _ => throw new NotSupportedException($"{externalId} is not valid id for external input")
        };

    Task Provide<Data>(string jobId, string id, Data input)
        => Retries.Storage(() => ProvideInput(jobId, id, input));

    async Task ProvideInput<Data>(string jobId, string id, Data input)
    {
        var partition = Partitions.Get<ExternalInputData>($"{nameof(ExternalInputData)}{jobId}");

        var serializedInput = Serializer.Serialize(input);

        try
        {
            var item = await partition.Get(id);

            await partition.Save(id, item.Data with { Input = serializedInput }, item.Version);
        }
        catch (PartitionedStorageItemNotFoundException)
        {
            await partition.Save(id, new() { Input = serializedInput });
        }

        await TriggerJob(jobId);
    }

    async Task TriggerJob(string jobId)
    {
        var serializedJobReference = Serializer.Serialize(new JobDataReference()
        {
            JobId = jobId
        });
        await Queue.Enqueue(serializedJobReference);
    }
}

class ExternalInputFlow<Seed, Data>(
    Partitions partitions,
    Serializer serializer,
    JobContext context,
    DataConverter<Seed, Data> converter
) : ExternalInput<Seed, Data>
{
    readonly Partition<ExternalInputData> Partition = partitions.Get<ExternalInputData>($"{nameof(ExternalInputData)}{context.JobId}");
    readonly Serializer Serializer = serializer;
    readonly JobContext Context = context;
    readonly DataConverter<Seed, Data> Converter = converter;

    class ResolvedInput(
        string id,
        Data input
    ) : ExternalInput<Data>
    {
        public string Id { get; } = id;

        public Data Input { get; } = input;
    }

    class UnresolvedInput(
        string id
    ) : ExternalInput<Data>
    {
        public string Id { get; } = id;

        public Data Input
            => throw new OperationNotCompleteException();
    }

    public async Task<ExternalInput<Data>> Create(Seed input)
    {
        var id = Converter.ToId(input);
        var externalId = $"{Context.JobId}:{id}";

        try
        {
            var item = await Partition.Get(id);
            if (string.IsNullOrEmpty(item.Data.Input)) return new UnresolvedInput(externalId);

            return new ResolvedInput(externalId, Serializer.Deserialize<Data>(item.Data.Input));
        }
        catch (PartitionedStorageItemNotFoundException)
        {
            return new UnresolvedInput(externalId);
        }
    }
}