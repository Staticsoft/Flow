using Staticsoft.Contracts.Abstractions;
using Staticsoft.Flow;

namespace Staticsoft.TestServer;

public class ConfirmEndpoint(
    ExternalInput input
) : HttpEndpoint<ConfirmRequest, ConfirmResponse>
{
    readonly ExternalInput Input = input;

    public async Task<ConfirmResponse> Execute(ConfirmRequest request)
    {
        await Input.Provide(request.ExternalInputId, new ConfirmExternalInput { Confirm = request.Confirm });

        return new();
    }
}