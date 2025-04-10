﻿using Staticsoft.Messages.Abstractions;
using Staticsoft.PartitionedStorage.Abstractions;
using Staticsoft.Serialization.Abstractions;

namespace Staticsoft.Flow.Internals;

class DecisionFlow(
    Partitions partitions,
    Queue queue,
    Serializer serializer,
    JobContext context
) : Decision
{
    readonly Partition<DecisionData> Partition = partitions.Get<DecisionData>();
    readonly Queue Queue = queue;
    readonly Serializer Serializer = serializer;
    readonly JobContext Context = context;

    public Task<string> Create(string id)
        => Retries.Storage(() => CreateDecision(id));

    async Task<string> CreateDecision(string id)
    {
        try
        {
            var item = await Partition.Get(id);

            if (string.IsNullOrEmpty(item.Data.JobId))
            {
                await Partition.Save(id, item.Data with { JobId = Context.JobId }, item.Version);
            }

            if (!string.IsNullOrEmpty(item.Data.Choice)) return item.Data.Choice;
        }
        catch (PartitionedStorageItemNotFoundException)
        {
            await Partition.Save(id, new() { JobId = Context.JobId });
        }
        throw new OperationNotCompleteException();
    }

    public Task Make(string id, string choice)
        => Retries.Storage(() => MakeDecision(id, choice));

    async Task MakeDecision(string id, string choice)
    {
        try
        {
            var item = await Partition.Get(id);
            await Partition.Save(id, item.Data with { Choice = choice }, item.Version);

            var jobReference = new JobDataReference()
            {
                JobId = item.Data.JobId
            };
            await Queue.Enqueue(Serializer.Serialize(jobReference));
        }
        catch (PartitionedStorageItemNotFoundException)
        {
            await Partition.Save(id, new() { Choice = choice });
        }
    }
}