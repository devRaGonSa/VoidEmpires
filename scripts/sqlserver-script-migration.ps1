Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

param(
    [string]$OutputPath = ".\artifacts\sql\voidempires-sqlserver-initial-baseline.sql"
)

$repoRoot = Split-Path -Parent $PSScriptRoot
$baselineMigrationPath = Join-Path $repoRoot "src\VoidEmpires.Infrastructure\Persistence\Migrations\SqlServer\*SqlServerInitialBaseline*.cs"

if (-not (Get-ChildItem -Path $baselineMigrationPath -ErrorAction SilentlyContinue)) {
    throw @"
SQL Server baseline migration files were not found under src\VoidEmpires.Infrastructure\Persistence\Migrations\SqlServer.

This helper is intentionally guarded.
It only scripts a reviewed SQL Server baseline after that baseline exists.

Next step:
- follow docs/dev/sql-server-migration-strategy.md
- generate SqlServerInitialBaseline first
- rerun this helper afterward
"@
}

$resolvedOutputPath = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($OutputPath)
$outputDirectory = Split-Path -Parent $resolvedOutputPath
if (-not [string]::IsNullOrWhiteSpace($outputDirectory) -and -not (Test-Path -LiteralPath $outputDirectory)) {
    New-Item -ItemType Directory -Path $outputDirectory -Force | Out-Null
}

$env:VoidEmpires__Persistence__Provider = "sqlserver"
if (-not $env:ConnectionStrings__DefaultConnection) {
    $env:ConnectionStrings__DefaultConnection = "Server=localhost;Database=VoidEmpires_GenerationOnly;User Id=<USER>;Password=<PASSWORD>;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True;"
}

if (-not $env:VOIDEMPIRES_CONNECTION_STRING) {
    $env:VOIDEMPIRES_CONNECTION_STRING = $env:ConnectionStrings__DefaultConnection
}

Push-Location $repoRoot
try {
    & dotnet ef migrations script 0 SqlServerInitialBaseline `
        --idempotent `
        --project "src/VoidEmpires.Infrastructure/VoidEmpires.Infrastructure.csproj" `
        --startup-project "src/VoidEmpires.Web/VoidEmpires.Web.csproj" `
        --context "VoidEmpires.Infrastructure.Persistence.VoidEmpiresDbContext" `
        --output $resolvedOutputPath
}
finally {
    Pop-Location
}

Write-Host "SQL Server migration script generated at: $resolvedOutputPath"
Write-Host "Review the script manually before opening it in SSMS."
Write-Host "This helper does not run database update and does not apply the script."
