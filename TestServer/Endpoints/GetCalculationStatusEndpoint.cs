using Staticsoft.Contracts.Abstractions;
using Staticsoft.Flow;

namespace Staticsoft.TestServer;

public class GetCalculationStatusEndpoint(
    Job<CalculateSumOfSquaresInput, CalculateSumOfSquaresOutput> job
) : HttpEndpoint<GetCalculationStatusRequest, JobStatus>
{
    readonly Job<CalculateSumOfSquaresInput, CalculateSumOfSquaresOutput> Job = job;

    public Task<JobStatus> Execute(GetCalculationStatusRequest request)
        => Job.GetStatus(request.JobId);
}