namespace Staticsoft.Flow.Handlers;

public interface JobHandler : Handler { }

public interface JobHandler<Input, Output> : JobHandler, Handler<Input, Output>
{
    Task<string> Create(Input input)
        => throw new InvalidOperationException();

    Task<Output> GetResult(string jobId)
        => throw new InvalidOperationException();

    Task<JobStatus> GetStatus(string jobId)
        => throw new InvalidOperationException();
}