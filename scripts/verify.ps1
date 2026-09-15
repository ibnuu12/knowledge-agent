$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path $PSScriptRoot -Parent
$Solution = Join-Path $RepoRoot "KnowledgeAgent.sln"

Write-Host "== SDK =="
dotnet --version
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host "== Restore =="
dotnet restore $Solution
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host "== Build =="
dotnet build $Solution --configuration Debug --no-restore
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host "== Test =="
dotnet test $Solution --configuration Debug --no-build
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host ""
Write-Host "Verification PASSED."
exit 0
