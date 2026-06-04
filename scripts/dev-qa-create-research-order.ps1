param(
    [string]$BaseUrl = "http://localhost:5142",
    [Guid]$CivilizationId = "00000000-0000-0000-0000-000000000001",
    [Guid]$PlanetId = "40000000-0000-0000-0000-000000000001",
    [switch]$ApplySeed,
    [string]$SeedProfile = "research-validation",
    [string]$ResearchType
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

function Get-ResearchState {
    Invoke-DevGet "/api/dev/research/ui-state?civilizationId=$CivilizationId&planetId=$PlanetId"
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

if ($ApplySeed) {
    Write-Host "Applying seed profile '$SeedProfile' before enqueue..."
    $seedResponse = Invoke-DevPost "/api/dev/seeds/apply" @{ profile = $SeedProfile }

    if (-not $seedResponse.succeeded) {
        throw "Seed profile '$SeedProfile' did not succeed."
    }
}

Write-Host "Reading research state before enqueue..."
$beforeResearchResponse = Get-ResearchState
$beforeResearch = $beforeResearchResponse.uiState

if ($null -eq $beforeResearch) {
    throw "Research UI state did not include a selected research context."
}

Write-Host "Reading planet state before enqueue..."
$beforePlanetResponse = Get-PlanetState
$beforePlanet = $beforePlanetResponse.uiState.planet

if ($null -eq $beforePlanet) {
    throw "Planet UI state did not include a selected planet."
}

$availableHints = @($beforeResearch.technologyHints | Where-Object { $_.canEnqueue -and $null -ne $_.enqueueCommand })
if ($availableHints.Count -eq 0) {
    $openQueueCount = Get-DevQaOpenQueueCount $beforeResearch.queue
    if ($openQueueCount -gt 0) {
        throw "No available research hints were found because the research queue is already occupied. OpenQueueItems=$openQueueCount VisibleQueueItems=$(@($beforeResearch.queue).Count)"
    }

    throw "No available research hints were found for civilization $CivilizationId on planet $PlanetId."
}

$selectedHint = if ($ResearchType) {
    $matchingHints = @($availableHints | Where-Object { "$($_.researchType)" -eq $ResearchType })
    if ($matchingHints.Count -eq 0) {
        $known = @($availableHints | ForEach-Object { "$($_.researchType)" }) -join ", "
        throw "No available research hint matched research type '$ResearchType'. Available research types: $known"
    }

    $matchingHints[0]
}
else {
    $availableHints[0]
}

$command = $selectedHint.enqueueCommand
$requestedAtUtc = [DateTime]::UtcNow
$beforeResources = (Get-DevQaResourceMap $beforePlanet.stockpile).Map
$beforeQueueCount = @($beforeResearch.queue).Count
$beforeOpenQueueCount = Get-DevQaOpenQueueCount $beforeResearch.queue
$enqueuePayload = [ordered]@{
    civilizationId = $command.civilizationId
    sourcePlanetId = $command.sourcePlanetId
    researchType = "$($command.researchType)"
    requestedAtUtc = $requestedAtUtc.ToString("O")
}

Write-Host "Creating a real persisted research order..."
Write-Host "Selected research: researchType=$($selectedHint.researchType) nextLevel=$($selectedHint.nextLevel)"
Write-Host "Payload summary: $(Format-DevQaPayloadSummary $enqueuePayload)"
$enqueueResult = Invoke-DevPostDetailed $command.route $enqueuePayload

if (-not $enqueueResult.Succeeded) {
    if ($enqueueResult.StatusCode -eq 409 -and (Test-DevQaResponseHasKnownError -ResponseObject $enqueueResult.Json -FallbackText $enqueueResult.BodyText -KnownErrorFragment "already has an open research order")) {
        Write-Host "Ya existe una investigacion abierta para esta civilizacion. No se crea otra orden."
        Write-Host "Reading research state again to summarize the current queue..."
        $occupiedResearchResponse = Get-ResearchState
        $occupiedResearch = $occupiedResearchResponse.uiState
        $occupiedQueueCount = @($occupiedResearch.queue).Count
        $occupiedOpenQueueCount = Get-DevQaOpenQueueCount $occupiedResearch.queue
        $currentOrder = @($occupiedResearch.queue | Where-Object { "$($_.status)" -in @("Pending", "Active") } | Select-Object -First 1)

        [pscustomobject]@{
            QueueCount = $occupiedQueueCount
            OpenQueueCount = $occupiedOpenQueueCount
            CurrentResearch = if ($currentOrder.Count -gt 0) { "$($currentOrder[0].researchType)" } else { "none" }
            CurrentStatus = if ($currentOrder.Count -gt 0) { "$($currentOrder[0].status)" } else { "none" }
        } | Format-List

        Write-Host "Estado esperado para una base de datos de Development reutilizada. La script finaliza sin crear otra orden."
        exit 0
    }

    $errors = Get-DevQaResponseErrorText -ResponseObject $enqueueResult.Json -FallbackText $enqueueResult.BodyText
    $details = if (-not [string]::IsNullOrWhiteSpace($errors)) { $errors } else { $enqueueResult.BodyText }
    throw "Research enqueue failed with HTTP $($enqueueResult.StatusCode). $details"
}

$enqueueResponse = $enqueueResult.Json

Write-Host "Reading research state after enqueue..."
$afterResearchResponse = Get-ResearchState
$afterResearch = $afterResearchResponse.uiState

Write-Host "Reading planet state after enqueue..."
$afterPlanetResponse = Get-PlanetState
$afterPlanet = $afterPlanetResponse.uiState.planet
$afterResources = (Get-DevQaResourceMap $afterPlanet.stockpile).Map
$afterQueueCount = @($afterResearch.queue).Count
$afterOpenQueueCount = Get-DevQaOpenQueueCount $afterResearch.queue

Write-Host ""
Write-Host "Real persisted research order created."
[pscustomobject]@{
    Planet = $afterResearch.selectedPlanetName
    CivilizationId = $CivilizationId
    PlanetId = $PlanetId
    OrderId = $enqueueResponse.orderId
    ResearchType = "$($command.researchType)"
    TargetLevel = $command.targetLevel
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
