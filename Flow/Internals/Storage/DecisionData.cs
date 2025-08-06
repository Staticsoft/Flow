using Staticsoft.Flow.State;

namespace Staticsoft.Flow.Internals;

record DecisionData : DecisionRecord
{
    public string JobId { get; init; } = string.Empty;
    public string Choice { get; init; } = string.Empty;
}
