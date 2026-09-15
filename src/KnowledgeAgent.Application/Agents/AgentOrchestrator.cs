namespace KnowledgeAgent.Application.Agents;

public sealed class AgentOrchestrator(
    IAgentContextAssembler contextAssembler) : IAgentOrchestrator
{
    public async Task<AgentResult> ProcessAsync(
        AgentMessage message,
        CancellationToken cancellationToken)
    {
        var context = await contextAssembler.AssembleAsync(
            message,
            cancellationToken);

        var state =
            $"Project={context.ProjectName}; " +
            $"Requirements={context.Requirements.Count}; " +
            $"Decisions={context.Decisions.Count}; " +
            $"Assumptions={context.Assumptions.Count}.";

        return new AgentResult(
            Response:
                "Agent runtime boundary is active. " +
                "LLM execution is not enabled; " +
                "no implementation or verification action is claimed.",

            CurrentStateSummary: state,

            Evidence:
                context.Evidence
                    .Select(evidence => evidence.Description)
                    .ToArray(),

            OpenQuestions:
            [
                "Which LLM provider/model should be configured " +
                "for the first real agent session?"
            ],

            ProposedActions: [],

            RequiresAuthorization: false);
    }
}