param(
    [string]$BaseUrl = "http://localhost:5142",
    [string]$DisplayName = ("QA Commander {0}" -f (Get-Date -Format "yyyyMMddHHmmss")),
    [string]$CivilizationName = ("QA Civ {0}" -f (Get-Date -Format "yyyyMMddHHmmss")),
    [string]$HomePlanetName = "",
    [double]$ElapsedSeconds = 0,
    [switch]$PrintQueueMaterializationCommand
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"
try {
    [Console]::OutputEncoding = [System.Text.Encoding]::UTF8
    $OutputEncoding = [System.Text.Encoding]::UTF8
}
catch {
    Write-Verbose "Console UTF-8 encoding setup was not available in this host."
}
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

function Invoke-DevPostDetailed {
    param(
        [string]$Path,
        [hashtable]$Body
    )

    try {
        $result = Invoke-RestMethod `
            -Method Post `
            -Uri ($BaseUrl.TrimEnd("/") + $Path) `
            -ContentType "application/json" `
            -Body ($Body | ConvertTo-Json -Depth 8 -Compress)

        return [pscustomobject]@{
            Succeeded = $true
            StatusCode = 200
            Json = $result
            BodyText = $null
        }
    }
    catch {
        $response = $_.Exception.Response
        $statusCode = if ($null -ne $response) { [int]$response.StatusCode } else { $null }
        $responseBody = Get-DevQaHttpResponseBody $_.Exception
        return [pscustomobject]@{
            Succeeded = $false
            StatusCode = $statusCode
            Json = ConvertFrom-DevQaJsonSafely $responseBody
            BodyText = $responseBody
        }
    }
}

function Get-PlanetState {
    param(
        [string]$CivilizationId,
        [string]$PlanetId
    )

    Invoke-DevGet "/api/dev/planets/ui-state?civilizationId=$CivilizationId&planetId=$PlanetId"
}

function Format-ResourceDelta {
    param(
        [hashtable]$Before,
        [hashtable]$After
    )

    $keys = @($Before.Keys + $After.Keys | Sort-Object -Unique)
    foreach ($key in $keys) {
        $beforeValue = if ($Before.ContainsKey($key)) { [decimal]$Before[$key] } else { [decimal]0 }
        $afterValue = if ($After.ContainsKey($key)) { [decimal]$After[$key] } else { [decimal]0 }
        [pscustomobject]@{
            Resource = $key
            Before = $beforeValue
            After = $afterValue
            Delta = $afterValue - $beforeValue
        }
    }
}

if ($ElapsedSeconds -lt 0) {
    throw "ElapsedSeconds cannot be negative."
}

$createPayload = [ordered]@{
    displayName = $DisplayName
    civilizationName = $CivilizationName
}

if (-not [string]::IsNullOrWhiteSpace($HomePlanetName)) {
    $createPayload.homePlanetName = $HomePlanetName
}

Write-Warning "Esta script muta la base Development. Crea un inicio jugable real y puede materializar produccion de recursos."
Write-Host ""
Write-Host "Target"
Write-Host "BaseUrl: $BaseUrl"
Write-Host "Action: Create Development playable session"
Write-Host "Payload summary: $(Format-DevQaPayloadSummary $createPayload)"

$createResult = Invoke-DevPostDetailed "/api/dev/players/starting-civilization" $createPayload
if (-not $createResult.Succeeded -or -not $createResult.Json.succeeded) {
    $errors = Get-DevQaResponseErrorText -ResponseObject $createResult.Json -FallbackText $createResult.BodyText
    $details = if (-not [string]::IsNullOrWhiteSpace($errors)) { $errors } else { $createResult.BodyText }
    throw "Playable-session creation failed with HTTP $($createResult.StatusCode). $details"
}

$created = $createResult.Json
$civilizationId = "$($created.civilizationId)"
$planetId = "$($created.homePlanetId)"

Write-Host "Reading planet state for the created playable session..."
$planetState = Get-PlanetState -CivilizationId $civilizationId -PlanetId $planetId
$planet = $planetState.uiState.planet
if ($null -eq $planet) {
    throw "Planet UI state did not include a selected planet for the new playable session."
}

$beforeResources = (Get-DevQaResourceMap $planet.stockpile).Map
$afterResources = $beforeResources
$economyApplied = $false

if ($ElapsedSeconds -gt 0) {
    Write-Host "Applying backend resource accrual for $ElapsedSeconds seconds..."
    $economyResult = Invoke-DevPostDetailed "/api/dev/planets/resource-economy/apply" @{
        civilizationId = $civilizationId
        planetId = $planetId
        elapsedSeconds = $ElapsedSeconds
    }

    if (-not $economyResult.Succeeded -or -not $economyResult.Json.succeeded) {
        $errors = Get-DevQaResponseErrorText -ResponseObject $economyResult.Json -FallbackText $economyResult.BodyText
        $details = if (-not [string]::IsNullOrWhiteSpace($errors)) { $errors } else { $economyResult.BodyText }
        throw "Resource accrual apply failed with HTTP $($economyResult.StatusCode). $details"
    }

    $refreshedPlanetState = Get-PlanetState -CivilizationId $civilizationId -PlanetId $planetId
    $refreshedPlanet = $refreshedPlanetState.uiState.planet
    if ($null -eq $refreshedPlanet) {
        throw "Planet UI state refresh failed after resource accrual."
    }

    $afterResources = (Get-DevQaResourceMap $refreshedPlanet.stockpile).Map
    $planet = $refreshedPlanet
    $economyApplied = $true
}

Write-Host ""
Write-Host "Playable session prepared."
[pscustomobject]@{
    UserId = $created.userId
    PlayerProfileId = $created.playerProfileId
    CivilizationId = $civilizationId
    HomePlanetId = $planetId
    HomePlanetName = $created.homePlanetName
    HomeSystemId = $created.homeSystemId
    HomeSystemName = $created.homeSystemName
    EconomyMaterialized = $economyApplied
    ElapsedSeconds = $ElapsedSeconds
} | Format-List

Write-Host ""
Write-Host "Starting resource snapshot:"
[pscustomobject]@{
    Resources = (Format-DevQaResourceSummary $created.startingResources).Summary
    Limitations = @($created.limitations) -join " | "
} | Format-List

Write-Host ""
Write-Host "Current planet snapshot:"
[pscustomobject]@{
    Planet = $planet.planetName
    Owner = $planet.ownerCivilizationName
    Resources = (Format-DevQaResourceSummary $planet.stockpile).Summary
    ProductionPerHour = if ($null -ne $planet.productionSummary) {
        "Credits={0}, Metal={1}, Crystal={2}, Gas={3}, Multiplier=x{4}" -f `
            $planet.productionSummary.creditsPerHour, `
            $planet.productionSummary.metalPerHour, `
            $planet.productionSummary.crystalPerHour, `
            $planet.productionSummary.gasPerHour, `
            $planet.productionSummary.researchMultiplier
    } else {
        "No visible production summary."
    }
} | Format-List

Write-Host ""
Write-Host "Next frontend routes:"
Write-Host "/planet?civilizationId=$civilizationId&planetId=$planetId"
Write-Host "/construction?civilizationId=$civilizationId&planetId=$planetId"
Write-Host "/research?civilizationId=$civilizationId&planetId=$planetId"
Write-Host "/shipyard?civilizationId=$civilizationId&planetId=$planetId"

if ($economyApplied) {
    Write-Host ""
    Write-Host "Resource delta after backend accrual:"
    Format-ResourceDelta -Before $beforeResources -After $afterResources | Format-Table -AutoSize
}

if ($PrintQueueMaterializationCommand) {
    $materializationBaseUrl = $BaseUrl.TrimEnd("/")
    $materializationCommand = "powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\dev-qa-materialize-due-queues.ps1 -BaseUrl `"$materializationBaseUrl`" -CivilizationId $civilizationId -PlanetId $planetId -ElapsedSeconds 3600"

    Write-Host ""
    Write-Host "Next suggested command for later manual QA:"
    Write-Host $materializationCommand
    Write-Host "Run it only after enqueueing due Construction, Research, or Shipyard orders for this playable start."
}

Write-Host ""
Write-Host "Nota operativa: esta script crea un inicio jugable real para Development y relee el planeta desde la API. No ejecuta migraciones, no borra datos y no modifica la base fuera de los endpoints de desarrollo existentes."
