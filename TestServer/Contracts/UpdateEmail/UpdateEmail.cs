using Staticsoft.Contracts.Abstractions;
using Staticsoft.Flow;
using HttpMethod = Staticsoft.HttpCommunication.Abstractions.HttpMethod;

namespace Staticsoft.TestServer;

public class UpdateEmail(
    HttpEndpoint<UpdateEmailRequest, UpdateEmailResponse> update,
    HttpEndpoint<ConfirmRequest, ConfirmResponse> confirm,
    HttpEndpoint<GetUpdateStatus, JobStatus> getUpdateStatus
)
{
    [Endpoint(HttpMethod.Post)]
    public HttpEndpoint<UpdateEmailRequest, UpdateEmailResponse> Update { get; } = update;

    [Endpoint(HttpMethod.Post)]
    public HttpEndpoint<ConfirmRequest, ConfirmResponse> Confirm { get; } = confirm;

    [Endpoint(HttpMethod.Post)]
    public HttpEndpoint<GetUpdateStatus, JobStatus> GetDecisionStatus { get; } = getUpdateStatus;
}
