param(
    [string]$BaseUrl = "http://localhost:5142",
    [Guid]$CivilizationId = "00000000-0000-0000-0000-000000000001",
    [Guid]$PlanetId = "40000000-0000-0000-0000-000000000001",
    [switch]$ApplySeed,
    [string]$SeedProfile = "cockpit-validation",
    [string]$SpaceAssetType
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

function Get-ShipyardState {
    Invoke-DevGet "/api/dev/shipyard/ui-state?civilizationId=$CivilizationId&planetId=$PlanetId"
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

Write-Host "Parametros: BaseUrl=$BaseUrl, CivilizationId=$CivilizationId, PlanetId=$PlanetId, ApplySeed=$ApplySeed, SeedProfile=$SeedProfile"
Write-Host "Reading shipyard state before enqueue..."
$beforeResponse = Get-ShipyardState
$beforeShipyard = $beforeResponse.uiState.shipyard

if ($null -eq $beforeShipyard) {
    throw "Shipyard UI state did not include a selected shipyard context."
}

$beforeResources = (Get-DevQaResourceMap $beforeShipyard.resourceStockpile).Map
$beforeQueueCount = @($beforeShipyard.queue).Count
$beforeOpenQueueCount = Get-DevQaOpenQueueCount $beforeShipyard.queue
$beforeStock = Format-DevQaStockSummary $beforeShipyard.orbitalStock

$availableItems = @($beforeShipyard.catalog | Where-Object { $_.availabilityStatus -eq "Available" -and $null -ne $_.enqueueCommand })

if ($availableItems.Count -eq 0) {
    $blockedReason = @($beforeShipyard.catalog | Select-Object -First 1 | ForEach-Object { $_.availabilityReason }) -join ", "
    Write-Host "No hay una produccion orbital disponible para enviar en este estado de Development."
    [pscustomobject]@{
        Planeta = $beforeShipyard.planetName
        QueueVisible = $beforeQueueCount
        OpenQueueItems = $beforeOpenQueueCount
        EnqueueSupported = $beforeShipyard.actionSummary.enqueueSupported
        Motivo = if ($beforeOpenQueueCount -gt 0) { "Planet already has an open asset production order." } elseif (-not [string]::IsNullOrWhiteSpace($blockedReason)) { $blockedReason } else { "$($beforeShipyard.actionSummary.enqueueActionReason)" }
        StockVisible = $beforeStock.Summary
    } | Format-List
    Write-Host "Estado esperado cuando la cola ya esta ocupada o el catalogo no ofrece una opcion segura. La script termina sin mutar nada."
    exit 0
}

$selectedItem = if ($SpaceAssetType) {
    $matchingItems = @($availableItems | Where-Object { "$($_.assetType)" -eq $SpaceAssetType })
    if ($matchingItems.Count -eq 0) {
        $known = @($availableItems | ForEach-Object { "$($_.assetType)" }) -join ", "
        throw "No available shipyard option matched asset type '$SpaceAssetType'. Available asset types: $known"
    }

    $matchingItems[0]
}
else {
    $availableItems[0]
}

$command = $selectedItem.enqueueCommand
$requestedAtUtc = [DateTime]::UtcNow
$enqueuePayload = [ordered]@{
    civilizationId = $command.civilizationId
    planetId = $command.planetId
    target = [int]$command.target
    spaceAssetType = [int]$command.spaceAssetType
    quantity = $command.quantity
    requestedAtUtc = $requestedAtUtc.ToString("O")
}

Write-Host "Creating a real persisted shipyard production order..."
Write-Host "Selected asset: assetType=$($selectedItem.assetType) quantity=$($command.quantity) duration=$($selectedItem.estimatedDuration) availability=$($selectedItem.availabilityReason)"
Write-Host "Payload summary: $(Format-DevQaPayloadSummary $enqueuePayload)"
$enqueueResult = Invoke-DevPostDetailed $command.route $enqueuePayload

if (-not $enqueueResult.Succeeded) {
    if ($enqueueResult.StatusCode -eq 409 -and (Test-DevQaResponseHasKnownError -ResponseObject $enqueueResult.Json -FallbackText $enqueueResult.BodyText -KnownErrorFragment "open asset production order")) {
        Write-Host "Ya existe una orden de produccion orbital abierta en este planeta. No se crea otra orden."
        Write-Host "Reading shipyard state again to summarize the current queue..."
        $occupiedResponse = Get-ShipyardState
        $occupiedShipyard = $occupiedResponse.uiState.shipyard
        $occupiedQueueCount = @($occupiedShipyard.queue).Count
        $occupiedOpenQueueCount = Get-DevQaOpenQueueCount $occupiedShipyard.queue
        $occupiedStock = Format-DevQaStockSummary $occupiedShipyard.orbitalStock

        [pscustomobject]@{
            Planeta = $occupiedShipyard.planetName
            QueueCount = $occupiedQueueCount
            OpenQueueCount = $occupiedOpenQueueCount
            StockVisible = $occupiedStock.Summary
            EnqueueReason = "$($occupiedShipyard.actionSummary.enqueueActionReason)"
        } | Format-List

        Write-Host "Estado esperado para una base de datos de Development reutilizada. La script finaliza sin crear otra orden."
        exit 0
    }

    $errors = Get-DevQaResponseErrorText -ResponseObject $enqueueResult.Json -FallbackText $enqueueResult.BodyText
    $details = if (-not [string]::IsNullOrWhiteSpace($errors)) { $errors } else { $enqueueResult.BodyText }
    throw "Shipyard enqueue failed with HTTP $($enqueueResult.StatusCode). $details"
}

$enqueueResponse = $enqueueResult.Json

Write-Host "Reading shipyard state after enqueue..."
$afterResponse = Get-ShipyardState
$afterShipyard = $afterResponse.uiState.shipyard
$afterResources = (Get-DevQaResourceMap $afterShipyard.resourceStockpile).Map
$afterQueueCount = @($afterShipyard.queue).Count
$afterOpenQueueCount = Get-DevQaOpenQueueCount $afterShipyard.queue
$afterStock = Format-DevQaStockSummary $afterShipyard.orbitalStock

Write-Host ""
Write-Host "Real persisted shipyard production order created."
[pscustomobject]@{
    Planeta = $afterShipyard.planetName
    CivilizationId = $CivilizationId
    PlanetId = $PlanetId
    OrderId = $enqueueResponse.orderId
    AssetType = "$($selectedItem.assetType)"
    Quantity = $command.quantity
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
Write-Host "Before stock:     $($beforeStock.Summary)"
Write-Host "After stock:      $($afterStock.Summary)"
Write-Host ""
Write-Host "Nota operativa: esta script crea una fila real en la base Development. No ejecuta migraciones, no borra datos, no completa produccion vencida y no muta estado de Flotas."
