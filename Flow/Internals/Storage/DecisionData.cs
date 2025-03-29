namespace Staticsoft.Flow.Internals;

record DecisionData
{
    public string JobId { get; init; } = string.Empty;
    public string Choice { get; init; } = string.Empty;
}