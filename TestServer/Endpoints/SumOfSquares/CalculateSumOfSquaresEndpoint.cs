using Staticsoft.Contracts.Abstractions;
using Staticsoft.Flow;

namespace Staticsoft.TestServer;

public class CalculateSumOfSquaresEndpoint(
    Job<CalculateSumOfSquaresInput, CalculateSumOfSquaresOutput> job
) : HttpEndpoint<CalculateSumOfSquaresRequest, CalculateSumOfSquaresResponse>
{
    readonly Job<CalculateSumOfSquaresInput, CalculateSumOfSquaresOutput> Job = job;

    public async Task<CalculateSumOfSquaresResponse> Execute(CalculateSumOfSquaresRequest request)
    {
        var jobId = await Job.Create(new() { Numbers = request.Numbers });
        return new() { JobId = jobId };
    }
}
