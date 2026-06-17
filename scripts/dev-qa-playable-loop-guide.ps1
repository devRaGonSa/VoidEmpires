param(
    [string]$BaseUrl = "http://localhost:5142",
    [string]$FrontendUrl = "http://localhost:5173",
    [Guid]$CivilizationId,
    [Guid]$PlanetId,
    [switch]$RunDiagnostics
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
. (Join-Path $PSScriptRoot "dev-qa-common.ps1")

$backendCommand = Format-DevQaBackendRunCommand
$frontendCommand = "npm run dev --prefix src\VoidEmpires.Frontend"
$prepareCommand = Format-DevQaPowerShellCommand `
    -ScriptName "dev-qa-prepare-playable-session-state.ps1" `
    -Parameters ([ordered]@{
        BaseUrl = $BaseUrl.TrimEnd("/")
        ElapsedSeconds = 3600
        PrintQueueMaterializationCommand = $true
    })
$materializeCommand = Format-DevQaPowerShellCommand `
    -ScriptName "dev-qa-materialize-due-queues.ps1" `
    -Parameters ([ordered]@{
        BaseUrl = $BaseUrl.TrimEnd("/")
        CivilizationId = "<CivilizationId>"
        PlanetId = "<PlanetId>"
        ElapsedSeconds = 3600
    })
$diagnosticsCommand = Format-DevQaPowerShellCommand `
    -ScriptName "dev-qa-get-playable-session-diagnostics.ps1" `
    -Parameters ([ordered]@{
        BaseUrl = $BaseUrl.TrimEnd("/")
        CivilizationId = "<CivilizationId>"
        PlanetId = "<PlanetId>"
    })

Write-Host "Target"
Write-Host "BaseUrl: $BaseUrl"
Write-Host "FrontendUrl: $FrontendUrl"
Write-Host "Action: Print playable-loop Development QA guide"
Write-Host ""
Write-Host "Guia ordenada del loop jugable Development QA"
Write-Host ""
Write-Host "1. Arranca el backend:"
Write-Host "   $backendCommand"
Write-Host ""
Write-Host "2. Crea o prepara un inicio jugable Development:"
Write-Host "   $prepareCommand"
Write-Host "   Nota: este helper crea datos reales de Development y puede materializar produccion de recursos si pasas ElapsedSeconds."
Write-Host ""
Write-Host "3. Arranca el frontend:"
Write-Host "   $frontendCommand"
Write-Host ""
Write-Host "4. Abre el flujo de entrada o una cabina con los ids devueltos:"
Write-Host "   $($FrontendUrl.TrimEnd("/"))/onboarding"
Write-Host "   $($FrontendUrl.TrimEnd("/"))/planet?civilizationId=<CivilizationId>&planetId=<PlanetId>"
Write-Host "   $($FrontendUrl.TrimEnd("/"))/construction?civilizationId=<CivilizationId>&planetId=<PlanetId>"
Write-Host "   $($FrontendUrl.TrimEnd("/"))/research?civilizationId=<CivilizationId>&planetId=<PlanetId>"
Write-Host "   $($FrontendUrl.TrimEnd("/"))/shipyard?civilizationId=<CivilizationId>&planetId=<PlanetId>"
Write-Host ""
Write-Host "5. Encola acciones solo desde las cabinas protegidas y con confirmacion explicita."
Write-Host "   No uses esta guia como auto-enqueue: Planeta/Construccion, Investigacion y Astillero siguen siendo los unicos puntos de mutacion del loop."
Write-Host ""
Write-Host "6. Materializa colas vencidas solo cuando ya existan ordenes vencidas:"
Write-Host "   $materializeCommand"
Write-Host "   Nota: este paso muta la base Development. No se ejecuta automaticamente desde esta guia."
Write-Host ""
Write-Host "7. Lee diagnosticos despues de cada tramo relevante:"
Write-Host "   $diagnosticsCommand"
Write-Host ""
Write-Host "Opciones de esta guia:"
Write-Host " -RunDiagnostics ejecuta solo la lectura de diagnosticos y requiere -CivilizationId y -PlanetId."
Write-Host " Ninguna opcion de esta guia crea, encola ni materializa por si sola."

if (-not $RunDiagnostics) {
    return
}

if (-not $PSBoundParameters.ContainsKey("CivilizationId") -or -not $PSBoundParameters.ContainsKey("PlanetId")) {
    throw "RunDiagnostics requiere -CivilizationId y -PlanetId."
}

Write-Host ""
Write-Host "Ejecutando diagnosticos read-only solicitados explicitamente..."
& (Join-Path $PSScriptRoot "dev-qa-get-playable-session-diagnostics.ps1") `
    -BaseUrl $BaseUrl `
    -CivilizationId $CivilizationId `
    -PlanetId $PlanetId
