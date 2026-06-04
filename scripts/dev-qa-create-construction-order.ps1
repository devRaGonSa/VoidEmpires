param(
    [string]$BaseUrl = "http://localhost:5142",
    [Guid]$CivilizationId = "00000000-0000-0000-0000-000000000001",
    [Guid]$PlanetId = "40000000-0000-0000-0000-000000000001",
    [switch]$ApplySeed,
    [string]$SeedProfile = "planet-full-validation",
    [string]$BuildingType
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"
. (Join-Path $PSScriptRoot "dev-qa-common.ps1")

function Invoke-DevGet {
    param([string]$Path)

    try {
        return Invoke-RestMethod -Method Get -Uri ($BaseUrl.TrimEnd("/") + $Path)
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
        return Invoke-RestMethod `
            -Method Post `
            -Uri ($BaseUrl.TrimEnd("/") + $Path) `
            -ContentType "application/json" `
            -Body ($Body | ConvertTo-Json -Depth 8 -Compress)
    }
    catch {
        $response = $_.Exception.Response
        if ($null -ne $response) {
            $reader = New-Object System.IO.StreamReader($response.GetResponseStream())
            $reader.BaseStream.Position = 0
            $reader.DiscardBufferedData()
            $responseBody = $reader.ReadToEnd()
            throw "POST $Path failed with HTTP $([int]$response.StatusCode). Response: $responseBody"
        }

        throw "POST $Path failed. Ensure the Development backend is running at $BaseUrl. $($_.Exception.Message)"
    }
}

function Get-PlanetState {
    Invoke-DevGet "/api/dev/planets/ui-state?civilizationId=$CivilizationId&planetId=$PlanetId"
}

function Format-ResourceDelta {
    param(
        [hashtable]$Before,
        [hashtable]$After
    )

    $keys = @($Before.Keys + $After.Keys | Sort-Object -Unique)
    $lines = foreach ($key in $keys) {
        $beforeValue = if ($Before.ContainsKey($key)) { [decimal]$Before[$key] } else { [decimal]0 }
        $afterValue = if ($After.ContainsKey($key)) { [decimal]$After[$key] } else { [decimal]0 }
        [pscustomobject]@{
            Resource = $key
            Before = $beforeValue
            After = $afterValue
            Delta = $afterValue - $beforeValue
        }
    }

    return $lines
}

if ($ApplySeed) {
    Write-Host "Applying seed profile '$SeedProfile' before enqueue..."
    $seedResponse = Invoke-DevPost "/api/dev/seeds/apply" @{ profile = $SeedProfile }

    if (-not $seedResponse.succeeded) {
        throw "Seed profile '$SeedProfile' did not succeed."
    }
}

Write-Host "Reading planet state before enqueue..."
$beforeResponse = Get-PlanetState
$beforePlanet = $beforeResponse.uiState.planet

if ($null -eq $beforePlanet) {
    throw "Planet UI state did not include a selected planet."
}

$availableActions = @($beforePlanet.constructionActions | Where-Object { $_.availabilityStatus -eq "Available" })
if ($availableActions.Count -eq 0) {
    $openQueueCount = Get-DevQaOpenQueueCount $beforePlanet.constructionQueue
    throw "No available construction actions were found for planet $PlanetId. OpenQueueItems=$openQueueCount VisibleQueueItems=$(@($beforePlanet.constructionQueue).Count)"
}

$selectedAction = if ($BuildingType) {
    $matchingAction = @($availableActions | Where-Object { "$($_.buildingType)" -eq $BuildingType })
    if ($matchingAction.Count -eq 0) {
        $known = @($availableActions | ForEach-Object { "$($_.buildingType)" }) -join ", "
        throw "No available action matched building type '$BuildingType'. Available building types: $known"
    }

    $matchingAction[0]
}
else {
    $availableActions[0]
}

$requestedAtUtc = [DateTime]::UtcNow
$beforeResources = (Get-DevQaResourceMap $beforePlanet.stockpile).Map
$beforeQueueCount = @($beforePlanet.constructionQueue).Count
$beforeOpenQueueCount = Get-DevQaOpenQueueCount $beforePlanet.constructionQueue

Write-Host "Creating a real persisted construction order..."
$enqueueResponse = Invoke-DevPost "/api/dev/buildings/construction-orders/enqueue" @{
    planetId = $PlanetId
    civilizationId = $CivilizationId
    action = "$($selectedAction.action)"
    buildingType = "$($selectedAction.buildingType)"
    requestedAtUtc = $requestedAtUtc.ToString("O")
}

if (-not $enqueueResponse.succeeded) {
    $errors = @($enqueueResponse.errors) -join "; "
    throw "Construction enqueue was rejected. Errors: $errors"
}

Write-Host "Reading planet state after enqueue..."
$afterResponse = Get-PlanetState
$afterPlanet = $afterResponse.uiState.planet
$afterResources = (Get-DevQaResourceMap $afterPlanet.stockpile).Map
$afterQueueCount = @($afterPlanet.constructionQueue).Count
$afterOpenQueueCount = Get-DevQaOpenQueueCount $afterPlanet.constructionQueue

Write-Host ""
Write-Host "Real persisted construction order created."
[pscustomobject]@{
    Planet = $afterPlanet.planetName
    CivilizationId = $CivilizationId
    PlanetId = $PlanetId
    OrderId = $enqueueResponse.orderId
    Action = "$($selectedAction.action)"
    BuildingType = "$($selectedAction.buildingType)"
    TargetLevel = $selectedAction.targetLevel
    RequestedAtUtc = $requestedAtUtc.ToString("O")
    StartsAtUtc = $enqueueResponse.startsAtUtc
    EndsAtUtc = $enqueueResponse.endsAtUtc
} | Format-List

Write-Host ""
Write-Host "Queue delta:"
[pscustomobject]@{
    QueueBefore = $beforeQueueCount
    QueueAfter = $afterQueueCount
    OpenQueueBefore = $beforeOpenQueueCount
    OpenQueueAfter = $afterOpenQueueCount
} | Format-List

Write-Host ""
Write-Host "Resource delta:"
Format-ResourceDelta -Before $beforeResources -After $afterResources | Format-Table -AutoSize

Write-Host ""
Write-Host "Before resources: $((Format-DevQaResourceSummary $beforeResources).Summary)"
Write-Host "After resources:  $((Format-DevQaResourceSummary $afterResources).Summary)"
Write-Host ""
Write-Host "This script creates a real persisted row in the Development database. It does not run migrations, delete data, or reset queues."
