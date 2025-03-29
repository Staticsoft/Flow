namespace Staticsoft.TestServer;

public class Api(
    SumOfSquares sumOfSquares,
    Decisions decisions
)
{
    public SumOfSquares SumOfSquares { get; } = sumOfSquares;
    public Decisions Decisions { get; } = decisions;
}
