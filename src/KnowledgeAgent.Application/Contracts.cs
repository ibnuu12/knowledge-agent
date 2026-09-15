using KnowledgeAgent.Domain;

namespace KnowledgeAgent.Application;

public sealed record CreateProjectCommand(string Name, string? Objective);
public sealed record AddRequirementCommand(string Statement);
public sealed record RecordDecisionCommand(string Topic, string DecisionText, string? EvidenceSummary, string? AuthorizationNote);
public sealed record RecordAssumptionCommand(string Statement, string RiskLevel);

public sealed record AgentMessageRequest(string Message);