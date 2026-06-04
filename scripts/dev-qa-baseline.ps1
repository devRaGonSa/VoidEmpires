param(
    [string]$BaseUrl = "http://localhost:5142",
    [string]$Profile = "cockpit-validation",
    [Guid]$CivilizationId = "00000000-0000-0000-0000-000000000001",
    [Guid]$PlanetId = "40000000-0000-0000-0000-000000000001"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"
. (Join-Path $PSScriptRoot "dev-qa-common.ps1")

function Invoke-DevGet {
    param([string]$Path)

    Invoke-RestMethod -Method Get -Uri ($BaseUrl.TrimEnd("/") + $Path)
}

function Invoke-DevPost {
    param(
        [string]$Path,
        [object]$Body
    )

    Invoke-RestMethod `
        -Method Post `
        -Uri ($BaseUrl.TrimEnd("/") + $Path) `
        -ContentType "application/json" `
        -Body ($Body | ConvertTo-Json -Depth 6 -Compress)
}

Write-Host "Checking seed profile catalog..."
$profilesResponse = Invoke-DevGet "/api/dev/seeds/profiles"
$profileNames = @($profilesResponse.profiles | ForEach-Object { $_.name })

if ($profileNames -notcontains $Profile) {
    throw "Seed profile '$Profile' was not found. Known profiles: $($profileNames -join ', ')"
}

Write-Host "Applying seed profile '$Profile' twice to confirm the additive baseline..."
$firstApply = Invoke-DevPost "/api/dev/seeds/apply" @{ profile = $Profile }
$secondApply = Invoke-DevPost "/api/dev/seeds/apply" @{ profile = $Profile }

Write-Host "Reading Construction baseline from /api/dev/planets/ui-state..."
$planetState = Invoke-DevGet "/api/dev/planets/ui-state?civilizationId=$CivilizationId&planetId=$PlanetId"

Write-Host "Reading Research baseline from /api/dev/research/ui-state..."
$researchState = Invoke-DevGet "/api/dev/research/ui-state?civilizationId=$CivilizationId&planetId=$PlanetId"

$planet = $planetState.uiState.planet
$research = $researchState.uiState
$planetResources = Format-DevQaResourceSummary $planet.stockpile

$availableConstruction = @($planet.constructionActions | Where-Object { $_.availabilityStatus -eq "Available" }).Count
$blockedConstruction = @($planet.constructionActions | Where-Object { $_.availabilityStatus -ne "Available" }).Count
$openConstructionQueue = Get-DevQaOpenQueueCount $planet.constructionQueue

$availableResearch = @($research.technologyHints | Where-Object { $_.canEnqueue }).Count
$blockedResearch = @($research.technologyHints | Where-Object { -not $_.canEnqueue }).Count
$openResearchQueue = Get-DevQaOpenQueueCount $research.queue
$completedResearchProjects = @($research.projects).Count

Write-Host ""
Write-Host "Seed profile catalog:"
$profilesResponse.profiles |
    Select-Object name, deterministic, destructive |
    Format-Table -AutoSize

Write-Host ""
Write-Host "Seed apply results:"
[pscustomobject]@{
    Attempt = 1
    Profile = $firstApply.profile
    Succeeded = $firstApply.succeeded
    AppliedSteps = @($firstApply.appliedSteps).Count
} | Format-Table -AutoSize
[pscustomobject]@{
    Attempt = 2
    Profile = $secondApply.profile
    Succeeded = $secondApply.succeeded
    AppliedSteps = @($secondApply.appliedSteps).Count
} | Format-Table -AutoSize

Write-Host ""
Write-Host "Construction baseline snapshot:"
[pscustomobject]@{
    Planet = $planet.planetName
    Resources = $planetResources.Summary
    AvailableActions = $availableConstruction
    BlockedActions = $blockedConstruction
    OpenQueueItems = $openConstructionQueue
    VisibleQueueItems = @($planet.constructionQueue).Count
} | Format-List

if ($planetResources.Warnings.Count -gt 0) {
    Write-Warning ($planetResources.Warnings -join " ")
}

Write-Host ""
Write-Host "Research baseline snapshot:"
[pscustomobject]@{
    Planet = $research.selectedPlanetName
    AvailableHints = $availableResearch
    BlockedHints = $blockedResearch
    OpenQueueItems = $openResearchQueue
    VisibleQueueItems = @($research.queue).Count
    CompletedProjects = $completedResearchProjects
} | Format-List

Write-Host ""
Write-Host "Read-only note: this script applies the documented seed baseline and inspects state only. It does not create orders, delete data, or run migrations."
