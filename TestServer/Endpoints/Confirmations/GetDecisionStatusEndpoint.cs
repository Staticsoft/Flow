using Staticsoft.Contracts.Abstractions;
using Staticsoft.Flow;

namespace Staticsoft.TestServer;

public class GetDecisionStatusEndpoint(
    Job<AskForDecisionInput, AskForDecisionOutput> job
) : HttpEndpoint<GetDecisionStatusRequest, JobStatus>
{
    readonly Job<AskForDecisionInput, AskForDecisionOutput> Job = job;

    public Task<JobStatus> Execute(GetDecisionStatusRequest request)
        => Job.GetStatus(request.JobId);
}
