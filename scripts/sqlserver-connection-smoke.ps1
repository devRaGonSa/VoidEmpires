param(
    [string]$ConnectionString
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot

function Get-SqlServerSmokeFailureGuidance {
    return @"
SQL Server smoke check failed.

Common checks:
- Server unreachable: verify the host, port 1433, SQL Server service state, TCP/IP configuration, firewall, and VPN/LAN reachability.
- Login failed: verify SQL Server Authentication is enabled, the local application login exists, the password is correct, and the login is not locked or expired.
- Database missing: create [VoidEmpires_Dev] manually in SSMS first, then confirm the connection string Database value matches it exactly.
- Encryption or certificate issue: keep Encrypt=True and use TrustServerCertificate=True for a private/self-managed certificate path, or install a trusted certificate chain before disabling that setting.

The check remained read-only. It executed only the SqlServerSmokeTests SELECT 1 path and did not apply migrations, inspect schema, or write gameplay data.
"@
}

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
        throw (Get-SqlServerSmokeFailureGuidance)
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
