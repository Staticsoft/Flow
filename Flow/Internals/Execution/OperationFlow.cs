using Staticsoft.Flow.Handlers;
using Staticsoft.Messages.Abstractions;
using Staticsoft.PartitionedStorage.Abstractions;
using Staticsoft.Serialization.Abstractions;
using System.Text;

namespace Staticsoft.Flow.Internals;

class OperationFlow<Input, Output>(
    Queue queue,
    Partitions partitions,
    Serializer serializer,
    OperationHandler<Input, Output> handler,
    JobContext context
) : Operation<Input, Output>
{
    readonly Queue Queue = queue;
    readonly Partitions Partitions = partitions;
    readonly Serializer Serializer = serializer;
    readonly OperationHandler<Input, Output> Handler = handler;
    readonly JobContext Context = context;

    public async Task<Output> Execute(Input input)
    {
        var operationId = ToOperationId(input);
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

            var operationReference = new OperationDataReference()
            {
                JobId = Context.JobId,
                OperationId = operationId
            };
            await Queue.Enqueue(Serializer.Serialize(operationReference));
            throw new OperationNotCompleteException();
        }
    }

    string ToOperationId(Input input)
    {
        var serialized = Serializer.Serialize(input);
        ReadOnlySpan<byte> span = Encoding.UTF8.GetBytes(serialized).AsSpan();
        var hash = MurmurHash.MurmurHash3.Hash32(ref span, seed: 0);
        return $"{hash}";
    }
}