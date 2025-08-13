namespace Staticsoft.TestServer;

public class UpdateEmailJobInput
{
    public required string UserId { get; init; }
    public required string NewEmail { get; init; }
}
