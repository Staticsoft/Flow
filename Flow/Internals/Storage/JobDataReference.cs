namespace Staticsoft.Flow.Internals;

class JobDataReference : DataReference
{
    public JobDataReference()
        => Type = DataReferenceType.Job;

    public required string JobId { get; init; }
}
