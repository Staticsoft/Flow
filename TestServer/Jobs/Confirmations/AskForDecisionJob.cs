using Staticsoft.Flow;

namespace Staticsoft.TestServer;

public class AskForDecisionJob(
    Operation<RecordDecisionInput, RecordDecisionOutput> recordDecision,
    Decision decision
) : Job<AskForDecisionInput, AskForDecisionOutput>
{
    readonly Operation<RecordDecisionInput, RecordDecisionOutput> RecordDecision = recordDecision;
    readonly Decision Decision = decision;

    public async Task<AskForDecisionOutput> Execute(AskForDecisionInput input)
    {
        await RecordDecision.Execute(new() { DecisionId = input.DeterministicId });

        var choice = await Decision.Create(input.DeterministicId);

        return new() { Choice = choice };
    }
}
