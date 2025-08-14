namespace Staticsoft.TestServer;

public class ConfirmRequest
{
    public required string ExternalInputId { get; init; }
    public required bool Confirm { get; init; }
}
