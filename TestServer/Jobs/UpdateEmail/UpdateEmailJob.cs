using Staticsoft.Flow;

namespace Staticsoft.TestServer;

public class UpdateEmailJob(
    Operation<RecordInputIdInput, RecordInputIdOutput> recordInput,
    ExternalInput<ConfirmExternalInputSeed, ConfirmExternalInput> externalInput,
    Operation<UpdateEmailOperationInput, UpdateEmailOperationOutput> updateEmail
) : Job<UpdateEmailJobInput, UpdateEmailJobOutput>
{
    readonly Operation<RecordInputIdInput, RecordInputIdOutput> RecordInput = recordInput;
    readonly ExternalInput<ConfirmExternalInputSeed, ConfirmExternalInput> ExternalInput = externalInput;
    readonly Operation<UpdateEmailOperationInput, UpdateEmailOperationOutput> UpdateEmail = updateEmail;

    public async Task<UpdateEmailJobOutput> Execute(UpdateEmailJobInput input)
    {
        var externalInput = await ExternalInput.Create(new()
        {
            UserId = input.UserId,
            NewEmail = input.NewEmail
        });

        await RecordInput.Execute(new() { InputId = externalInput.Id });

        var providedInput = externalInput.Input;
        if (providedInput.Confirm)
        {
            await UpdateEmail.Execute(new() { UserId = input.UserId, NewEmail = input.NewEmail });
        }

        return new() { };
    }
}
