param(
    [string]$BaseUrl = "http://localhost:5142",
    [Guid]$CivilizationId = "00000000-0000-0000-0000-000000000001",
    [Guid]$PlanetId = "40000000-0000-0000-0000-000000000001",
    [DateTime]$NowUtc,
    [int]$ElapsedSeconds = 0,
    [switch]$IncludeConstruction,
    [switch]$IncludeResearch,
    [switch]$IncludeShipyard
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

Write-Warning "Este script modifica la base de datos de Development materializando ordenes vencidas."

$anyIncludeSwitch = $IncludeConstruction -or $IncludeResearch -or $IncludeShipyard
if (-not $anyIncludeSwitch) {
    $IncludeConstruction = $true
    $IncludeResearch = $true
    $IncludeShipyard = $true
}

if ($ElapsedSeconds -lt 0) {
    throw "ElapsedSeconds cannot be negative."
}

$effectiveNowUtc = if ($PSBoundParameters.ContainsKey("NowUtc")) {
    if ($NowUtc.Kind -eq [DateTimeKind]::Unspecified) {
        [DateTime]::SpecifyKind($NowUtc, [DateTimeKind]::Utc)
    }
    else {
        $NowUtc.ToUniversalTime()
    }
}
else {
    [DateTime]::UtcNow.AddSeconds($ElapsedSeconds)
}

function Format-MaterializationSummary {
    param(
        [string]$Label,
        [object]$Summary
    )

    if ($null -eq $Summary) {
        return "${Label}: no solicitado"
    }

    return "{0}: procesadas={1}, vencidas={2}, no vencidas={3}" -f `
        $Label, `
        $Summary.processedCount, `
        $Summary.dueCount, `
        $Summary.skippedNotDueCount
}

try {
    $request = [ordered]@{
        civilizationId = $CivilizationId
        planetId = $PlanetId
        nowUtc = $effectiveNowUtc.ToString("O")
        includeConstruction = [bool]$IncludeConstruction
        includeResearch = [bool]$IncludeResearch
        includeShipyard = [bool]$IncludeShipyard
    }

    Write-Host "Invocando materializacion: Civilization=$CivilizationId Planet=$PlanetId NowUtc=$($request.nowUtc)"
    $response = Invoke-RestMethod `
        -Method Post `
        -Uri ($BaseUrl.TrimEnd("/") + "/api/dev/queues/materialize-due") `
        -ContentType "application/json" `
        -Body ($request | ConvertTo-Json -Depth 6 -Compress)

    if (-not $response.succeeded) {
        $notes = if ($null -ne $response.notes) { ($response.notes | ForEach-Object { "$_" }) -join "; " } else { "unknown error" }
        throw "No se pudo materializar la cola. Nota(s): $notes"
    }

    Write-Host ""
    Write-Host "Materializacion completada."
    Write-Host (Format-MaterializationSummary -Label "Construccion" -Summary $response.construction)
    Write-Host (Format-MaterializationSummary -Label "Investigacion" -Summary $response.research)
    Write-Host (Format-MaterializationSummary -Label "Astillero" -Summary $response.shipyard)

    if ($null -ne $response.notes -and @($response.notes).Count -gt 0) {
        Write-Host ""
        Write-Host "Notas:"
        foreach ($note in $response.notes) {
            Write-Host " - $note"
        }
    }
}
catch {
    $response = $_.Exception.Response
    if ($null -eq $response) {
        throw "Backend no disponible en '$BaseUrl' o error de red. Verifica que `dotnet run --project .\src\VoidEmpires.Web` este activo. $($_.Exception.Message)"
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
