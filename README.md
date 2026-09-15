# Knowledge Agent v0.1

Foundation/vertical-slice baseline for the Knowledge Agent project.

## Baseline
- .NET SDK 10.0.400
- ASP.NET Core Web API
- PostgreSQL via local installation
- Obsidian integration: adapter boundary only in v0.1
- Master Prompt: v1.1 as the operating contract

## v0.1 scope
- Project creation and retrieval
- Requirement recording
- Decision recording with decision history
- Assumption recording
- Persistent project state in PostgreSQL through EF Core
- Agent orchestration boundary
- Health endpoint
- Explicit separation between observed state and agent inference/recommendation

## Not implemented yet
- Real LLM provider integration
- Obsidian filesystem/indexing implementation
- Vector search
- Arbitrary shell/SQL tools
- Production authentication/authorization
- Full behavioral harness

## Run
1. Create a PostgreSQL database, e.g. `knowledge_agent`.
2. Set `ConnectionStrings__Postgres` in the environment.
3. Restore and run `src/KnowledgeAgent.Api`.

Example PowerShell:

```powershell
$env:ConnectionStrings__Postgres="Host=localhost;Port=5432;Database=knowledge_agent;Username=postgres;Password=..."
dotnet restore
dotnet run --project .\src\KnowledgeAgent.Api
```

The container used to generate this scaffold does not contain the .NET SDK, so compilation/restore could not be executed here. The target environment is the user's Windows/.NET 10.0.400 machine.

## Environment and IDE compatibility

- Target framework: `net10.0`
- Required SDK baseline: `.NET SDK 10.0.400`
- Visual Studio: `18.0` or newer for .NET 10 targeting support
- PostgreSQL: local installation
- API: ASP.NET Core Web API

The project is intentionally kept on .NET 10 rather than downgraded to .NET 9.

### CLI verification on Windows

From the repository root:

```powershell
dotnet --version
dotnet restore .\KnowledgeAgent.sln
dotnet build .\KnowledgeAgent.sln --configuration Debug
dotnet test .\KnowledgeAgent.sln --configuration Debug
```

Or run:

```powershell
.\scripts\verify.ps1
```

If Visual Studio is older than 18.0, use the .NET CLI for build/test until Visual Studio is upgraded.

## Security dependency policy

High-severity NuGet audit findings are treated as blockers. The project pins patched
transitive dependencies instead of suppressing NU1903. For the current .NET 10 baseline,
`Microsoft.OpenApi` is pinned to 2.7.5 and `System.Security.Cryptography.Xml` to 10.0.10.

## PostgreSQL development setup

The application reads the PostgreSQL connection string from the standard `ConnectionStrings:Postgres` configuration key. For local development, prefer the environment variable `ConnectionStrings__Postgres` rather than committing credentials.

Example PowerShell session:

```powershell
$env:ConnectionStrings__Postgres = "Host=localhost;Port=5432;Database=knowledge_agent;Username=postgres;Password=<LOCAL_PASSWORD>"
```

Restore the local EF CLI tool and create/apply the initial migration:

```powershell
dotnet tool restore
dotnet ef migrations add InitialState --project src/KnowledgeAgent.Infrastructure --startup-project src/KnowledgeAgent.Api --output-dir Migrations
dotnet ef database update --project src/KnowledgeAgent.Infrastructure --startup-project src/KnowledgeAgent.Api
```

Do not commit a real PostgreSQL password. The `CHANGE_ME` value is a placeholder only.
