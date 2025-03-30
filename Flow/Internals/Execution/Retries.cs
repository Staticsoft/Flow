using Polly;
using Polly.Retry;
using Staticsoft.PartitionedStorage.Abstractions;

namespace Staticsoft.Flow.Internals;

static class Retries
{
    const int MaxRetries = 100;

    public static Task Storage(Func<Task> task)
        => StoragePolicy.ExecuteAsync(task);

    public static Task<T> Storage<T>(Func<Task<T>> task)
        => StoragePolicy.ExecuteAsync(task);

    static readonly AsyncRetryPolicy StoragePolicy = Policy
        .Handle<PartitionedStorageItemVersionMismatchException>()
        .WaitAndRetryAsync(RandomSmallInterval);

    static IEnumerable<TimeSpan> RandomSmallInterval
        => Enumerable
            .Range(0, MaxRetries)
            .Select(i => TimeSpan.FromSeconds(Random.Shared.NextDouble() * Math.Sqrt(i)));
}
