using Staticsoft.Contracts.Abstractions;
using Staticsoft.Flow;

namespace Staticsoft.TestServer;

public class UpdateEmailEndpoint(
    Job<UpdateEmailJobInput, UpdateEmailJobOutput> updateEmail
) : HttpEndpoint<UpdateEmailRequest, UpdateEmailResponse>
{
    readonly Job<UpdateEmailJobInput, UpdateEmailJobOutput> UpdateEmail = updateEmail;

    public async Task<UpdateEmailResponse> Execute(UpdateEmailRequest request)
    {
        var jobId = await UpdateEmail.Create(new() { UserId = request.UserId, NewEmail = request.NewEmail });

        return new() { JobId = jobId };
    }
}
