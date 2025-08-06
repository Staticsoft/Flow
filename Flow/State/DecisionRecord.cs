namespace Staticsoft.Flow.State;

public interface DecisionRecord
{
    string JobId { get; }
    string Choice { get; }
}