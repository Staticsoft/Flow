using Staticsoft.Flow.Handlers;
using Staticsoft.PartitionedStorage.Abstractions;
using Staticsoft.Serialization.Abstractions;

namespace Staticsoft.Flow.Internals;

class OperationFlow<Input, Output>(
    Partitions partitions,
    Serializer serializer,
    OperationHandler<Input, Output> handler,
    JobContext context,
    DataConverter<Input, Output> converter
) : Operation<Input, Output>
{
    readonly Partitions Partitions = partitions;
    readonly Serializer Serializer = serializer;
    readonly OperationHandler<Input, Output> Handler = handler;
    readonly JobContext Context = context;
    readonly DataConverter<Input, Output> Converter = converter;

    public async Task<Output> Execute(Input input)
    {
        var operationId = Converter.ToId(input);
        var operation = new OperationData()
        {
            Input = Serializer.Serialize(input),
            Handler = Handler.Name
        };
        var partitionId = $"{nameof(JobData)}{Context.JobId}";
        var partition = Partitions.Get<OperationData>(partitionId);
        try
        {
            var item = await partition.Get(operationId);
            if (!item.Data.IsComplete) throw new OperationNotCompleteException();

            return Serializer.Deserialize<Output>(item.Data.Output);
        }
        catch (PartitionedStorageItemNotFoundException)
        {
            await partition.Save(operationId, operation);

            throw new OperationNotCompleteException();
        }
    }
}