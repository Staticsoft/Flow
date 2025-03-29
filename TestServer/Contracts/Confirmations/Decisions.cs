using Staticsoft.Contracts.Abstractions;
using Staticsoft.Flow;
using HttpMethod = Staticsoft.HttpCommunication.Abstractions.HttpMethod;

namespace Staticsoft.TestServer;

public class Decisions(
    HttpEndpoint<EmptyRequest, AskForDecisionResponse> askForDecision,
    HttpEndpoint<DecideRequest, DecideResponse> decide,
    HttpEndpoint<GetDecisionStatusRequest, JobStatus> getDecisionStatus,
    HttpEndpoint<GetChoiceRequest, GetChoiceResponse> getChoice
)
{
    [Endpoint(HttpMethod.Post)]
    public HttpEndpoint<EmptyRequest, AskForDecisionResponse> AskForDecision { get; } = askForDecision;

    [Endpoint(HttpMethod.Post)]
    public HttpEndpoint<DecideRequest, DecideResponse> Decide { get; } = decide;

    [Endpoint(HttpMethod.Post)]
    public HttpEndpoint<GetDecisionStatusRequest, JobStatus> GetDecisionStatus { get; } = getDecisionStatus;

    [Endpoint(HttpMethod.Post)]
    public HttpEndpoint<GetChoiceRequest, GetChoiceResponse> GetChoice { get; } = getChoice;
}
