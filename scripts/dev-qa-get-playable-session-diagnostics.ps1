param(
    [string]$BaseUrl = "http://localhost:5142",
    [Guid]$CivilizationId = "00000000-0000-0000-0000-000000000001",
    [Guid]$PlanetId = "40000000-0000-0000-0000-000000000001",
    [switch]$RawJson
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

Write-Host "Target"
Write-Host "BaseUrl: $BaseUrl"
Write-Host "CivilizationId: $CivilizationId"
Write-Host "PlanetId: $PlanetId"
Write-Host "Action: Read playable-session diagnostics"

try {
    $endpoint = "{0}/api/dev/playable-session/diagnostics?civilizationId={1}&planetId={2}" -f `
        $BaseUrl.TrimEnd("/"), `
        $CivilizationId, `
        $PlanetId

    $response = Invoke-RestMethod -Method Get -Uri $endpoint

    if (-not $response.succeeded) {
        $errors = if ($null -ne $response.errors) { ($response.errors | ForEach-Object { "$_" }) -join "; " } else { "unknown error" }
        throw "No se pudieron leer los diagnosticos. Error(es): $errors"
    }

    $diagnostics = $response.diagnostics
    if ($null -eq $diagnostics) {
        throw "El endpoint no devolvio un bloque de diagnosticos."
    }

    $resourceSummary = ($diagnostics.resources | ForEach-Object { "{0}={1}" -f $_.resourceType, $_.quantity }) -join ", "
    $stockSummary = ($diagnostics.orbitalStock | ForEach-Object { "{0}={1}" -f $_.assetType, $_.quantity }) -join ", "

    Write-Host ""
    Write-Host "Resumen"
    Write-Host "Planeta: $($diagnostics.planetName)"
    Write-Host "Recursos: $resourceSummary"
    Write-Host "Construccion abierta: $($diagnostics.construction.openCount)"
    Write-Host "Investigacion abierta: $($diagnostics.research.openCount)"
    Write-Host "Astillero abierto: $($diagnostics.shipyard.openCount)"
    Write-Host "Stock orbital: $stockSummary"

    if ($null -ne $diagnostics.readinessNotes -and @($diagnostics.readinessNotes).Count -gt 0) {
        Write-Host ""
        Write-Host "Notas de readiness:"
        foreach ($note in $diagnostics.readinessNotes) {
            Write-Host " - $note"
        }
    }

    if ($null -ne $diagnostics.warnings -and @($diagnostics.warnings).Count -gt 0) {
        Write-Host ""
        Write-Host "Advertencias:"
        foreach ($warning in $diagnostics.warnings) {
            Write-Host " - $warning"
        }
    }

    Write-Host ""
    Write-Host "Next suggested checks:"
    Write-Host "/planet?civilizationId=$CivilizationId&planetId=$PlanetId"
    Write-Host "/construction?civilizationId=$CivilizationId&planetId=$PlanetId"
    Write-Host "/research?civilizationId=$CivilizationId&planetId=$PlanetId"
    Write-Host "/shipyard?civilizationId=$CivilizationId&planetId=$PlanetId"

    if ($RawJson) {
        Write-Host ""
        Write-Host "Raw JSON:"
        $response | ConvertTo-Json -Depth 12
    }
}
catch {
    $httpResponse = $_.Exception.Response
    if ($null -eq $httpResponse) {
        throw "Backend no disponible en '$BaseUrl' o error de red. Verifica que `dotnet run --project .\src\VoidEmpires.Web` este activo. $($_.Exception.Message)"
    }

    $statusCode = [int]$httpResponse.StatusCode
    $reader = New-Object System.IO.StreamReader($httpResponse.GetResponseStream())
    try {
        if ($reader.BaseStream.CanSeek) {
            $reader.BaseStream.Position = 0
        }

        $reader.DiscardBufferedData()
        $body = $reader.ReadToEnd()
        throw "No se pudo invocar el endpoint de diagnosticos. HTTP $statusCode. Body: $body"
    }
    finally {
        $reader.Dispose()
    }
}
