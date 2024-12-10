using Staticsoft.Flow;

namespace Staticsoft.TestServer;

public class CalculateSquareOperation : Operation<CalculateSquareInput, CalculateSquareOutput>
{
    public Task<CalculateSquareOutput> Execute(CalculateSquareInput input)
    {
        var squared = input.Number * input.Number;
        return Task.FromResult(new CalculateSquareOutput() { Squared = squared });
    }
}