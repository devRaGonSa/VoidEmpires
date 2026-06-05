param()

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$frontendSrc = Join-Path $PSScriptRoot "..\src\VoidEmpires.Frontend\src"
$resolvedFrontendSrc = [System.IO.Path]::GetFullPath($frontendSrc)

if (-not (Test-Path -LiteralPath $resolvedFrontendSrc)) {
  throw "Frontend source root was not found at '$resolvedFrontendSrc'."
}

$frontendFiles = Get-ChildItem -Path $resolvedFrontendSrc -Recurse -Include *.ts, *.tsx -File
if ($frontendFiles.Count -eq 0) {
  Write-Host "No frontend source files found under '$resolvedFrontendSrc'." -ForegroundColor Yellow
  exit 0
}

$patterns = @(
  "Recurso no clasificado",
  "Alliance cockpit remains read-only in this phase",
  "Void Seed Civilization | current",
  "read-only in this phase"
)

$matches = Select-String -Path ($frontendFiles | Select-Object -ExpandProperty FullName) -Pattern $patterns -SimpleMatch -CaseSensitive:$true
$deltaMatches = Select-String -Path ($frontendFiles | Select-Object -ExpandProperty FullName) -Pattern "\bDelta\b" -CaseSensitive:$true

# Exclude regex matches from generated output helpers and docs-adjacent generated snapshots.
$allMatches = @()
if ($matches) {
  $allMatches += $matches
}
if ($deltaMatches) {
  $allMatches += $deltaMatches
}

$filteredMatches = $allMatches | Where-Object {
  if (-not $_) {
    return $false
  }

  $_.Path -notmatch "generated" -and
  $_.Path -notmatch "\\docs\\"
}

if ($filteredMatches) {
  Write-Host "Frontend copy regression detected in source files:" -ForegroundColor Red
  $filteredMatches | ForEach-Object {
    Write-Host ("{0}:{1}: {2}" -f $_.Path, $_.LineNumber, $_.Line.Trim())
  }

  throw "Frontend copy regression check failed."
}

Write-Host "Frontend copy regression check passed." -ForegroundColor Green
