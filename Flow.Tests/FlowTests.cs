using Staticsoft.PartitionedStorage.Abstractions;
using Staticsoft.TestServer;
using Xunit;

namespace Staticsoft.Flow.Tests;

public abstract class FlowTests<Startup> : TestBase<Startup>
    where Startup : class
{
    Partition<ExternalInputs> ExternalInputs
        => Server<Partitions>().Get<ExternalInputs>();

    Partition<User> Users
        => Server<Partitions>().Get<User>();

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
    public async Task RunsJobWithExternalInput()
    {
        var inputs = new ExternalInputs() { Ids = [] };
        await ExternalInputs.Save("Ids", inputs);

        await Users.Save("User", new() { Email = "old@email.com" });

        var updateEmailResponse = await Api.UpdateEmail.Update.Execute(new()
        {
            UserId = "User",
            NewEmail = "new@email.com"
        });
        var jobId = updateEmailResponse.JobId;

        while (inputs.Ids.Length == 0)
        {
            var item = await ExternalInputs.Get("Ids");
            inputs = item.Data;
            if (inputs.Ids.Length == 0)
            {
                await Task.Delay(1000);
            }
        }

        await Api.UpdateEmail.Confirm.Execute(new()
        {
            ExternalInputId = inputs.Ids.Single(),
            Confirm = true
        });

        var isCompleted = false;
        while (!isCompleted)
        {
            var statusResponse = await Api.UpdateEmail.GetConfirmationStatus.Execute(new() { JobId = jobId });
            isCompleted = statusResponse.IsCompleted;
            if (!isCompleted)
            {
                await Task.Delay(1000);
            }
        }

        var user = await Users.Get("User");
        Assert.Equal("new@email.com", user.Data.Email);
    }
}
