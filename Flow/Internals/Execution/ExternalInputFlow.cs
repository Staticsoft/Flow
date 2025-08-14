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
    {
        var partition = Partitions.Get<ExternalInputData>($"{nameof(ExternalInputData)}{jobId}");
        var serializedInput = Serializer.Serialize(input);

        return Retries.Storage(() => ProvideInput<Data>(partition, jobId, id, serializedInput));
    }

    async Task ProvideInput<Data>(
        Partition<ExternalInputData> partition,
        string jobId,
        string id,
        string input
    )
    {
        try
        {
            var item = await partition.Get(id);

            await partition.Save(id, item.Data with { Input = input }, item.Version);

            await TriggerJob(jobId);
        }
        catch (PartitionedStorageItemNotFoundException)
        {
            await partition.Save(id, new() { Input = input });
        }
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
    readonly Partitions Partitions = partitions;
    readonly Serializer Serializer = serializer;
    readonly JobContext Context = context;
    readonly DataConverter<Seed, Data> Converter = converter;

    public ExternalInput<Data> Create(Seed input)
    {
        var id = Converter.ToId(input);
        return new Input(Partitions, Serializer, Context.JobId, id);
    }

    class Input(
        Partitions partitions,
        Serializer serializer,
        string jobId,
        string id
    ) : ExternalInput<Data>
    {
        readonly Partition<ExternalInputData> Partition
            = partitions.Get<ExternalInputData>($"{nameof(ExternalInputData)}{jobId}");
        readonly Serializer Serializer = serializer;
        readonly string InputId = id;

        public string Id { get; } = $"{jobId}:{id}";

        public Task<Data> Get()
            => Retries.Storage(GetInput);

        async Task<Data> GetInput()
        {
            try
            {
                var item = await Partition.Get(InputId);

                if (string.IsNullOrEmpty(item.Data.Input)) throw new OperationNotCompleteException();

                return Serializer.Deserialize<Data>(item.Data.Input);
            }
            catch (PartitionedStorageItemNotFoundException)
            {
                await Partition.Save(InputId, new() { });

                throw new OperationNotCompleteException();
            }
        }
    }
}
