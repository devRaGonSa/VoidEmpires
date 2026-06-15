param()

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$frontendSrc = Join-Path $PSScriptRoot "..\src\VoidEmpires.Frontend\src"
$resolvedFrontendSrc = [System.IO.Path]::GetFullPath($frontendSrc)
$docsRoot = Join-Path $PSScriptRoot "..\docs"
$resolvedDocsRoot = [System.IO.Path]::GetFullPath($docsRoot)
$seedUiStateRoot = Join-Path $PSScriptRoot "..\src\VoidEmpires.Infrastructure"
$resolvedSeedUiStateRoot = [System.IO.Path]::GetFullPath($seedUiStateRoot)

if (-not (Test-Path -LiteralPath $resolvedFrontendSrc)) {
  throw "Frontend source root was not found at '$resolvedFrontendSrc'."
}
if (-not (Test-Path -LiteralPath $resolvedDocsRoot)) {
  throw "Docs root was not found at '$resolvedDocsRoot'."
}
if (-not (Test-Path -LiteralPath $resolvedSeedUiStateRoot)) {
  throw "Infrastructure src root was not found at '$resolvedSeedUiStateRoot'."
}

$frontendFiles = Get-ChildItem -Path $resolvedFrontendSrc -Recurse -Include *.ts, *.tsx -File
$docsFiles = Get-ChildItem -Path $resolvedDocsRoot -Recurse -Include *.md -File
$seedPayloadFiles = Get-ChildItem -Path $resolvedSeedUiStateRoot -Recurse -Include *UiStateService.cs, *UiState*Service.cs -File |
  Where-Object { $_.FullName -notmatch "\\bin\\" -and $_.FullName -notmatch "\\obj\\" }

$allFiles = @()
$allFiles += $frontendFiles
$allFiles += $docsFiles
$allFiles += $seedPayloadFiles

if ($allFiles.Count -eq 0) {
  Write-Host "No source, docs, or seed payload files found." -ForegroundColor Yellow
  exit 0
}

$frontendPatterns = @(
  "Recurso no clasificado",
  "Void Seed Civilization | current"
)

$allianceReadOnlyPatterns = @(
  "Alliance cockpit remains read-only in this phase"
)

$frontendAndSeedFiles = $frontendFiles + $seedPayloadFiles
$frontendAndSeedPaths = $frontendAndSeedFiles | Select-Object -ExpandProperty FullName
$frontendMatches = Select-String -Path $frontendAndSeedPaths -Pattern $frontendPatterns -SimpleMatch -CaseSensitive:$true
$deltaMatches = Select-String -Path $frontendAndSeedPaths -Pattern "\bDelta\b" -CaseSensitive:$true
$allianceReadOnlyMatches = Select-String -Path ($docsFiles + $frontendAndSeedFiles | Select-Object -ExpandProperty FullName) -Pattern $allianceReadOnlyPatterns -SimpleMatch -CaseSensitive:$true

# Exclude generated output helpers and snapshot artifacts.
 $allMatches = @()
if ($frontendMatches) {
  $allMatches += $frontendMatches
}
if ($deltaMatches) {
  $allMatches += $deltaMatches
}
if ($allianceReadOnlyMatches) {
  $allMatches += $allianceReadOnlyMatches
}

$filteredMatches = $allMatches | Where-Object {
  if (-not $_) {
    return $false
  }

  if ($_.Line -match "Delta de recursos") {
    return $false
  }

  $_.Path -notmatch "generated"
}

if ($filteredMatches) {
  Write-Host "Frontend copy regression detected in source, docs, or seed payload files:" -ForegroundColor Red
  $filteredMatches | ForEach-Object {
    Write-Host ("{0}:{1}: {2}" -f $_.Path, $_.LineNumber, $_.Line.Trim())
  }

  throw "Frontend copy regression check failed."
}

$forbiddenActionPatterns = @(
  "Attack",
  "Atacar",
  "Move fleet",
  "Mover flota",
  "Create mission",
  "Crear mision",
  "Crear misión",
  "Auto-complete",
  "Autocomplete",
  "autoComplete",
  "auto complete"
)

$frontendActionSurfacePaths = $frontendFiles |
  Where-Object { $_.FullName -match "\\(pages|components)\\" } |
  Select-Object -ExpandProperty FullName

$forbiddenActionMatches = Select-String -Path $frontendActionSurfacePaths -Pattern $forbiddenActionPatterns -SimpleMatch -CaseSensitive:$false
if ($forbiddenActionMatches) {
  Write-Host "Forbidden active-action copy detected in frontend page or component files:" -ForegroundColor Red
  $forbiddenActionMatches | ForEach-Object {
    Write-Host ("{0}:{1}: {2}" -f $_.Path, $_.LineNumber, $_.Line.Trim())
  }

  throw "Frontend forbidden-action guard failed."
}

$playableSessionPath = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot "..\src\VoidEmpires.Frontend\src\utils\playableSession.ts"))
if (-not (Test-Path -LiteralPath $playableSessionPath)) {
  throw "Playable session helper was not found at '$playableSessionPath'."
}

$playableSessionContent = Get-Content -LiteralPath $playableSessionPath -Raw
$requiredPlayableSessionFragments = @(
  "civilizationId: string;",
  "planetId: string;",
  "playerDisplayName?: string;",
  "civilizationName?: string;",
  "planetName?: string;",
  "createdAt: string;",
  "updatedAt: string;"
)
$forbiddenPlayableSessionFragments = @(
  "token",
  "credential",
  "password",
  "cookie",
  "bearer",
  "email",
  "role",
  "claim",
  "auth"
)

$playableSessionViolations = New-Object System.Collections.Generic.List[string]
foreach ($fragment in $requiredPlayableSessionFragments) {
  if ($playableSessionContent -notlike "*$fragment*") {
    $playableSessionViolations.Add("playableSession.ts is missing allowed navigation field fragment: $fragment")
  }
}

foreach ($fragment in $forbiddenPlayableSessionFragments) {
  if ($playableSessionContent -match "(?i)$([regex]::Escape($fragment))") {
    $playableSessionViolations.Add("playableSession.ts must not store credential-like field or wording: $fragment")
  }
}

if ($playableSessionViolations.Count -gt 0) {
  throw "Playable session storage guard failed:`n$($playableSessionViolations -join [Environment]::NewLine)"
}

$requiredSafetyCopy = @(
  @{
    Path = "src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx"
    Fragments = @(
      "Esta vista se centra en administracion colonial, no en combate ni maniobras espaciales.",
      "no promueve stock orbital automaticamente"
    )
  },
  @{
    Path = "src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx"
    Fragments = @(
      "La cabina la mantiene visible sin completarla automaticamente.",
      "El stock orbital no equivale automaticamente a una escuadra visible en Flotas."
    )
  },
  @{
    Path = "src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx"
    Fragments = @(
      "Sin combate ni intercepcion",
      "Una orden visible confirma readiness de construccion, no combate ni cierre automatico."
    )
  },
  @{
    Path = "src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx"
    Fragments = @(
      "La estimacion sigue en solo lectura y nunca reserva escuadras ni gasta recursos.",
      "Flotas no promueve automaticamente stock orbital a grupo operativo."
    )
  }
)

$requiredGameplayModalGuards = @(
  @{
    Path = "src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx"
    Fragments = @(
      "GameModal",
      "Confirmo que quiero enviar esta orden de construccion"
    )
  },
  @{
    Path = "src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx"
    Fragments = @(
      "GameModal",
      "Confirmo que quiero iniciar esta investigacion"
    )
  },
  @{
    Path = "src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx"
    Fragments = @(
      "GameModal",
      "Confirmo que quiero enviar esta produccion orbital a la cola"
    )
  }
)

$missingSafetyCopy = New-Object System.Collections.Generic.List[string]

foreach ($requirement in $requiredSafetyCopy) {
  $path = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot "..\$($requirement.Path)"))
  if (-not (Test-Path -LiteralPath $path)) {
    $missingSafetyCopy.Add("Missing protected cockpit file '$($requirement.Path)'.")
    continue
  }

  $content = Get-Content -LiteralPath $path -Raw
  foreach ($fragment in $requirement.Fragments) {
    if ($content -notlike "*$fragment*") {
      $missingSafetyCopy.Add("$($requirement.Path) is missing safety copy fragment: $fragment")
    }
  }
}

if ($missingSafetyCopy.Count -gt 0) {
  throw "Frontend safety copy guard failed:`n$($missingSafetyCopy -join [Environment]::NewLine)"
}

$missingGameplayModalGuards = New-Object System.Collections.Generic.List[string]

foreach ($requirement in $requiredGameplayModalGuards) {
  $path = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot "..\$($requirement.Path)"))
  if (-not (Test-Path -LiteralPath $path)) {
    $missingGameplayModalGuards.Add("Missing gameplay modal file '$($requirement.Path)'.")
    continue
  }

  $content = Get-Content -LiteralPath $path -Raw
  foreach ($fragment in $requirement.Fragments) {
    if ($content -notlike "*$fragment*") {
      $missingGameplayModalGuards.Add("$($requirement.Path) is missing gameplay modal guard fragment: $fragment")
    }
  }
}

if ($missingGameplayModalGuards.Count -gt 0) {
  throw "Frontend gameplay modal guard failed:`n$($missingGameplayModalGuards -join [Environment]::NewLine)"
}

Write-Host "Frontend copy regression check passed." -ForegroundColor Green
