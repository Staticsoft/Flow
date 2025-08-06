using Staticsoft.Flow.Internals;
using Staticsoft.PartitionedStorage.Abstractions;

namespace Staticsoft.Flow.State;

public class FlowState(
    Partitions partitions
)
{
    readonly Partitions Partitions = partitions;
    readonly Partition<DecisionData> Decisions = partitions.Get<DecisionData>();
    readonly Partition<JobData> Jobs = partitions.Get<JobData>();

    Partition<OperationData> Operations(string jobId)
        => Partitions.Get<OperationData>($"{nameof(JobData)}{jobId}");

    public async Task<JobRecord[]> ListJobs()
    {
        var jobs = await Jobs.Scan();

        return jobs.Select(job => job.Data).ToArray();
    }

    public async Task<OperationRecord[]> ListJobOperations(string jobId)
    {
        var operations = await Operations(jobId).Scan();

        return operations.Select(operation => operation.Data).ToArray();
    }
}