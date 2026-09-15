using KnowledgeAgent.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace KnowledgeAgent.Application
{
    public interface IProjectRepository
    {
        Task<Project?> GetAsync(Guid projectId, CancellationToken cancellationToken);
        Task AddAsync(Project project, CancellationToken cancellationToken);
        Task SaveAsync(Project project, CancellationToken cancellationToken);
        Task AddRequirementAsync(Requirement requirement, CancellationToken cancellationToken);
        Task AddDecisionAsync(Decision decision,CancellationToken cancellationToken);
        Task AddAssumptionAsync(Assumption assumption,CancellationToken cancellationToken);
    }
}
