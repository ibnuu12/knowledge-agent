namespace KnowledgeAgent.Application.Agents;

public sealed class AgentResponseComposer : IAgentResponseComposer
{
    private readonly AgentInquiryClassifier classifier = new();

    public AgentResult Compose(
        AgentMessage message,
        AgentContext context)
    {
        var inquiryType = classifier.Classify(message.Message);

        return inquiryType switch
        {
            AgentInquiryType.CurrentState =>
                ComposeCurrentState(context),

            AgentInquiryType.Requirements =>
                ComposeRequirements(context),

            AgentInquiryType.Decisions =>
                ComposeDecisions(context),

            AgentInquiryType.Assumptions =>
                ComposeAssumptions(context),

            _ =>
                ComposeUnknownInquiry(context)
        };
    }

    private static AgentResult ComposeCurrentState(
        AgentContext context)
    {
        var state =
            $"Project={context.ProjectName}; " +
            $"Requirements={context.Requirements.Count}; " +
            $"Decisions={context.Decisions.Count}; " +
            $"Assumptions={context.Assumptions.Count}.";

        return CreateResult(
            context,
            "Current project state has been assembled " +
            "from the available project context.",
            state);
    }

    private static AgentResult ComposeRequirements(
        AgentContext context)
    {
        var response = context.Requirements.Count == 0
            ? "No requirements are currently available in the project context."
            : string.Join(
                Environment.NewLine,
                context.Requirements.Select(
                    requirement =>
                        $"- [{requirement.Status}] {requirement.Statement}"));

        return CreateResult(
            context,
            response);
    }

    private static AgentResult ComposeDecisions(
        AgentContext context)
    {
        var response = context.Decisions.Count == 0
            ? "No decisions are currently available in the project context."
            : string.Join(
                Environment.NewLine,
                context.Decisions.Select(
                    decision =>
                        $"- [{decision.Status}] " +
                        $"{decision.Topic}: {decision.DecisionText}"));

        return CreateResult(
            context,
            response);
    }

    private static AgentResult ComposeAssumptions(
        AgentContext context)
    {
        var response = context.Assumptions.Count == 0
            ? "No assumptions are currently available in the project context."
            : string.Join(
                Environment.NewLine,
                context.Assumptions.Select(
                    assumption =>
                        $"- [{assumption.RiskLevel}] " +
                        $"{assumption.Statement} " +
                        $"(Validated={assumption.Validated})"));

        return CreateResult(
            context,
            response);
    }

    private static AgentResult ComposeUnknownInquiry(
        AgentContext context)
    {
        return CreateResult(
            context,
            "The requested information is not available " +
            "from the current deterministic orchestration boundary.",
            openQuestions:
            [
                "What project information should be retrieved " +
                "for this inquiry?"
            ]);
    }

    private static AgentResult CreateResult(
        AgentContext context,
        string response,
        string? state = null,
        IReadOnlyList<string>? openQuestions = null)
    {
        var currentStateSummary =
            state ??
            $"Project={context.ProjectName}; " +
            $"Requirements={context.Requirements.Count}; " +
            $"Decisions={context.Decisions.Count}; " +
            $"Assumptions={context.Assumptions.Count}.";

        return new AgentResult(
            Response: response,
            CurrentStateSummary: currentStateSummary,
            Evidence:
                context.Evidence
                    .Select(evidence => evidence.Description)
                    .ToArray(),
            OpenQuestions: openQuestions ?? [],
            ProposedActions: [],
            RequiresAuthorization: false);
    }
}