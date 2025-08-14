namespace Staticsoft.TestServer;

public class Api(
    SumOfSquares sumOfSquares,
    UpdateEmail updateEmail
)
{
    public SumOfSquares SumOfSquares { get; } = sumOfSquares;
    public UpdateEmail UpdateEmail { get; } = updateEmail;
}
