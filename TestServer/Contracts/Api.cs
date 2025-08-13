namespace Staticsoft.TestServer;

public class Api(
    SumOfSquares sumOfSquares,
    UpdateEmail decisions
)
{
    public SumOfSquares SumOfSquares { get; } = sumOfSquares;
    public UpdateEmail UpdateEmail { get; } = decisions;
}
