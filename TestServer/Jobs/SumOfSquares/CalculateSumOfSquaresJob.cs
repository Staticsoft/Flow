using Staticsoft.Flow;

namespace Staticsoft.TestServer;

public class CalculateSumOfSquaresJob(
    Operation<CalculateSquareInput, CalculateSquareOutput> calculateSquare
) : Job<CalculateSumOfSquaresInput, CalculateSumOfSquaresOutput>
{
    readonly Operation<CalculateSquareInput, CalculateSquareOutput> CalculateSquare = calculateSquare;

    public async Task<CalculateSumOfSquaresOutput> Execute(CalculateSumOfSquaresInput input)
    {
        var operations = input.Numbers.Select(number => CalculateSquare.Execute(new() { Number = number }));
        var results = await Task.WhenAll(operations);

        return new() { Sum = results.Sum(result => result.Squared) };
    }
}
