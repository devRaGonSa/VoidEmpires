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

    try {
        Invoke-RestMethod -Method Get -Uri ($BaseUrl.TrimEnd("/") + $Path)
    }
    catch {
        throw "GET $Path failed. Ensure the Development backend is running at $BaseUrl. $($_.Exception.Message)"
    }
}

function Invoke-DevPost {
    param(
        [string]$Path,
        [object]$Body
    )

    try {
        Invoke-RestMethod `
            -Method Post `
            -Uri ($BaseUrl.TrimEnd("/") + $Path) `
            -ContentType "application/json" `
            -Body ($Body | ConvertTo-Json -Depth 6 -Compress)
    }
    catch {
        throw "POST $Path failed. Ensure the Development backend is running at $BaseUrl. $($_.Exception.Message)"
    }
}

function Invoke-DevOptionalGet {
    param(
        [string]$Path,
        [string]$Label
    )

    try {
        return Invoke-DevGet $Path
    }
    catch {
        $responseBody = Get-DevQaHttpResponseBody $_.Exception
        $errorText = Get-DevQaResponseErrorText -ResponseObject $null -FallbackText $responseBody
        if ([string]::IsNullOrWhiteSpace($errorText)) {
            $errorText = $_.Exception.Message
        }

        Write-Warning ("No se pudo leer {0}: {1}" -f $Label, $errorText)
        return $null
    }
}

Write-Host "Parametros: BaseUrl=$BaseUrl, Profile=$Profile, CivilizationId=$CivilizationId, PlanetId=$PlanetId"
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

Write-Host "Reading Shipyard baseline from /api/dev/shipyard/ui-state..."
$shipyardState = Invoke-DevOptionalGet "/api/dev/shipyard/ui-state?civilizationId=$CivilizationId&planetId=$PlanetId" "el estado de Astillero"

Write-Host "Reading Fleet baseline from /api/dev/fleets/ui-state..."
$fleetState = Invoke-DevOptionalGet "/api/dev/fleets/ui-state?civilizationId=$CivilizationId" "el estado de Flotas"

$planet = $planetState.uiState.planet
$research = $researchState.uiState
$planetResources = Format-DevQaResourceSummary $planet.stockpile
$shipyardSnapshot = Get-DevQaShipyardSnapshot (Get-DevQaPropertyValue -InputObject $shipyardState -PropertyNames @("uiState", "UiState"))
$fleetSnapshot = Get-DevQaFleetSnapshot -FleetUiState (Get-DevQaPropertyValue -InputObject $fleetState -PropertyNames @("uiState", "UiState")) -PlanetId $PlanetId

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

if ($null -ne $shipyardSnapshot) {
    Write-Host ""
    Write-Host "Astillero baseline snapshot:"
    [pscustomobject]@{
        Planeta = $shipyardSnapshot.Planet
        Recursos = $shipyardSnapshot.Resources
        OpcionesDisponibles = $shipyardSnapshot.AvailableOptions
        OpcionesBloqueadas = $shipyardSnapshot.BlockedOptions
        ColaVisible = $shipyardSnapshot.QueueCount
        StockVisible = $shipyardSnapshot.StockCount
    } | Format-List

    if ($shipyardSnapshot.ResourceWarnings.Count -gt 0) {
        Write-Warning ($shipyardSnapshot.ResourceWarnings -join " ")
    }
}

if ($null -ne $fleetSnapshot) {
    Write-Host ""
    Write-Host "Flotas baseline snapshot:"
    [pscustomobject]@{
        Escuadras = $fleetSnapshot.GroupCount
        Estacionadas = $fleetSnapshot.StationedCount
        TransferenciasActivas = $fleetSnapshot.ActiveTransferCount
        ContextoLocal = $fleetSnapshot.ResourceContext
        ContextosDeRecursos = $fleetSnapshot.ResourceContextCount
    } | Format-List

    if ($fleetSnapshot.ResourceWarnings.Count -gt 0) {
        Write-Warning ($fleetSnapshot.ResourceWarnings -join " ")
    }
}

Write-Host ""
Write-Host "Nota operativa: esta script aplica el seed documentado y relee el estado de Construction, Research, Astillero y Flotas. Puede insertar o completar minimos del baseline en la base Development, pero no crea ordenes manuales, no borra datos y no ejecuta migraciones."
