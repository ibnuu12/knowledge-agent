using KnowledgeAgent.Api;
using KnowledgeAgent.Application;
using KnowledgeAgent.Infrastructure;
using Microsoft.EntityFrameworkCore;
using KnowledgeAgent.Application.Agents;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddDbContext<KnowledgeAgentDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

// Depedency Registration
builder.Services.AddScoped<IProjectRepository, EfProjectRepository>();
builder.Services.AddScoped<ProjectService>();
builder.Services.AddScoped<IAgentOrchestrator, AgentOrchestrator>();
builder.Services.AddScoped<IAgentContextAssembler, AgentContextAssembler>();
builder.Services.AddScoped<IAgentOrchestrator, AgentOrchestrator>();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.MapGet("/health", async (KnowledgeAgentDbContext db, CancellationToken ct) =>
{
    try
    {
        var databaseOk = await db.Database.CanConnectAsync(ct);
        return Results.Ok(new { status = databaseOk ? "ok" : "degraded", database = databaseOk });
    }
    catch (Exception ex) when (ex is Npgsql.NpgsqlException or InvalidOperationException)
    {
        return Results.Ok(new { status = "degraded", database = false });
    }
});

app.MapPost("/api/projects", async (CreateProjectCommand command, ProjectService service, CancellationToken ct) =>
{
    var project = await service.CreateAsync(command, ct);
    return Results.Created($"/api/projects/{project.Id}", project);
});

app.MapGet("/api/projects/{id:guid}", async (Guid id, ProjectService service, CancellationToken ct) =>
{
    var project = await service.GetAsync(id, ct);
    return project is null ? Results.NotFound() : Results.Ok(project);
});

app.MapPost("/api/projects/{id:guid}/requirements", async (Guid id, AddRequirementCommand command, ProjectService service, CancellationToken ct) =>
{
    var item = await service.AddRequirementAsync(id, command, ct);
    return Results.Created($"/api/projects/{id}", item);
});

app.MapPost("/api/projects/{id:guid}/decisions", async (Guid id, RecordDecisionCommand command, ProjectService service, CancellationToken ct) =>
{
    var item = await service.RecordDecisionAsync(id, command, ct);
    return Results.Created($"/api/projects/{id}", item);
});

app.MapPost("/api/projects/{id:guid}/assumptions", async (Guid id, RecordAssumptionCommand command, ProjectService service, CancellationToken ct) =>
{
    var item = await service.RecordAssumptionAsync(id, command, ct);
    return Results.Created($"/api/projects/{id}", item);
});

app.MapPost(
    "/api/projects/{id:guid}/agent/messages",
    async (
        Guid id,
        AgentMessageRequest request,
        IAgentOrchestrator agent,
        CancellationToken ct) =>
    {
        var message = new AgentMessage(
            id,
            request.Message);

        var response = await agent.ProcessAsync(message, ct);

        return Results.Ok(response);
    });

app.Run();