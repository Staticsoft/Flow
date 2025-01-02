using Staticsoft.Jobs.Abstractions;
using Staticsoft.PartitionedStorage.Abstractions;

namespace Staticsoft.Flow.Internals;

class StorageCleanupJob(
    Partitions partitions
) : Job
{
    readonly Partitions Partitions = partitions;

    readonly static Schedule EveryDay = new() { Hours = 24, Minutes = 60 };
    readonly static TimeSpan ExpiredJob = TimeSpan.FromDays(7);

    public Schedule Schedule { get; } = EveryDay;

    public async Task Run()
    {
        var jobs = await Partitions.Get<JobData>().Scan();
        foreach (var job in jobs)
        {
            if (DateTime.UtcNow - job.Data.CreatedAt > ExpiredJob)
            {
                var partition = Partitions.Get<OperationData>($"{nameof(JobData)}{job.Id}");
                var operations = await partition.Scan();
                foreach (var operation in operations)
                {
                    await partition.Remove(operation.Id);
                }
                await Partitions.Get<JobData>().Remove(job.Id);
            }
        }
    }
}