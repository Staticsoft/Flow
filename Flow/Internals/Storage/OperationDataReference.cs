namespace Staticsoft.Flow.Internals;

class OperationDataReference : DataReference
{
    public OperationDataReference()
        => Type = DataReferenceType.Operation;

    public required string JobId { get; init; }
    public required string OperationId { get; init; }
}
