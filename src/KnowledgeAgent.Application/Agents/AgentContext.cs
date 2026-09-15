namespace KnowledgeAgent.Application.Agents;

public sealed record AgentContext(
    Guid ProjectId,
    string ProjectName,
    string? Objective,
    IReadOnlyList<AgentRequirementContext> Requirements,
    IReadOnlyList<AgentDecisionContext> Decisions,
    IReadOnlyList<AgentAssumptionContext> Assumptions,
    IReadOnlyList<AgentEvidence> Evidence);

public sealed record AgentRequirementContext(
    Guid Id,
    string Statement,
    string Status);

public sealed record AgentDecisionContext(
    Guid Id,
    string Topic,
    string DecisionText,
    string Status,
    string? EvidenceSummary);

public sealed record AgentAssumptionContext(
    Guid Id,
    string Statement,
    string RiskLevel,
    bool Validated);

public sealed record AgentEvidence(
    string Source,
    string Description);