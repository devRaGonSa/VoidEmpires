param(
    [string]$OutputPath = ".\artifacts\sql\voidempires-sqlserver-initial-baseline.sql"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$baselineMigrationPath = Join-Path $repoRoot "src\VoidEmpires.Infrastructure\Persistence\Migrations\SqlServer\*SqlServerInitialBaseline*.cs"
$generationOnlyConnectionString = "Server=localhost;Database=VoidEmpires_GenerationOnly;User Id=<USER>;Password=<PASSWORD>;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True;"

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
if ([System.IO.Path]::GetExtension($resolvedOutputPath) -ne ".sql") {
    throw "OutputPath must point to a .sql file for manual review."
}

$outputDirectory = Split-Path -Parent $resolvedOutputPath
if (-not [string]::IsNullOrWhiteSpace($outputDirectory) -and -not (Test-Path -LiteralPath $outputDirectory)) {
    New-Item -ItemType Directory -Path $outputDirectory -Force | Out-Null
}

$previousProvider = $env:VoidEmpires__Persistence__Provider
$previousDefaultConnection = $env:ConnectionStrings__DefaultConnection
$previousLegacyConnection = $env:VOIDEMPIRES_CONNECTION_STRING

$env:VoidEmpires__Persistence__Provider = "sqlserver"
$env:ConnectionStrings__DefaultConnection = $generationOnlyConnectionString
$env:VOIDEMPIRES_CONNECTION_STRING = $generationOnlyConnectionString

Write-Host "Generating an idempotent SQL Server migration script for manual review."
Write-Host "This helper uses a placeholder generation-only connection string and does not require a real password."
Write-Host "This helper does not run database update and does not apply migrations."

Push-Location $repoRoot
try {
    & dotnet ef migrations script 0 SqlServerInitialBaseline `
        --idempotent `
        --project "src/VoidEmpires.Infrastructure/VoidEmpires.Infrastructure.csproj" `
        --startup-project "src/VoidEmpires.Web/VoidEmpires.Web.csproj" `
        --context "VoidEmpires.Infrastructure.Persistence.VoidEmpiresDbContext" `
        --output $resolvedOutputPath

    if ($LASTEXITCODE -ne 0) {
        throw "dotnet ef migrations script failed. No migration was applied by this helper."
    }
}
finally {
    Pop-Location

    if ($null -eq $previousProvider) {
        Remove-Item Env:VoidEmpires__Persistence__Provider -ErrorAction SilentlyContinue
    }
    else {
        $env:VoidEmpires__Persistence__Provider = $previousProvider
    }

    if ($null -eq $previousDefaultConnection) {
        Remove-Item Env:ConnectionStrings__DefaultConnection -ErrorAction SilentlyContinue
    }
    else {
        $env:ConnectionStrings__DefaultConnection = $previousDefaultConnection
    }

    if ($null -eq $previousLegacyConnection) {
        Remove-Item Env:VOIDEMPIRES_CONNECTION_STRING -ErrorAction SilentlyContinue
    }
    else {
        $env:VOIDEMPIRES_CONNECTION_STRING = $previousLegacyConnection
    }
}

Write-Host "SQL Server migration script generated at: $resolvedOutputPath"
Write-Host "Next step: review the generated SQL manually before opening it in SSMS."
Write-Host "Warning: the generated script may alter schema if an operator later executes it manually."
Write-Host "Do not commit one-off generated SQL output unless a future task explicitly asks for a reviewed template."
