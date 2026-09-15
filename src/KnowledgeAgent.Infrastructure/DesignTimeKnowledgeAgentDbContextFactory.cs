using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace KnowledgeAgent.Infrastructure;

public sealed class DesignTimeKnowledgeAgentDbContextFactory
    : IDesignTimeDbContextFactory<KnowledgeAgentDbContext>
{
    public KnowledgeAgentDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("ConnectionStrings__Postgres");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "ConnectionStrings__Postgres environment variable is required " +
                "when running EF Core migrations.");
        }

        var options = new DbContextOptionsBuilder<KnowledgeAgentDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new KnowledgeAgentDbContext(options);
    }
}