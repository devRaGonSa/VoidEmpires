param()

$patterns = @(
  "passive signal rows",
  "sensor profile rows",
  "detection coverage rows",
  "visible transfer trajectories",
  "gameplay is not implemented",
  "future placeholder",
  "is not executable from this cockpit",
  "Reconnaissance remains",
  "Infiltration gameplay",
  "Sabotage gameplay"
)

$paths = @(
  "src/VoidEmpires.Frontend/src/pages/*.tsx",
  "src/VoidEmpires.Frontend/src/utils/*.ts"
)

$matches = Select-String -Path $paths -Pattern $patterns -CaseSensitive:$false |
  Where-Object {
    $_.Line -notmatch "\.match\(" -and
    $_.Line -notmatch "\.test\("
  }

if ($matches) {
  Write-Host "Espionage copy regression detected:" -ForegroundColor Red
  $matches | ForEach-Object {
    Write-Host ("{0}:{1}: {2}" -f $_.Path, $_.LineNumber, $_.Line.Trim())
  }

  exit 1
}

Write-Host "Espionage copy check passed." -ForegroundColor Green
