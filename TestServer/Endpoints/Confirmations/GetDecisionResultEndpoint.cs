using Staticsoft.Contracts.Abstractions;
using Staticsoft.Flow;

namespace Staticsoft.TestServer;

public class GetDecisionResultEndpoint(
    Job<AskForDecisionInput, AskForDecisionOutput> job
) : HttpEndpoint<GetChoiceRequest, GetChoiceResponse>
{
    readonly Job<AskForDecisionInput, AskForDecisionOutput> Job = job;

    public async Task<GetChoiceResponse> Execute(GetChoiceRequest request)
    {
        var result = await Job.GetResult(request.JobId);
        return new() { Choice = result.Choice };
    }
}