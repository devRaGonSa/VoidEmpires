param(
    [string]$BaseUrl = "http://localhost:5142",
    [Guid]$CivilizationId = "00000000-0000-0000-0000-000000000001",
    [Guid]$PlanetId = "40000000-0000-0000-0000-000000000001"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

Write-Warning "Este script modifica la base de datos de Development para preparar QA manual de Construccion."

try {
    $request = [ordered]@{
        civilizationId = $CivilizationId
        planetId = $PlanetId
    }

    Write-Host "Invocando endpoint de preparacion para Civilization=$CivilizationId Planet=$PlanetId..."
    $response = Invoke-RestMethod `
        -Method Post `
        -Uri ($BaseUrl.TrimEnd("/") + "/api/dev/construction/qa-state/prepare") `
        -ContentType "application/json" `
        -Body ($request | ConvertTo-Json -Depth 6 -Compress)

    if (-not $response.succeeded) {
        $errors = if ($null -ne $response.errors) {
            ($response.errors | ForEach-Object { "$_" }) -join "; "
        }
        else {
            "unknown error"
        }

        throw "No se pudo preparar el estado de QA. Error(es): $errors"
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
    Write-Host "Preparacion completada."
    Write-Host "Sucedio: $($response.succeeded)"
    Write-Host "Civilizacion: $targetCivilizationId"
    Write-Host "Planeta:      $targetPlanetId"
    Write-Host "Antes abiertos: $($response.blockingOrdersBefore)"
    Write-Host "Despues abiertos: $($response.blockingOrdersAfter)"
    Write-Host "Recursos antes: $resourcesBefore"
    Write-Host "Recursos despues: $resourcesAfter"
    if (-not [string]::IsNullOrWhiteSpace($notes)) {
        Write-Host ""
        Write-Host "Notas:"
        Write-Host $notes
    }
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
        throw "No se pudo invocar el endpoint. HTTP $statusCode. Body: $body"
    }
    finally {
        $reader.Dispose()
    }
}
