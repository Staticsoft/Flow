using Staticsoft.Contracts.Abstractions;
using Staticsoft.Flow;

namespace Staticsoft.TestServer;

public class GetCalculationResultEndpoint(
    Job<CalculateSumOfSquaresInput, CalculateSumOfSquaresOutput> job
) : HttpEndpoint<GetCalculationResultRequest, GetCalculationResultResponse>
{
    readonly Job<CalculateSumOfSquaresInput, CalculateSumOfSquaresOutput> Job = job;

    public async Task<GetCalculationResultResponse> Execute(GetCalculationResultRequest request)
    {
        var jobResult = await Job.GetResult(request.JobId);
        return new() { Sum = jobResult.Sum };
    }
}
