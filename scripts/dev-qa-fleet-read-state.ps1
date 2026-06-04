param(
    [string]$BaseUrl = "http://localhost:5142",
    [Guid]$CivilizationId = "00000000-0000-0000-0000-000000000001"
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

Write-Host "Parametros: BaseUrl=$BaseUrl, CivilizationId=$CivilizationId"
Write-Host "Reading fleet UI state..."
$response = Invoke-DevGet "/api/dev/fleets/ui-state?civilizationId=$CivilizationId"
$uiState = $response.uiState

if ($null -eq $uiState) {
    throw "Fleet UI state response did not include a readable uiState payload."
}

$snapshot = Get-DevQaFleetSnapshot -FleetUiState $uiState -PlanetId ([Guid]::Empty)
$groups = Get-DevQaEnumerable $uiState.groups
$transfers = Format-DevQaFleetTransferSummary $groups
$stationedGroups = @($groups | Where-Object { "$($_.status)" -eq "Stationed" })

Write-Host ""
Write-Host "Fleet read-state snapshot:"
[pscustomobject]@{
    CivilizationId = $CivilizationId
    Escuadras = $snapshot.GroupCount
    Estacionadas = $snapshot.StationedCount
    TransferenciasActivas = $snapshot.ActiveTransferCount
    ContextoLocal = $snapshot.ResourceContext
    ContextosDeRecursos = $snapshot.ResourceContextCount
} | Format-List

if ($snapshot.ResourceWarnings.Count -gt 0) {
    Write-Warning ($snapshot.ResourceWarnings -join " ")
}

if (@($groups).Count -eq 0) {
    Write-Host "No hay grupos orbitales visibles para esta civilizacion. El endpoint sigue respondiendo con un estado valido y no se intenta ninguna mutacion."
    exit 0
}

Write-Host ""
Write-Host "Stationed group summary:"
$stationedGroups |
    Select-Object `
        @{ Name = "AssetType"; Expression = { $_.assetType } }, `
        @{ Name = "Quantity"; Expression = { $_.quantity } }, `
        @{ Name = "CurrentPlanetId"; Expression = { $_.currentPlanetId } }, `
        @{ Name = "Status"; Expression = { $_.status } } |
    Format-Table -AutoSize

if (@($transfers).Count -gt 0) {
    Write-Host ""
    Write-Host "Transfer summary:"
    $transfers | Format-Table -AutoSize
}
else {
    Write-Host ""
    Write-Host "Transfer summary: no hay transferencias activas visibles."
}

Write-Host ""
Write-Host "Nota operativa: esta script es solo de lectura. No crea transferencias, no crea grupos, no divide, no fusiona, no cancela y no completa ningun estado de Flotas."
