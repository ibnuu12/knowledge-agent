using KnowledgeAgent.Application;

namespace KnowledgeAgent.Application.Agents;

public sealed class AgentContextAssembler(
    IProjectRepository repository) : IAgentContextAssembler
{
    public async Task<AgentContext> AssembleAsync(
        AgentMessage message,
        CancellationToken cancellationToken)
    {
        var project = await repository.GetAsync(
            message.ProjectId,
            cancellationToken);

        if (project is null)
        {
            throw new KeyNotFoundException(
                $"Project '{message.ProjectId}' was not found.");
        }

        return new AgentContext(
            ProjectId: project.Id,
            ProjectName: project.Name,
            Objective: project.Objective,

            Requirements:
            project.Requirements
                .Select(requirement =>
                    new AgentRequirementContext(
                        requirement.Id,
                        requirement.Statement,
                        requirement.Status))
                .ToArray(),

            Decisions:
            project.Decisions
                .Select(decision =>
                    new AgentDecisionContext(
                        decision.Id,
                        decision.Topic,
                        decision.DecisionText,
                        decision.Status,
                        decision.EvidenceSummary))
                .ToArray(),

            Assumptions:
            project.Assumptions
                .Select(assumption =>
                    new AgentAssumptionContext(
                        assumption.Id,
                        assumption.Statement,
                        assumption.RiskLevel,
                        assumption.Validated))
                .ToArray(),

            Evidence:
            [
                new AgentEvidence(
                    Source: "IProjectRepository",
                    Description:
                        "Project state was read from the project repository.")
            ]);
    }
}