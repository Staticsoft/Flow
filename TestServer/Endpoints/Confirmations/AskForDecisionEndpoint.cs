using Staticsoft.Contracts.Abstractions;
using Staticsoft.Flow;

namespace Staticsoft.TestServer;

public class AskForDecisionEndpoint(
    Job<AskForDecisionInput, AskForDecisionOutput> askForDecision
) : HttpEndpoint<EmptyRequest, AskForDecisionResponse>
{
    readonly Job<AskForDecisionInput, AskForDecisionOutput> AskForDecision = askForDecision;

    public async Task<AskForDecisionResponse> Execute(EmptyRequest request)
    {
        var deterministicId = $"{Guid.NewGuid()}";
        var jobId = await AskForDecision.Create(new() { DeterministicId = deterministicId });
        return new() { JobId = jobId };
    }
}
