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

Write-Host "Frontend copy regression check passed." -ForegroundColor Green
