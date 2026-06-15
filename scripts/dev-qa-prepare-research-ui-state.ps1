param(
    [string]$BaseUrl = "http://localhost:5142",
    [Guid]$CivilizationId = "00000000-0000-0000-0000-000000000001",
    [Guid]$SourcePlanetId = "40000000-0000-0000-0000-000000000001",
    [Guid]$PlanetId
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

$targetPlanetId = if ($PSBoundParameters.ContainsKey("PlanetId")) { $PlanetId } else { $SourcePlanetId }

Write-Warning "Este script modifica la base de datos de Development para preparar QA manual de Investigacion."
Write-Host ""
Write-Host "Target"
Write-Host "BaseUrl: $BaseUrl"
Write-Host "CivilizationId: $CivilizationId"
Write-Host "SourcePlanetId: $targetPlanetId"
Write-Host "Action: Prepare Research QA state"

try {
    $request = [ordered]@{
        civilizationId = $CivilizationId
        sourcePlanetId = $targetPlanetId
        planetId = $targetPlanetId
    }

    Write-Host "Invocando endpoint de preparacion para Civilization=$CivilizationId SourcePlanet=$targetPlanetId..."
    $response = Invoke-RestMethod `
        -Method Post `
        -Uri ($BaseUrl.TrimEnd("/") + "/api/dev/research/qa-state/prepare") `
        -ContentType "application/json" `
        -Body ($request | ConvertTo-Json -Depth 6 -Compress)

    if (-not $response.succeeded) {
        $errors = if ($null -ne $response.errors) {
            ($response.errors | ForEach-Object { "$_" }) -join "; "
        }
        else {
            "unknown error"
        }

        throw "No se pudo preparar el estado de QA de Investigacion. Error(es): $errors"
    }

    $notes = if ($null -ne $response.notes) { ($response.notes | ForEach-Object { " - $_" }) -join [Environment]::NewLine } else { "" }
    $targetCivilizationId = if ($null -ne $response.result -and $null -ne $response.result.civilizationId) {
        $response.result.civilizationId
    }
    else {
        $CivilizationId
    }

    $preparedSourcePlanetId = if ($null -ne $response.result -and $null -ne $response.result.sourcePlanetId) {
        $response.result.sourcePlanetId
    }
    else {
        $targetPlanetId
    }

    $resourcesBefore = if ($null -ne $response.resourcesBefore) {
        "Credits={0}, Metal={1}, Crystal={2}, Gas={3}" -f $response.resourcesBefore.credits, $response.resourcesBefore.metal, $response.resourcesBefore.crystal, $response.resourcesBefore.gas
    }
    else {
        "<sin stockpile legible>"
    }
    $resourcesAfter = if ($null -ne $response.resourcesAfter) {
        "Credits={0}, Metal={1}, Crystal={2}, Gas={3}" -f $response.resourcesAfter.credits, $response.resourcesAfter.metal, $response.resourcesAfter.crystal, $response.resourcesAfter.gas
    }
    else {
        "<sin stockpile legible>"
    }

    Write-Host ""
    Write-Host "Preparacion completada."
    Write-Host "Sucedio: $($response.succeeded)"
    Write-Host "Civilizacion:   $targetCivilizationId"
    Write-Host "Planeta origen: $preparedSourcePlanetId"
    Write-Host "Antes abiertos: $($response.blockingOrdersBefore)"
    Write-Host "Despues abiertos: $($response.blockingOrdersAfter)"
    Write-Host "Recursos antes: $resourcesBefore"
    Write-Host "Recursos despues: $resourcesAfter"
    if (-not [string]::IsNullOrWhiteSpace($notes)) {
        Write-Host ""
        Write-Host "Notas:"
        Write-Host $notes
    }

    Write-Host ""
    Write-Host "Next suggested command:"
    Write-Host "powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\dev-qa-create-research-order.ps1 -BaseUrl `"$($BaseUrl.TrimEnd("/"))`" -CivilizationId $targetCivilizationId -PlanetId $preparedSourcePlanetId"
}
catch {
    $response = $_.Exception.Response
    if ($null -eq $response) {
        throw "Backend no disponible en '$BaseUrl' o error de red. Verifica que `dotnet run --project .\src\VoidEmpires.Web` este activo y que no exista un bloqueo de conectividad."
    }

    $statusCode = [int]$response.StatusCode
    $reader = New-Object System.IO.StreamReader($response.GetResponseStream())
    try {
        if ($reader.BaseStream.CanSeek) {
            $reader.BaseStream.Position = 0
        }
        $reader.DiscardBufferedData()
        $body = $reader.ReadToEnd()
        throw "No se pudo invocar el endpoint de preparacion de Investigacion. HTTP $statusCode. Body: $body"
    }
    finally {
        $reader.Dispose()
    }
}
