using Staticsoft.Flow;

namespace Staticsoft.TestServer;

public class CalculateSquareOperation : Operation<CalculateSquareInput, CalculateSquareOutput>
{
    public async Task<CalculateSquareOutput> Execute(CalculateSquareInput input)
    {
        var squared = input.Number * input.Number;
        await Task.Delay(45_000);
        return new CalculateSquareOutput() { Squared = squared };
    }
}