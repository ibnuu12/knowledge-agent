# Implementation State — v0.1

## Completed in scaffold
- .NET 10 Web API project structure
- Domain entities for Project, Requirement, Decision, Assumption
- Application service/repository boundaries
- PostgreSQL EF Core persistence boundary
- Basic REST endpoints
- Agent orchestration boundary
- Explicit no-false-claim behavior for unexecuted LLM/tools
- Initial unit test

## Not yet verified
- `dotnet restore`
- `dotnet build`
- `dotnet test`
- PostgreSQL connectivity on the target Windows machine
- EF Core migration generation/application
- Real LLM integration
- Obsidian integration

## Next implementation slice
1. Run restore/build/test on Windows/.NET 10.0.400.
2. Add EF Core migration and apply it to local PostgreSQL.
3. Define the LLM provider boundary and configure the selected provider.
4. Implement Master Prompt v1.1 loading as a versioned prompt artifact.
5. Implement Context Builder from persistent state.
6. Add Obsidian read-only adapter with provenance/freshness metadata.
7. Execute T01 as the first real longitudinal session.

## Remediation: xUnit/Test Dependencies

- Added central package versions for xUnit, xUnit Visual Studio runner, and Microsoft.NET.Test.Sdk.
- Added explicit test package references to the application test project.
- Added `using Xunit;` where `[Fact]` is used.
- Verification script now resolves the repository root from `$PSScriptRoot` and exits non-zero on restore/build/test failure.
- Actual restore/build/test verification remains pending in the user's Windows environment.

## Remediation: NuGet security advisories

- `Microsoft.OpenApi` pinned to `2.7.5`, the first patched 2.x release for GHSA-v5pm-xwqc-g5wc.
- `System.Security.Cryptography.Xml` pinned to `10.0.10`, the patched .NET 10 package line for the reported July 2026 high-severity advisories.
- No NU1903 suppression was added; restore must remain security-strict.
- Actual restore/build/test verification remains pending in the user's Windows environment.


### H2 PostgreSQL state foundation

- Connection string: configured through `ConnectionStrings:Postgres`; environment override supported via `ConnectionStrings__Postgres`.
- EF design-time factory: implemented.
- Local `dotnet-ef` tool manifest: implemented at `.config/dotnet-tools.json`.
- Migration generation/application: **pending local PostgreSQL verification**.
- Automatic production startup migration: intentionally not enabled; schema changes remain an explicit operational action.
- Database health endpoint: reports `degraded` instead of failing the process when PostgreSQL is unavailable.

### Security dependency correction

- `Microsoft.OpenApi` and `System.Security.Cryptography.Xml` are now explicit package references so the central patched versions participate in dependency resolution rather than existing only as unused central version declarations.
- Verification on Windows is required to confirm the resolved graph no longer reports the previously observed NU1903 findings.
