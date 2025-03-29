using Staticsoft.Flow;
using Staticsoft.PartitionedStorage.Abstractions;

namespace Staticsoft.TestServer;

public class RecordDecisionOperation(
    Partitions partitions
) : Operation<RecordDecisionInput, RecordDecisionOutput>
{
    readonly Partition<UserDecision> Decisions = partitions.Get<UserDecision>();

    public async Task<RecordDecisionOutput> Execute(RecordDecisionInput input)
    {
        await Decisions.Save(input.DecisionId, new() { Id = input.DecisionId });
        return new();
    }
}
