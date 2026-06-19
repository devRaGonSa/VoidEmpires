Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

param(
    [string]$ConnectionString
)

$repoRoot = Split-Path -Parent $PSScriptRoot

if ([string]::IsNullOrWhiteSpace($ConnectionString)) {
    $ConnectionString = $env:VOIDEMPIRES_SQLSERVER_SMOKE_CONNECTION_STRING
}

if ([string]::IsNullOrWhiteSpace($ConnectionString)) {
    $ConnectionString = $env:ConnectionStrings__DefaultConnection
}

if ([string]::IsNullOrWhiteSpace($ConnectionString)) {
    throw @"
No SQL Server connection string was provided.

Provide one of the following outside source control:
- -ConnectionString "<placeholder connection string>"
- VOIDEMPIRES_SQLSERVER_SMOKE_CONNECTION_STRING
- ConnectionStrings__DefaultConnection
"@
}

$previousEnabled = $env:VOIDEMPIRES_SQLSERVER_SMOKE_ENABLED
$previousConnection = $env:VOIDEMPIRES_SQLSERVER_SMOKE_CONNECTION_STRING

$env:VOIDEMPIRES_SQLSERVER_SMOKE_ENABLED = "true"
$env:VOIDEMPIRES_SQLSERVER_SMOKE_CONNECTION_STRING = $ConnectionString

Write-Host "Running the read-only SQL Server smoke check via SqlServerSmokeTests."
Write-Host "The check executes SELECT 1 only and does not apply migrations or write gameplay data."
Write-Host "The connection string will not be echoed."

Push-Location $repoRoot
try {
    & dotnet test --no-build --filter FullyQualifiedName~SqlServerSmokeTests
    if ($LASTEXITCODE -ne 0) {
        throw @"
SQL Server smoke check failed.

Confirm that:
- the SQL Server host is reachable
- the target database already exists
- the provided credentials are valid
- firewall or certificate settings are correct for your environment

The check remained read-only and did not apply migrations.
"@
    }
}
finally {
    Pop-Location

    if ($null -eq $previousEnabled) {
        Remove-Item Env:VOIDEMPIRES_SQLSERVER_SMOKE_ENABLED -ErrorAction SilentlyContinue
    }
    else {
        $env:VOIDEMPIRES_SQLSERVER_SMOKE_ENABLED = $previousEnabled
    }

    if ($null -eq $previousConnection) {
        Remove-Item Env:VOIDEMPIRES_SQLSERVER_SMOKE_CONNECTION_STRING -ErrorAction SilentlyContinue
    }
    else {
        $env:VOIDEMPIRES_SQLSERVER_SMOKE_CONNECTION_STRING = $previousConnection
    }
}

Write-Host "SQL Server smoke check passed."
Write-Host "Open your reviewed SQL scripts in SSMS separately if you continue with manual operator validation."
