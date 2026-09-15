using KnowledgeAgent.Application;
using KnowledgeAgent.Application.Agents;
using KnowledgeAgent.Domain;
using System.Collections;
using Xunit;

namespace KnowledgeAgent.Application.Tests;

public sealed class ProjectServiceTests
{
    [Fact]
    public async Task CreateProject_PersistsProjectThroughRepository()
    {
        var repository = new FakeProjectRepository();
        var service = new ProjectService(repository);

        var project = await service.CreateAsync(
            new CreateProjectCommand("Knowledge Agent", "Build v0.1"),
            CancellationToken.None);

        Assert.Equal("Knowledge Agent", project.Name);
        Assert.Equal("Build v0.1", project.Objective);

        var persisted = await repository.GetAsync(
            project.Id,
            CancellationToken.None);

        Assert.NotNull(persisted);
        Assert.Equal(project.Id, persisted.Id);
    }

    [Fact]
    public async Task CreateProject_TrimsNameAndObjective()
    {
        var repository = new FakeProjectRepository();
        var service = new ProjectService(repository);

        var project = await service.CreateAsync(
            new CreateProjectCommand(
                "  Knowledge Agent  ",
                "  Build v0.1  "),
            CancellationToken.None);

        Assert.Equal("Knowledge Agent", project.Name);
        Assert.Equal("Build v0.1", project.Objective);
    }

    [Fact]
    public async Task CreateProject_WithoutName_ThrowsArgumentException()
    {
        var repository = new FakeProjectRepository();
        var service = new ProjectService(repository);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CreateAsync(
                new CreateProjectCommand("   ", "Build v0.1"),
                CancellationToken.None));
    }

    [Fact]
    public async Task AddRequirement_AddsRequirementThroughRepository()
    {
        var repository = new FakeProjectRepository();
        var service = new ProjectService(repository);

        var project = await service.CreateAsync(
            new CreateProjectCommand("Knowledge Agent", null),
            CancellationToken.None);

        var requirement = await service.AddRequirementAsync(
            project.Id,
            new AddRequirementCommand(
                "The agent must preserve accepted engineering decisions."),
            CancellationToken.None);

        Assert.NotEqual(Guid.Empty, requirement.Id);
        Assert.Equal(project.Id, requirement.ProjectId);
        Assert.Equal(
            "The agent must preserve accepted engineering decisions.",
            requirement.Statement);
        Assert.Equal("PROPOSED", requirement.Status);

        Assert.Same(
            requirement,
            repository.AddedRequirement);
    }

    [Fact]
    public async Task AddRequirement_TrimsStatement()
    {
        var repository = new FakeProjectRepository();
        var service = new ProjectService(repository);

        var project = await service.CreateAsync(
            new CreateProjectCommand("Knowledge Agent", null),
            CancellationToken.None);

        var requirement = await service.AddRequirementAsync(
            project.Id,
            new AddRequirementCommand(
                "  Preserve accepted engineering decisions.  "),
            CancellationToken.None);

        Assert.Equal(
            "Preserve accepted engineering decisions.",
            requirement.Statement);
    }

    [Fact]
    public async Task AddRequirement_WhenProjectDoesNotExist_ThrowsKeyNotFoundException()
    {
        var repository = new FakeProjectRepository();
        var service = new ProjectService(repository);

        var projectId = Guid.NewGuid();

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.AddRequirementAsync(
                projectId,
                new AddRequirementCommand("A requirement"),
                CancellationToken.None));

        Assert.Null(repository.AddedRequirement);
    }

    [Fact]
    public async Task AddRequirement_DoesNotRequireParentGraphMutation()
    {
        var repository = new FakeProjectRepository();
        var service = new ProjectService(repository);

        var project = await service.CreateAsync(
            new CreateProjectCommand("Knowledge Agent", null),
            CancellationToken.None);

        var requirement = await service.AddRequirementAsync(
            project.Id,
            new AddRequirementCommand("A requirement"),
            CancellationToken.None);

        Assert.Empty(project.Requirements);
        Assert.Same(requirement, repository.AddedRequirement);
    }

    [Fact]
    public async Task RecordDecision_RecordsDecisionThroughRepository()
    {
        var repository = new FakeProjectRepository();
        var service = new ProjectService(repository);

        var project = await service.CreateAsync(
            new CreateProjectCommand("Knowledge Agent", null),
            CancellationToken.None);

        var decision = await service.RecordDecisionAsync(
            project.Id,
            new RecordDecisionCommand(
                "Persistence",
                "PostgreSQL is the initial state persistence store.",
                null,
                null),
            CancellationToken.None);

        Assert.NotEqual(Guid.Empty, decision.Id);
        Assert.Equal(project.Id, decision.ProjectId);
        Assert.Equal("Persistence", decision.Topic);
        Assert.Equal(
            "PostgreSQL is the initial state persistence store.",
            decision.DecisionText);
        Assert.Equal("ACCEPTED", decision.Status);

        Assert.Same(decision, repository.AddedDecision);
    }

    [Fact]
    public async Task RecordDecision_TrimsFields()
    {
        var repository = new FakeProjectRepository();
        var service = new ProjectService(repository);

        var project = await service.CreateAsync(
            new CreateProjectCommand("Knowledge Agent", null),
            CancellationToken.None);

        var decision = await service.RecordDecisionAsync(
            project.Id,
            new RecordDecisionCommand(
                "  Persistence  ",
                "  PostgreSQL is the initial state persistence store.  ",
                "  Verified against H2 requirements.  ",
                "  Approved by project owner.  "),
            CancellationToken.None);

        Assert.Equal("Persistence", decision.Topic);
        Assert.Equal(
            "PostgreSQL is the initial state persistence store.",
            decision.DecisionText);
        Assert.Equal(
            "Verified against H2 requirements.",
            decision.EvidenceSummary);
        Assert.Equal(
            "Approved by project owner.",
            decision.AuthorizationNote);
    }

    [Fact]
    public async Task RecordDecision_WhenProjectDoesNotExist_ThrowsKeyNotFoundException()
    {
        var repository = new FakeProjectRepository();
        var service = new ProjectService(repository);

        var projectId = Guid.NewGuid();

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.RecordDecisionAsync(
                projectId,
                new RecordDecisionCommand(
                    "Persistence",
                    "PostgreSQL is the initial state persistence store.",
                    null,
                    null),
                CancellationToken.None));

        Assert.Null(repository.AddedDecision);
    }
    [Fact]
    public async Task RecordDecision_PreservesEvidenceAndAuthorization()
    {
        var repository = new FakeProjectRepository();
        var service = new ProjectService(repository);

        var project = await service.CreateAsync(
            new CreateProjectCommand("Knowledge Agent", null),
            CancellationToken.None);

        var decision = await service.RecordDecisionAsync(
            project.Id,
            new RecordDecisionCommand(
                "Testing strategy",
                "Application behavior is covered with unit tests.",
                "H2 testing plan.",
                "Authorized for H2 implementation."),
            CancellationToken.None);

        Assert.Equal("H2 testing plan.", decision.EvidenceSummary);
        Assert.Equal(
            "Authorized for H2 implementation.",
            decision.AuthorizationNote);

        Assert.Same(decision, repository.AddedDecision);
    }

    [Fact]
    public async Task RecordAssumption_RecordsAssumptionThroughRepository()
    {
        var repository = new FakeProjectRepository();
        var service = new ProjectService(repository);


        var project = await service.CreateAsync(
            new CreateProjectCommand("Knowledge Agent", null),
            CancellationToken.None);

        var assumption = await service.RecordAssumptionAsync(
            project.Id,
            new RecordAssumptionCommand(
                "PostgreSQL is available in the development environment.",
                "LOW"),
            CancellationToken.None);

        Assert.NotEqual(Guid.Empty, assumption.Id);
        Assert.Equal(project.Id, assumption.ProjectId);
        Assert.Equal(
            "PostgreSQL is available in the development environment.",
            assumption.Statement);
        Assert.Equal("LOW", assumption.RiskLevel);
        Assert.False(assumption.Validated);

        Assert.Same(assumption, repository.AddedAssumption);


    }

    [Fact]
    public async Task RecordAssumption_TrimsStatementAndNormalizesRiskLevel()
    {
        var repository = new FakeProjectRepository();
        var service = new ProjectService(repository);


        var project = await service.CreateAsync(
            new CreateProjectCommand("Knowledge Agent", null),
            CancellationToken.None);

        var assumption = await service.RecordAssumptionAsync(
            project.Id,
            new RecordAssumptionCommand(
                "  PostgreSQL is available.  ",
                "  medium  "),
            CancellationToken.None);

        Assert.Equal(
            "PostgreSQL is available.",
            assumption.Statement);
        Assert.Equal("MEDIUM", assumption.RiskLevel);


    }

    [Fact]
    public async Task RecordAssumption_WhenProjectDoesNotExist_ThrowsKeyNotFoundException()
    {
        var repository = new FakeProjectRepository();
        var service = new ProjectService(repository);


        var projectId = Guid.NewGuid();

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.RecordAssumptionAsync(
                projectId,
                new RecordAssumptionCommand(
                    "PostgreSQL is available.",
                    "LOW"),
                CancellationToken.None));

        Assert.Null(repository.AddedAssumption);


    }

    [Fact]
    public async Task RecordAssumption_CreatesUnvalidatedAssumption()
    {
        var repository = new FakeProjectRepository();
        var service = new ProjectService(repository);


        var project = await service.CreateAsync(
            new CreateProjectCommand("Knowledge Agent", null),
            CancellationToken.None);

        var assumption = await service.RecordAssumptionAsync(
            project.Id,
            new RecordAssumptionCommand(
                "The development database is isolated from production.",
                "HIGH"),
            CancellationToken.None);

        Assert.False(assumption.Validated);
        Assert.Equal("HIGH", assumption.RiskLevel);
        Assert.Same(assumption, repository.AddedAssumption);

    }

    [Fact]
    public async Task AssembleAsync_ReturnsProjectContext()
    {
        var repository = new FakeProjectRepository();

        var project = new Project
        {
            Name = "Knowledge Agent",
            Objective = "Validate agent context assembly."
        };

        project.Requirements.Add(
            new Requirement
            {
                ProjectId = project.Id,
                Statement = "The agent must preserve accepted decisions."
            });

        project.Decisions.Add(
            new Decision
            {
                ProjectId = project.Id,
                Topic = "Persistence",
                DecisionText = "PostgreSQL is the state store.",
                Status = "ACCEPTED",
                EvidenceSummary = "H2 validation."
            });

        project.Assumptions.Add(
            new Assumption
            {
                ProjectId = project.Id,
                Statement = "PostgreSQL is available.",
                RiskLevel = "MEDIUM",
                Validated = false
            });

        await repository.AddAsync(
            project,
            CancellationToken.None);

        var assembler = new AgentContextAssembler(repository);

        var context = await assembler.AssembleAsync(
            new AgentMessage(project.Id, "What is the current state?"),
            CancellationToken.None);

        Assert.Equal(project.Id, context.ProjectId);
        Assert.Equal(project.Name, context.ProjectName);
        Assert.Equal(project.Objective, context.Objective);

        Assert.Single(context.Requirements);
        Assert.Equal(
            "The agent must preserve accepted decisions.",
            context.Requirements[0].Statement);

        Assert.Single(context.Decisions);
        Assert.Equal(
            "Persistence",
            context.Decisions[0].Topic);

        Assert.Single(context.Assumptions);
        Assert.Equal(
            "MEDIUM",
            context.Assumptions[0].RiskLevel);

        Assert.NotEmpty(context.Evidence);
    }

    [Fact]
    public async Task AssembleAsync_DoesNotCrossProjectBoundary()
    {
        var repository = new FakeProjectRepository();

        var projectA = new Project
        {
            Name = "Project A"
        };

        projectA.Requirements.Add(
            new Requirement
            {
                ProjectId = projectA.Id,
                Statement = "Requirement A"
            });

        var projectB = new Project
        {
            Name = "Project B"
        };

        projectB.Requirements.Add(
            new Requirement
            {
                ProjectId = projectB.Id,
                Statement = "Requirement B"
            });

        await repository.AddAsync(
            projectA,
            CancellationToken.None);

        await repository.AddAsync(
            projectB,
            CancellationToken.None);

        var assembler = new AgentContextAssembler(repository);

        var context = await assembler.AssembleAsync(
            new AgentMessage(projectA.Id, "Show requirements."),
            CancellationToken.None);

        var requirement = Assert.Single(context.Requirements);

        Assert.Equal("Requirement A", requirement.Statement);
        Assert.DoesNotContain(
            context.Requirements,
            item => item.Statement == "Requirement B");
    }

    [Fact]
    public async Task AssembleAsync_ThrowsWhenProjectDoesNotExist()
    {
        var repository = new FakeProjectRepository();
        var assembler = new AgentContextAssembler(repository);

        var projectId = Guid.NewGuid();

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => assembler.AssembleAsync(
                new AgentMessage(projectId, "Show state."),
                CancellationToken.None));

        Assert.Contains(
            projectId.ToString(),
            exception.Message);
    }

    [Fact]
    public void Compose_CurrentState_ReturnsDeterministicStateSummary()
    {
        var context = new AgentContext(
            ProjectId: Guid.NewGuid(),
            ProjectName: "Knowledge Agent",
            Objective: "Validate orchestration.",
            Requirements:
            [
                new AgentRequirementContext(
                Guid.NewGuid(),
                "Requirement A",
                "PROPOSED"),

            new AgentRequirementContext(
                Guid.NewGuid(),
                "Requirement B",
                "ACCEPTED")
            ],
            Decisions:
            [
                new AgentDecisionContext(
                Guid.NewGuid(),
                "Persistence",
                "PostgreSQL is the state store.",
                "ACCEPTED",
                "H2 validation.")
            ],
            Assumptions:
            [
                new AgentAssumptionContext(
                Guid.NewGuid(),
                "PostgreSQL is available.",
                "MEDIUM",
                false)
            ],
            Evidence:
            [
                new AgentEvidence(
                "IProjectRepository",
                "Project state was read from the project repository.")
            ]);

        var message = new AgentMessage(
            context.ProjectId,
            "What is the current state?");

        var composer = new AgentResponseComposer();
        var result = composer.Compose(message, context);

        Assert.Equal(
            "Project=Knowledge Agent; Requirements=2; Decisions=1; Assumptions=1.",
            result.CurrentStateSummary);

        Assert.Empty(result.ProposedActions);
        Assert.False(result.RequiresAuthorization);
        Assert.NotEmpty(result.Evidence);
    }

    [Fact]
    public void Compose_SameInput_ProducesSameResult()
    {
        var composer = new AgentResponseComposer();
        var context = CreateContext();

        var message = new AgentMessage(
            context.ProjectId,
            "What is the current state?");

        var first = composer.Compose(message, context);
        var second = composer.Compose(message, context);

        Assert.Equal(
            first.Response,
            second.Response);

        Assert.Equal(
            first.CurrentStateSummary,
            second.CurrentStateSummary);

        Assert.Equal(
            first.Evidence,
            second.Evidence);

        Assert.Equal(
            first.OpenQuestions,
            second.OpenQuestions);

        Assert.Equal(
            first.ProposedActions,
            second.ProposedActions);

        Assert.Equal(
            first.RequiresAuthorization,
            second.RequiresAuthorization);
    }

    [Fact]
    public void Compose_DoesNotMutateContext()
    {
        var context = CreateContext();

        var originalRequirements = context.Requirements;
        var originalDecisions = context.Decisions;
        var originalAssumptions = context.Assumptions;

        var composer = new AgentResponseComposer();
        composer.Compose(
            new AgentMessage(
                context.ProjectId,
                "What is the current state?"),
            context);

        Assert.Same(originalRequirements, context.Requirements);
        Assert.Same(originalDecisions, context.Decisions);
        Assert.Same(originalAssumptions, context.Assumptions);
    }

    [Fact]
    public void Compose_RequirementsInquiry_ReturnsRequirements()
    {
        var composer = new AgentResponseComposer();
        var context = CreateContext();

        var result = composer.Compose(
            new AgentMessage(
                context.ProjectId,
                "What are the requirements?"),
            context);

        Assert.Contains("Requirement A", result.Response);
        Assert.Contains("Requirement B", result.Response);
        Assert.Empty(result.OpenQuestions);
        Assert.Empty(result.ProposedActions);
        Assert.False(result.RequiresAuthorization);
    }

    [Fact]
    public void Compose_AssumptionsInquiry_ReturnsAssumptions()
    {
        var composer = new AgentResponseComposer();
        var context = CreateContext();

        var result = composer.Compose(
            new AgentMessage(
                context.ProjectId,
                "What assumptions exist?"),
            context);

        Assert.Contains(
            "PostgreSQL is available.",
            result.Response);

        Assert.Contains(
            "MEDIUM",
            result.Response);

        Assert.Contains(
            "Validated=False",
            result.Response);

        Assert.Empty(result.OpenQuestions);
        Assert.Empty(result.ProposedActions);
        Assert.False(result.RequiresAuthorization);
    }

    [Fact]
    public void Compose_UnknownInquiry_DoesNotInventInformation()
    {
        var composer = new AgentResponseComposer();
        var context = CreateContext();

        var result = composer.Compose(
            new AgentMessage(
                context.ProjectId,
                "What is the deployment status?"),
            context);

        Assert.Empty(result.ProposedActions);
        Assert.False(result.RequiresAuthorization);
        Assert.NotEmpty(result.OpenQuestions);

        Assert.DoesNotContain(
            "deployment",
            result.Response,
            StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Compose_RequirementsInquiry_WithNoRequirements_ReturnsExplicitEmptyState()
    {
        var composer = new AgentResponseComposer();

        var context = new AgentContext(
            Guid.NewGuid(),
            "Empty Project",
            null,
            [],
            [],
            [],
            [
                new AgentEvidence(
                "IProjectRepository",
                "Project state was read from the project repository.")
            ]);

        var result = composer.Compose(
            new AgentMessage(
                context.ProjectId,
                "Show requirements"),
            context);

        Assert.Contains(
            "No requirements",
            result.Response);

        Assert.Empty(result.OpenQuestions);
        Assert.Empty(result.ProposedActions);
        Assert.False(result.RequiresAuthorization);
    }


    private sealed class FakeProjectRepository : IProjectRepository
    {
        private readonly Dictionary<Guid, Project> _projects = [];

        public Requirement? AddedRequirement { get; private set; }

        public Decision? AddedDecision { get; private set; }

        public Assumption? AddedAssumption { get; private set; }

        public Task<Project?> GetAsync(
            Guid projectId,
            CancellationToken cancellationToken)
        {
            _projects.TryGetValue(projectId, out var project);
            return Task.FromResult(project);
        }

        public Task AddAsync(
            Project project,
            CancellationToken cancellationToken)
        {
            _projects[project.Id] = project;
            return Task.CompletedTask;
        }

        public Task SaveAsync(
            Project project,
            CancellationToken cancellationToken)
        {
            _projects[project.Id] = project;
            return Task.CompletedTask;
        }

        public Task AddRequirementAsync(
            Requirement requirement,
            CancellationToken cancellationToken)
        {
            AddedRequirement = requirement;
            return Task.CompletedTask;
        }

        public Task AddDecisionAsync(
            Decision decision,
            CancellationToken cancellationToken)
        {
            AddedDecision = decision;
            return Task.CompletedTask;
        }

        public Task AddAssumptionAsync(
            Assumption assumption,
            CancellationToken cancellationToken)
        {
            AddedAssumption = assumption;
            return Task.CompletedTask;
        }

    }
    private static AgentContext CreateContext()
    {
        return new AgentContext(
            ProjectId: Guid.NewGuid(),
            ProjectName: "Knowledge Agent",
            Objective: "Validate orchestration.",
            Requirements:
            [
                new AgentRequirementContext(
                Guid.NewGuid(),
                "Requirement A",
                "PROPOSED"),

            new AgentRequirementContext(
                Guid.NewGuid(),
                "Requirement B",
                "ACCEPTED")
            ],
            Decisions:
            [
                new AgentDecisionContext(
                Guid.NewGuid(),
                "Persistence",
                "PostgreSQL is the state store.",
                "ACCEPTED",
                "H2 validation.")
            ],
            Assumptions:
            [
                new AgentAssumptionContext(
                Guid.NewGuid(),
                "PostgreSQL is available.",
                "MEDIUM",
                false)
            ],
            Evidence:
            [
                new AgentEvidence(
                "IProjectRepository",
                "Project state was read from the project repository.")
            ]);
    }
}