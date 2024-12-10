using Staticsoft.TestServer;
using Staticsoft.TestServer.Local;
using Xunit;

namespace Staticsoft.Flow.Tests;

//public abstract class FlowTests<Startup> : TestBase<Startup>
//    where Startup : class
//{
public class FlowTests : TestBase<LocalStartup>
{
    [Fact]
    public async Task RunsJobUntilCompletion()
    {
        var numbers = Enumerable.Range(0, 100).ToArray();
        var calculateSumResponse = await Api.CalculateSum.Execute(new() { Numbers = numbers });
        var jobId = calculateSumResponse.JobId;

        var isCompleted = false;
        while (!isCompleted)
        {
            var statusResponse = await Api.GetCalculationStatus.Execute(new() { JobId = jobId });
            isCompleted = statusResponse.IsCompleted;
        }

        var jobResult = await Api.GetCalculationResult.Execute(new() { JobId = jobId });

        var expectedSum = numbers.Sum(number => number * number);
        var actualSum = jobResult.Sum;
        Assert.Equal(expectedSum, actualSum);
    }
}
