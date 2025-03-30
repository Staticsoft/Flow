namespace Staticsoft.Flow.Internals;

record JobOperationsData
{
    public int Completed { get; init; } = 0;
    public int Total { get; init; } = 0;
}