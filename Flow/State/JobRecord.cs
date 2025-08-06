namespace Staticsoft.Flow.State;

public interface JobRecord
{
    DateTime CreatedAt { get; }
    string Handler { get; }
    string Input { get; }
    string Output { get; }
    bool IsComplete { get; }
    JobOperationsRecord Operations { get; }
}
