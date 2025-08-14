using Staticsoft.Flow;
using Staticsoft.PartitionedStorage.Abstractions;

namespace Staticsoft.TestServer;

public class UpdateEmailOperation(
    Partitions partitions
) : Operation<UpdateEmailOperationInput, UpdateEmailOperationOutput>
{
    readonly Partition<User> Users = partitions.Get<User>();

    public async Task<UpdateEmailOperationOutput> Execute(UpdateEmailOperationInput input)
    {
        var user = await Users.Get(input.UserId);

        var updated = user.Data with { Email = input.NewEmail };
        await Users.Save(user.Id, updated, user.Version);

        return new();
    }
}