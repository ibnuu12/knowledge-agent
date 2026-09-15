using KnowledgeAgent.Domain;
using Microsoft.EntityFrameworkCore;

namespace KnowledgeAgent.Infrastructure;

public sealed class KnowledgeAgentDbContext(DbContextOptions<KnowledgeAgentDbContext> options) : DbContext(options)
{
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Requirement> Requirements => Set<Requirement>();
    public DbSet<Decision> Decisions => Set<Decision>();
    public DbSet<Assumption> Assumptions => Set<Assumption>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Project>(e =>
        {
            e.ToTable("projects");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.Objective).HasMaxLength(4000);
            e.HasMany(x => x.Requirements).WithOne().HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Cascade);
            e.HasMany(x => x.Decisions).WithOne().HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Cascade);
            e.HasMany(x => x.Assumptions).WithOne().HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Requirement>(e =>
        {
            e.ToTable("requirements");
            e.HasKey(x => x.Id);
            e.Property(x => x.Statement).HasMaxLength(8000).IsRequired();
            e.Property(x => x.Status).HasMaxLength(32).IsRequired();
        });

        modelBuilder.Entity<Decision>(e =>
        {
            e.ToTable("decisions");
            e.HasKey(x => x.Id);
            e.Property(x => x.Topic).HasMaxLength(300).IsRequired();
            e.Property(x => x.DecisionText).HasMaxLength(8000).IsRequired();
            e.Property(x => x.Status).HasMaxLength(32).IsRequired();
            e.Property(x => x.EvidenceSummary).HasMaxLength(8000);
            e.Property(x => x.AuthorizationNote).HasMaxLength(4000);
        });

        modelBuilder.Entity<Assumption>(e =>
        {
            e.ToTable("assumptions");
            e.HasKey(x => x.Id);
            e.Property(x => x.Statement).HasMaxLength(8000).IsRequired();
            e.Property(x => x.RiskLevel).HasMaxLength(16).IsRequired();
        });
    }
}
