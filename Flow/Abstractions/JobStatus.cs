namespace Staticsoft.Flow;

public class JobStatus
{
    public required bool IsCompleted { get; init; }
    public required int TotalSteps { get; init; }
    public required int CompletedSteps { get; init; }
}