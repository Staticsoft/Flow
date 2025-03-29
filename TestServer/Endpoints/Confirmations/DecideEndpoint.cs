using Staticsoft.Contracts.Abstractions;
using Staticsoft.Flow;

namespace Staticsoft.TestServer;

public class DecideEndpoint(
    Decision decision
) : HttpEndpoint<DecideRequest, DecideResponse>
{
    readonly Decision Decision = decision;

    public async Task<DecideResponse> Execute(DecideRequest request)
    {
        await Decision.Make(request.DecisionId, request.Choice);
        return new();
    }
}
