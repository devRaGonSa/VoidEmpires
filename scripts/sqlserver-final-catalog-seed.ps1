param(
    [string]$ConnectionString,
    [string]$SourceDirectory,
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Debug",
    [switch]$Apply,
    [switch]$ConfirmMutation
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Get-OperatorConnectionString {
    param(
        [string]$PreferredConnectionString
    )

    if (-not [string]::IsNullOrWhiteSpace($PreferredConnectionString)) {
        return $PreferredConnectionString.Trim()
    }

    $candidates = @(
        $env:VOIDEMPIRES_CONNECTION_STRING,
        $env:ConnectionStrings__DefaultConnection,
        $env:VOIDEMPIRES_SQLSERVER_SMOKE_CONNECTION_STRING
    )

    foreach ($candidate in $candidates) {
        if (-not [string]::IsNullOrWhiteSpace($candidate)) {
            return $candidate.Trim()
        }
    }

    throw @"
No SQL Server operator connection context was provided.

Provide one of the following outside source control:
- -ConnectionString "<connection string>"
- VOIDEMPIRES_CONNECTION_STRING
- ConnectionStrings__DefaultConnection
- VOIDEMPIRES_SQLSERVER_SMOKE_CONNECTION_STRING
"@
}

function Assert-ExplicitConnectionContext {
    param(
        [Parameter(Mandatory = $true)]
        [string]$EffectiveConnectionString
    )

    if ($EffectiveConnectionString -match "<USER>|<PASSWORD>") {
        throw "The provided connection string still contains placeholder tokens. Supply an operator-managed value outside source control."
    }
}

function Get-InfrastructureAssemblyPath {
    param(
        [Parameter(Mandatory = $true)]
        [string]$RepositoryRoot,
        [Parameter(Mandatory = $true)]
        [string]$BuildConfiguration
    )

    return Join-Path $RepositoryRoot "src\VoidEmpires.Infrastructure\bin\$BuildConfiguration\net8.0\VoidEmpires.Infrastructure.dll"
}

function Invoke-BackendSeedRunner {
    param(
        [Parameter(Mandatory = $true)]
        [string]$RepositoryRoot,
        [Parameter(Mandatory = $true)]
        [string]$CatalogSourceDirectory,
        [Parameter(Mandatory = $true)]
        [bool]$DryRun
    )

    $tempRoot = Join-Path ([System.IO.Path]::GetTempPath()) ("voidempires-final-catalog-seed-" + [Guid]::NewGuid().ToString("N"))
    $projectPath = Join-Path $tempRoot "FinalCatalogSeedRunner.csproj"
    $programPath = Join-Path $tempRoot "Program.cs"

    New-Item -ItemType Directory -Path $tempRoot -Force | Out-Null

    $escapedRepoRoot = $RepositoryRoot.Replace("\", "\\")
    $escapedCatalogSourceDirectory = $CatalogSourceDirectory.Replace("\", "\\")
    $dryRunLiteral = if ($DryRun) { "true" } else { "false" }

    $projectContent = @"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="$escapedRepoRoot\\src\\VoidEmpires.Application\\VoidEmpires.Application.csproj" />
    <ProjectReference Include="$escapedRepoRoot\\src\\VoidEmpires.Infrastructure\\VoidEmpires.Infrastructure.csproj" />
  </ItemGroup>
</Project>
"@

    $programContent = @"
using VoidEmpires.Application.Seeding;
using VoidEmpires.Infrastructure.SeedData.CatalogSources;

var sourceDirectory = "$escapedCatalogSourceDirectory";
var dryRun = $dryRunLiteral;

var service = new FinalCatalogSeedService(new CatalogSeedSourceLoader());
var result = await service.RunAsync(new FinalCatalogSeedRequest(dryRun, sourceDirectory));

foreach (var catalog in result.Catalogs)
{
    Console.WriteLine($"Validated {catalog.CatalogFile}: {catalog.RowCount} rows");
}

foreach (var note in result.Notes)
{
    Console.WriteLine($"Note: {note}");
}

foreach (var error in result.Errors)
{
    Console.Error.WriteLine(error);
}

Console.WriteLine(result.ApplyDeferred
    ? "Apply remains deferred by backend design."
    : "Apply is not deferred.");

Environment.ExitCode = result.Succeeded ? 0 : 1;
"@

    Set-Content -LiteralPath $projectPath -Value $projectContent -Encoding UTF8
    Set-Content -LiteralPath $programPath -Value $programContent -Encoding UTF8

    Push-Location $RepositoryRoot
    try {
        & dotnet run --project $projectPath
        if ($LASTEXITCODE -ne 0) {
            throw "The backend final catalog seed runner reported a failure."
        }
    }
    finally {
        Pop-Location
        Remove-Item -LiteralPath $tempRoot -Recurse -Force -ErrorAction SilentlyContinue
    }
}

$repoRoot = Split-Path -Parent $PSScriptRoot
$effectiveConnectionString = Get-OperatorConnectionString -PreferredConnectionString $ConnectionString
Assert-ExplicitConnectionContext -EffectiveConnectionString $effectiveConnectionString

if ($Apply -and -not $ConfirmMutation) {
    throw @"
Final catalog apply requires explicit confirmation.

Rerun with:
-Apply -ConfirmMutation

This repository never applies final SQL Server catalog changes automatically.
"@
}

$effectiveDryRun = -not $Apply
$resolvedSourceDirectory = if ([string]::IsNullOrWhiteSpace($SourceDirectory)) {
    Join-Path $repoRoot "src\VoidEmpires.Infrastructure\SeedData\CatalogSources"
}
else {
    $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($SourceDirectory)
}

if (-not (Test-Path -LiteralPath $resolvedSourceDirectory -PathType Container)) {
    throw "Catalog source directory '$resolvedSourceDirectory' was not found."
}

$infrastructureAssemblyPath = Get-InfrastructureAssemblyPath -RepositoryRoot $repoRoot -BuildConfiguration $Configuration
if (-not (Test-Path -LiteralPath $infrastructureAssemblyPath)) {
    Write-Host "Infrastructure build output was not found. Running dotnet build --no-restore first."

    Push-Location $repoRoot
    try {
        & dotnet build --no-restore
        if ($LASTEXITCODE -ne 0) {
            throw "dotnet build --no-restore failed, so the final catalog seed helper could not invoke the backend service."
        }
    }
    finally {
        Pop-Location
    }
}

if (-not (Test-Path -LiteralPath $infrastructureAssemblyPath)) {
    throw "Expected backend assembly '$infrastructureAssemblyPath' was not found after build."
}

if ($effectiveDryRun) {
    Write-Host "Running final catalog seed dry-run validation."
    Write-Host "The backend catalog loader will validate the checked-in catalog sources without mutating a database."
}
else {
    Write-Host "Attempting final catalog apply with explicit operator confirmation."
    Write-Host "The connection string will not be echoed."
    Write-Host "Current backend behavior is expected to stop before mutation until the final relational apply path exists."
}

Invoke-BackendSeedRunner -RepositoryRoot $repoRoot -CatalogSourceDirectory $resolvedSourceDirectory -DryRun:$effectiveDryRun

Write-Host "Final catalog seed workflow completed successfully."
