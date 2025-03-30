namespace Staticsoft.Flow.Internals;

record JobData
{
    public DateTime CreatedAt { get; init; }
    public string Handler { get; init; } = string.Empty;
    public string Input { get; init; } = string.Empty;
    public string Output { get; init; } = string.Empty;
    public bool IsComplete { get; init; } = false;
    public JobOperationsData Operations { get; init; } = new();
}
