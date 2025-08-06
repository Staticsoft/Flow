namespace Staticsoft.Flow.State;

public interface JobOperationsRecord
{
    int Completed { get; }
    int Total { get; }
}