using KnowledgeAgent.Domain;

namespace KnowledgeAgent.Application;

public sealed class ProjectService(IProjectRepository repository)
{
    public async Task<Project> CreateAsync(CreateProjectCommand command, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
            throw new ArgumentException("Project name is required.", nameof(command));

        var project = new Project { Name = command.Name.Trim(), Objective = command.Objective?.Trim() };
        await repository.AddAsync(project, ct);
        return project;
    }

    public Task<Project?> GetAsync(Guid id, CancellationToken ct) => repository.GetAsync(id, ct);

    public async Task<Requirement> AddRequirementAsync(Guid projectId, AddRequirementCommand command, CancellationToken ct)
    {
        _ = await Require(projectId, ct);

        var requirement = new Requirement
        {
            ProjectId = projectId,
            Statement = command.Statement.Trim()
        };

        await repository.AddRequirementAsync(requirement, ct);

        return requirement;
    }

    public async Task<Decision> RecordDecisionAsync(Guid projectId, RecordDecisionCommand command, CancellationToken ct)
    {
        _ = await Require(projectId, ct);

        var decision = new Decision
        {
            ProjectId = projectId,
            Topic = command.Topic.Trim(),
            DecisionText = command.DecisionText.Trim(),
            EvidenceSummary = command.EvidenceSummary?.Trim(),
            AuthorizationNote = command.AuthorizationNote?.Trim()
        };

        await repository.AddDecisionAsync(decision, ct);

        return decision;
    }

    public async Task<Assumption> RecordAssumptionAsync(Guid projectId, RecordAssumptionCommand command, CancellationToken ct)
    {
        _ = await Require(projectId, ct);

        var assumption = new Assumption
        {
            ProjectId = projectId,
            Statement = command.Statement.Trim(),
            RiskLevel = command.RiskLevel.Trim().ToUpperInvariant()
        };

        await repository.AddAssumptionAsync(assumption, ct);

        return assumption;
    }

    private async Task<Project> Require(Guid id, CancellationToken ct) =>
        await repository.GetAsync(id, ct) ?? throw new KeyNotFoundException($"Project '{id}' was not found.");
}
