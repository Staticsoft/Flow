using Staticsoft.Flow;
using Staticsoft.PartitionedStorage.Abstractions;

namespace Staticsoft.TestServer;

public class RecordInputOperation(
    Partitions partitions
) : Operation<RecordInputIdInput, RecordInputIdOutput>
{
    readonly Partition<ExternalInputs> Inputs = partitions.Get<ExternalInputs>();

    public async Task<RecordInputIdOutput> Execute(RecordInputIdInput input)
    {
        var inputs = await Inputs.Get("Ids");

        await Inputs.Save("Ids", inputs.Data with { Ids = [.. inputs.Data.Ids, input.InputId] }, inputs.Version);

        return new();
    }
}
