namespace Staticsoft.Flow.State;

public interface OperationRecord
{
    string Handler { get; }
    string Input { get; }
    string Output { get; }
    bool IsComplete { get; }
}