using Staticsoft.Flow.State;

namespace Staticsoft.Flow.Internals;

record JobOperationsData : JobOperationsRecord
{
    public int Completed { get; init; } = 0;
    public int Total { get; init; } = 0;
}
