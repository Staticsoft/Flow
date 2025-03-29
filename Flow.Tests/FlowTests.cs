using Staticsoft.Contracts.Abstractions;
using Staticsoft.PartitionedStorage.Abstractions;
using Staticsoft.TestServer;
using Xunit;

namespace Staticsoft.Flow.Tests;

public abstract class FlowTests<Startup> : TestBase<Startup>
    where Startup : class
{
    Partition<UserDecision> Decisions
        => Server<Partitions>().Get<UserDecision>();

    [Fact]
    public async Task RunsJobUntilCompletion()
    {
        var numbers = Enumerable.Range(0, 100).ToArray();
        var calculateSumResponse = await Api.SumOfSquares.CalculateSum.Execute(new() { Numbers = numbers });
        var jobId = calculateSumResponse.JobId;

        var isCompleted = false;
        while (!isCompleted)
        {
            var statusResponse = await Api.SumOfSquares.GetCalculationStatus.Execute(new() { JobId = jobId });
            isCompleted = statusResponse.IsCompleted;
            if (!isCompleted)
            {
                await Task.Delay(1000);
            }
        }

        var jobResult = await Api.SumOfSquares.GetCalculationResult.Execute(new() { JobId = jobId });

        var expectedSum = numbers.Sum(number => number * number);
        var actualSum = jobResult.Sum;
        Assert.Equal(expectedSum, actualSum);
    }

    [Fact]
    public async Task RunsJobWithDecision()
    {
        var askForDecisionResponse = await Api.Decisions.AskForDecision.Execute();
        var jobId = askForDecisionResponse.JobId;

        var decisions = Array.Empty<Item<UserDecision>>();
        while (decisions.Length != 1)
        {
            decisions = await Decisions.Scan();
            if (decisions.Length != 1)
            {
                await Task.Delay(1000);
            }
        }
        var decision = decisions.Single();

        await Api.Decisions.Decide.Execute(new()
        {
            DecisionId = decision.Id,
            Choice = "Choice"
        });

        var isCompleted = false;
        while (!isCompleted)
        {
            var statusResponse = await Api.Decisions.GetDecisionStatus.Execute(new() { JobId = jobId });
            isCompleted = statusResponse.IsCompleted;
            if (!isCompleted)
            {
                await Task.Delay(1000);
            }
        }

        var response = await Api.Decisions.GetChoice.Execute(new() { JobId = jobId });
        Assert.Equal("Choice", response.Choice);
    }
}
