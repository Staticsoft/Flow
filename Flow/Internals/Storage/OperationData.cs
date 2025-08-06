using Staticsoft.Flow.State;

namespace Staticsoft.Flow.Internals;

record OperationData : OperationRecord
{
    public string Handler { get; init; } = string.Empty;
    public string Input { get; init; } = string.Empty;
    public string Output { get; init; } = string.Empty;
    public bool IsComplete { get; init; } = false;
}