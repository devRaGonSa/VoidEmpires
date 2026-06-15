param(
    [string]$BaseUrl = "http://localhost:5142",
    [Guid]$CivilizationId = "00000000-0000-0000-0000-000000000001",
    [Guid]$PlanetId = "40000000-0000-0000-0000-000000000001"
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

Write-Warning "Este script modifica la base de datos de Development para preparar QA manual de produccion orbital."
Write-Host ""
Write-Host "Target"
Write-Host "BaseUrl: $BaseUrl"
Write-Host "CivilizationId: $CivilizationId"
Write-Host "PlanetId: $PlanetId"
Write-Host "Action: Prepare Shipyard QA state"

try {
    $request = [ordered]@{
        civilizationId = $CivilizationId
        planetId = $PlanetId
    }

    Write-Host "Invocando endpoint de preparacion orbital para Civilization=$CivilizationId Planet=$PlanetId..."
    $response = Invoke-RestMethod `
        -Method Post `
        -Uri ($BaseUrl.TrimEnd("/") + "/api/dev/shipyard/qa-state/prepare") `
        -ContentType "application/json" `
        -Body ($request | ConvertTo-Json -Depth 6 -Compress)

    if (-not $response.succeeded) {
        $errors = if ($null -ne $response.errors) {
            ($response.errors | ForEach-Object { "$_" }) -join "; "
        }
        else {
            "unknown error"
        }

        throw "No se pudo preparar el estado de QA orbital. Error(es): $errors"
    }

    $notes = if ($null -ne $response.notes) { ($response.notes | ForEach-Object { " - $_" }) -join [Environment]::NewLine } else { "" }
    $targetCivilizationId = if ($null -ne $response.result -and $null -ne $response.result.civilizationId) {
        $response.result.civilizationId
    }
    else {
        $CivilizationId
    }

    $targetPlanetId = if ($null -ne $response.result -and $null -ne $response.result.planetId) {
        $response.result.planetId
    }
    else {
        $PlanetId
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
    Write-Host "Preparacion orbital completada."
    Write-Host "Sucedio: $($response.succeeded)"
    Write-Host "Civilizacion:   $targetCivilizationId"
    Write-Host "Planeta:        $targetPlanetId"
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
    Write-Host "powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\dev-qa-create-shipyard-production-order.ps1 -BaseUrl `"$($BaseUrl.TrimEnd("/"))`" -CivilizationId $targetCivilizationId -PlanetId $targetPlanetId"
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
        throw "No se pudo invocar el endpoint de preparacion orbital. HTTP $statusCode. Body: $body"
    }
    finally {
        $reader.Dispose()
    }
}
