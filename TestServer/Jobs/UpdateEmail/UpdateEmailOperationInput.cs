namespace Staticsoft.TestServer;

public class UpdateEmailOperationInput
{
    public required string UserId { get; init; }
    public required string NewEmail { get; init; }
}
