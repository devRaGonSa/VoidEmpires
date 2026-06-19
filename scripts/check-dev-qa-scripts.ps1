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

function Get-ExistingDevQaScriptPaths {
    $requiredScriptNames = @(
        "dev-qa-common.ps1",
        "dev-qa-baseline.ps1",
        "dev-qa-create-construction-order.ps1",
        "dev-qa-prepare-construction-ui-state.ps1",
        "dev-qa-create-research-order.ps1",
        "dev-qa-prepare-research-ui-state.ps1",
        "dev-qa-create-shipyard-production-order.ps1",
        "dev-qa-prepare-orbital-production-ui-state.ps1",
        "dev-qa-fleet-read-state.ps1",
        "dev-qa-prepare-playable-session-state.ps1",
        "dev-qa-materialize-due-queues.ps1",
        "dev-qa-get-playable-session-diagnostics.ps1",
        "dev-qa-playable-loop-guide.ps1"
    )

    $optionalScriptNames = @(
        "dev-qa-create-orbital-group-from-stock.ps1",
        "sqlserver-script-migration.ps1",
        "sqlserver-connection-smoke.ps1",
        "sqlserver-final-catalog-seed.ps1"
    )

    $paths = New-Object System.Collections.Generic.List[string]

    foreach ($scriptName in $requiredScriptNames) {
        $scriptPath = Join-Path $PSScriptRoot $scriptName
        if (-not (Test-Path -LiteralPath $scriptPath)) {
            throw "Required QA script '$scriptName' was not found."
        }

        $paths.Add($scriptPath)
    }

    foreach ($scriptName in $optionalScriptNames) {
        $scriptPath = Join-Path $PSScriptRoot $scriptName
        if (Test-Path -LiteralPath $scriptPath) {
            $paths.Add($scriptPath)
        }
    }

    return @($paths)
}

$scriptPaths = Get-ExistingDevQaScriptPaths

$parseFailures = New-Object System.Collections.Generic.List[string]

foreach ($scriptPath in $scriptPaths) {
    $tokens = $null
    $errors = $null
    [void][System.Management.Automation.Language.Parser]::ParseFile($scriptPath, [ref]$tokens, [ref]$errors)

    if ($errors.Count -gt 0) {
        foreach ($parseError in $errors) {
            $parseFailures.Add("${scriptPath}:$($parseError.Extent.StartLineNumber): $($parseError.Message)")
        }
    }
}

if ($parseFailures.Count -gt 0) {
    throw "PowerShell parser errors were found:`n$($parseFailures -join [Environment]::NewLine)"
}

$frontendRouteGuardPath = Join-Path $PSScriptRoot "check-frontend-route-lazy-imports.ps1"
if (Test-Path -LiteralPath $frontendRouteGuardPath) {
    & $frontendRouteGuardPath
}

$frontendCopyRegressionPath = Join-Path $PSScriptRoot "check-frontend-copy-regressions.ps1"
if (Test-Path -LiteralPath $frontendCopyRegressionPath) {
    & $frontendCopyRegressionPath
}

$repoSecretScanPath = Join-Path $PSScriptRoot "check-repo-secret-scan.ps1"
if (Test-Path -LiteralPath $repoSecretScanPath) {
    & $repoSecretScanPath
}

function Assert-ResourceSummaryContains {
    param(
        [Parameter(Mandatory = $true)]
        [object]$InputObject,
        [Parameter(Mandatory = $true)]
        [string[]]$ExpectedFragments
    )

    $summary = Format-DevQaResourceSummary $InputObject
    foreach ($fragment in $ExpectedFragments) {
        if ($summary.Summary -notlike "*$fragment*") {
            throw "Expected resource summary to contain '$fragment' but got '$($summary.Summary)'."
        }
    }
}

Assert-ResourceSummaryContains -InputObject @(
    [pscustomobject]@{ resourceType = "Credits"; amount = 1250 },
    [pscustomobject]@{ resourceType = "Metal"; amount = 40 }
) -ExpectedFragments @("Creditos=1250", "Metal=40")

Assert-ResourceSummaryContains -InputObject ([pscustomobject]@{
    credits = 1000
    metal = 80
    crystal = 60
    gas = 20
}) -ExpectedFragments @("Creditos=1000", "Metal=80", "Cristal=60", "Gas=20")

$unknownShape = Format-DevQaResourceSummary ([pscustomobject]@{ unexpected = 1 })
if ($unknownShape.Summary -notlike "warning:*") {
    throw "Expected unknown resource shapes to return a warning summary."
}

$shipyardSnapshot = Get-DevQaShipyardSnapshot ([pscustomobject]@{
    shipyard = [pscustomobject]@{
        planetName = "Aurelia"
        resourceStockpile = @(
            [pscustomobject]@{ resourceType = "Credits"; quantity = 180 },
            [pscustomobject]@{ resourceType = "Metal"; quantity = 120 }
        )
        catalog = @(
            [pscustomobject]@{ availabilityStatus = "Available" },
            [pscustomobject]@{ availabilityStatus = "Blocked" }
        )
        queue = @([pscustomobject]@{ id = 1 })
        orbitalStock = @([pscustomobject]@{ assetType = "ScoutCraft"; quantity = 1 })
    }
})
if ($shipyardSnapshot.Planet -ne "Aurelia" -or $shipyardSnapshot.AvailableOptions -ne 1 -or $shipyardSnapshot.BlockedOptions -ne 1 -or $shipyardSnapshot.QueueCount -ne 1 -or $shipyardSnapshot.StockCount -ne 1) {
    throw "Expected shipyard snapshot helper to summarize the current DTO shape."
}

$fleetSnapshot = Get-DevQaFleetSnapshot -PlanetId ([Guid]"40000000-0000-0000-0000-000000000001") -FleetUiState ([pscustomobject]@{
    groups = @(
        [pscustomobject]@{ status = "Stationed"; hasActiveTransfer = $false },
        [pscustomobject]@{ status = "Reserved"; hasActiveTransfer = $true }
    )
    resourceContexts = @(
        [pscustomobject]@{
            planetId = [Guid]"40000000-0000-0000-0000-000000000001"
            balances = @(
                [pscustomobject]@{ resourceType = "Credits"; quantity = 90 },
                [pscustomobject]@{ resourceType = "Metal"; quantity = 30 }
            )
        }
    )
})
if ($fleetSnapshot.GroupCount -ne 2 -or $fleetSnapshot.StationedCount -ne 1 -or $fleetSnapshot.ActiveTransferCount -ne 1 -or $fleetSnapshot.ResourceContextCount -ne 1) {
    throw "Expected fleet snapshot helper to summarize groups, transfers, and local resource contexts."
}

$stockSummary = Format-DevQaStockSummary @(
    [pscustomobject]@{ assetType = "ScoutCraft"; quantity = 1 },
    [pscustomobject]@{ assetType = "EscortCraft"; quantity = 4 }
)
if ($stockSummary.Count -ne 2 -or $stockSummary.TotalQuantity -ne 5 -or $stockSummary.Summary -notlike "*Nave exploradora=1*" -or $stockSummary.Summary -notlike "*Nave escolta=4*") {
    throw "Expected stock summary helper to summarize shipyard stock rows."
}

$transferSummary = Format-DevQaFleetTransferSummary @(
    [pscustomobject]@{
        assetType = "ScoutCraft"
        quantity = 2
        activeTransfer = [pscustomobject]@{
            destinationPlanetId = "40000000-0000-0000-0000-000000000002"
            status = "Planned"
            arrivalAtUtc = "2026-01-01T12:05:00Z"
        }
    }
)
if (@($transferSummary).Count -ne 1 -or $transferSummary[0].AssetType -ne "Nave exploradora" -or $transferSummary[0].Status -ne "Planned") {
    throw "Expected fleet transfer summary helper to summarize active transfer rows."
}

$emptyTransferSummary = Format-DevQaFleetTransferSummary @(
    [pscustomobject]@{
        assetType = "ScoutCraft"
        quantity = 2
    }
)
if (@($emptyTransferSummary).Count -ne 0) {
    throw "Expected fleet transfer summary helper to ignore groups without an active transfer."
}

$constructionPayloadSummary = Format-DevQaPayloadSummary ([ordered]@{
    planetId = "00000000-0000-0000-0000-000000000001"
    civilizationId = "00000000-0000-0000-0000-000000000002"
    action = 0
    buildingType = 1
    requestedAtUtc = "2026-01-01T00:00:00.0000000Z"
})
if ($constructionPayloadSummary -like "*action=Construir*" -or $constructionPayloadSummary -like "*action=Mejorar*") {
    throw "Construction payload summary should use backend-compatible action values, not display labels."
}

$known409Response = [pscustomobject]@{
    errors = @("Civilization already has an open research order.")
}
if (-not (Test-DevQaResponseHasKnownError -ResponseObject $known409Response -KnownErrorFragment "already has an open research order")) {
    throw "Expected known research 409 detection helper to match the current backend error."
}

$known409JsonText = '{"succeeded":false,"orderId":null,"startsAtUtc":null,"endsAtUtc":null,"errors":["Civilization already has an open research order."]}'
if (-not (Test-DevQaResponseHasKnownError -ResponseObject $null -FallbackText $known409JsonText -KnownErrorFragment "already has an open research order")) {
    throw "Expected known research 409 detection helper to match the current backend JSON body text."
}

$shipyardOpenOrderResponse = [pscustomobject]@{
    errors = @("Planet already has an open asset production order.")
}
if (-not (Test-DevQaResponseHasKnownError -ResponseObject $shipyardOpenOrderResponse -KnownErrorFragment "open asset production order")) {
    throw "Expected known shipyard 409 detection helper to match the current backend error."
}

$shipyardOpenOrderJsonText = '{"succeeded":false,"orderId":null,"startsAtUtc":null,"endsAtUtc":null,"errors":["Planet already has an open asset production order."]}'
if (-not (Test-DevQaResponseHasKnownError -ResponseObject $null -FallbackText $shipyardOpenOrderJsonText -KnownErrorFragment "open asset production order")) {
    throw "Expected known shipyard 409 detection helper to match the current backend JSON body text."
}

$offlineMessage = Format-DevQaBackendUnavailableMessage `
    -BaseUrl "http://localhost:5142" `
    -Method "GET" `
    -Path "/api/dev/seeds/profiles" `
    -Detail "connection refused"
if ($offlineMessage -notlike "*dotnet run --project .\src\VoidEmpires.Web*" -or $offlineMessage -notlike "*GET /api/dev/seeds/profiles*" -or $offlineMessage -notlike "*connection refused*") {
    throw "Expected backend-offline helper to print the start command, endpoint, and detail."
}

$copySafeCommand = Format-DevQaPowerShellCommand `
    -ScriptName "dev-qa-materialize-due-queues.ps1" `
    -Parameters ([ordered]@{
        BaseUrl = "http://localhost:5142"
        CivilizationId = "00000000-0000-0000-0000-000000000001"
        PlanetId = "40000000-0000-0000-0000-000000000001"
        ElapsedSeconds = 3600
    })
if ($copySafeCommand -notlike 'powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\dev-qa-materialize-due-queues.ps1*' -or $copySafeCommand -notlike '*-BaseUrl "http://localhost:5142"*' -or $copySafeCommand -notlike '*-ElapsedSeconds "3600"*') {
    throw "Expected command formatter to produce a copy-safe PowerShell command."
}

Write-Host "Persisted QA PowerShell scripts parsed successfully."
Write-Host "Resource-format, command, offline-error, and payload helper checks passed."
