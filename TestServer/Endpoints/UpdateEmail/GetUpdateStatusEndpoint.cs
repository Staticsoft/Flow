using Staticsoft.Contracts.Abstractions;
using Staticsoft.Flow;

namespace Staticsoft.TestServer;

public class GetUpdateStatusEndpoint(
    Job<UpdateEmailJobInput, UpdateEmailJobOutput> job
) : HttpEndpoint<GetUpdateStatus, JobStatus>
{
    readonly Job<UpdateEmailJobInput, UpdateEmailJobOutput> Job = job;

    public Task<JobStatus> Execute(GetUpdateStatus request)
        => Job.GetStatus(request.JobId);
}
