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
$enqueuePayload = [ordered]@{
    planetId = $PlanetId
    civilizationId = $CivilizationId
    action = $selectedAction.action
    buildingType = $selectedAction.buildingType
    requestedAtUtc = $requestedAtUtc.ToString("O")
}

Write-Host "Creating a real persisted construction order..."
$selectedLabel = if ($null -ne $selectedAction.display) {
    "$($selectedAction.display.actionLabel) $($selectedAction.display.buildingTypeLabel)".Trim()
}
else {
    "$($selectedAction.action) $($selectedAction.buildingType)"
}
Write-Host "Selected action: action=$($selectedAction.action) buildingType=$($selectedAction.buildingType) targetLevel=$($selectedAction.targetLevel) label=$selectedLabel"
Write-Host "Payload summary: $(Format-DevQaPayloadSummary $enqueuePayload)"
$enqueueResult = Invoke-DevPostDetailed "/api/dev/buildings/construction-orders/enqueue" $enqueuePayload

if (-not $enqueueResult.Succeeded) {
    $responseErrors = Get-DevQaResponseErrorText $enqueueResult.Json
    if ($enqueueResult.StatusCode -eq 400) {
        $sanitizedPayload = Format-DevQaPayloadSummary $enqueuePayload
        $responseText = if ([string]::IsNullOrWhiteSpace($enqueueResult.BodyText)) { "<no body>" } else { $enqueueResult.BodyText }
        $spanishSummary = switch -Regex ($responseErrors) {
            "Planet already has an open construction order\." { "El planeta ya tiene una construccion abierta."; break }
            "Planet is not owned by the requesting civilization\." { "El planeta no pertenece a la civilizacion solicitante."; break }
            "Insufficient resources\." { "No hay recursos suficientes para esta orden."; break }
            default { "No se pudo enviar la orden de construccion con el contrato actual." }
        }

        throw "Construction enqueue failed with HTTP 400.`nResumen: $spanishSummary`nEndpoint: /api/dev/buildings/construction-orders/enqueue`nSelectedOptionLabel: $selectedLabel`nSelectedBackendAction: $($selectedAction.action)`nPayload: $sanitizedPayload`nResponse: $responseText"
    }

    $details = if (-not [string]::IsNullOrWhiteSpace($responseErrors)) { $responseErrors } else { $enqueueResult.BodyText }
    throw "Construction enqueue failed with HTTP $($enqueueResult.StatusCode). $details"
}

$enqueueResponse = $enqueueResult.Json

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
    Action = $selectedAction.action
    BuildingType = $selectedAction.buildingType
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
