namespace Staticsoft.TestServer;

public class UpdateEmailRequest
{
    public required string UserId { get; init; }
    public required string NewEmail { get; init; }
}
