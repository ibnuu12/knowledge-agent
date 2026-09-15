using KnowledgeAgent.Application;
using KnowledgeAgent.Domain;
using Microsoft.EntityFrameworkCore;

namespace KnowledgeAgent.Infrastructure;

public sealed class EfProjectRepository(KnowledgeAgentDbContext db) : IProjectRepository
{
    public Task<Project?> GetAsync(Guid projectId, CancellationToken cancellationToken) =>
        db.Projects
            .Include(x => x.Requirements)
            .Include(x => x.Decisions)
            .Include(x => x.Assumptions)
            .SingleOrDefaultAsync(x => x.Id == projectId, cancellationToken);

    public async Task AddAsync(Project project, CancellationToken cancellationToken)
    {
        db.Projects.Add(project);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task SaveAsync(Project project, CancellationToken cancellationToken) =>
        await db.SaveChangesAsync(cancellationToken);

    public async Task AddRequirementAsync(Requirement requirement, CancellationToken cancellationToken)
    {
        db.Requirements.Add(requirement);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task AddDecisionAsync(Decision decision, CancellationToken cancellationToken)
    {
        db.Decisions.Add(decision);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task AddAssumptionAsync(Assumption assumption, CancellationToken cancellationToken)
    {
        db.Assumptions.Add(assumption);
        await db.SaveChangesAsync(cancellationToken);
    }
}
