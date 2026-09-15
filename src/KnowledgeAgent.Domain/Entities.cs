namespace KnowledgeAgent.Domain;

public sealed class Project
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Name { get; set; }
    public string? Objective { get; set; }
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    public List<Requirement> Requirements { get; init; } = [];
    public List<Decision> Decisions { get; init; } = [];
    public List<Assumption> Assumptions { get; init; } = [];
}

public sealed class Requirement
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid ProjectId { get; init; }
    public required string Statement { get; set; }
    public string Status { get; set; } = "PROPOSED";
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
}

public sealed class Decision
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid ProjectId { get; init; }
    public required string Topic { get; set; }
    public required string DecisionText { get; set; }
    public string Status { get; set; } = "ACCEPTED";
    public string? EvidenceSummary { get; set; }
    public string? AuthorizationNote { get; set; }
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? SupersededAt { get; set; }
}

public sealed class Assumption
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid ProjectId { get; init; }
    public required string Statement { get; set; }
    public string RiskLevel { get; set; } = "LOW";
    public bool Validated { get; set; }
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
}
