param()

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$frontendSrc = Join-Path $PSScriptRoot "..\src\VoidEmpires.Frontend\src"
$resolvedFrontendSrc = [System.IO.Path]::GetFullPath($frontendSrc)
$docsRoot = Join-Path $PSScriptRoot "..\docs"
$resolvedDocsRoot = [System.IO.Path]::GetFullPath($docsRoot)
$scriptsRoot = Join-Path $PSScriptRoot "..\scripts"
$resolvedScriptsRoot = [System.IO.Path]::GetFullPath($scriptsRoot)
$seedUiStateRoot = Join-Path $PSScriptRoot "..\src\VoidEmpires.Infrastructure"
$resolvedSeedUiStateRoot = [System.IO.Path]::GetFullPath($seedUiStateRoot)
$repoRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot ".."))

if (-not (Test-Path -LiteralPath $resolvedFrontendSrc)) {
  throw "Frontend source root was not found at '$resolvedFrontendSrc'."
}
if (-not (Test-Path -LiteralPath $resolvedDocsRoot)) {
  throw "Docs root was not found at '$resolvedDocsRoot'."
}
if (-not (Test-Path -LiteralPath $resolvedScriptsRoot)) {
  throw "Scripts root was not found at '$resolvedScriptsRoot'."
}
if (-not (Test-Path -LiteralPath $resolvedSeedUiStateRoot)) {
  throw "Infrastructure src root was not found at '$resolvedSeedUiStateRoot'."
}

$frontendFiles = Get-ChildItem -Path $resolvedFrontendSrc -Recurse -Include *.ts, *.tsx -File
$docsFiles = Get-ChildItem -Path $resolvedDocsRoot -Recurse -Include *.md -File
$scriptFiles = Get-ChildItem -Path $resolvedScriptsRoot -Recurse -Include *.ps1 -File |
  Where-Object { $_.FullName -ne $PSCommandPath }
$seedPayloadFiles = Get-ChildItem -Path $resolvedSeedUiStateRoot -Recurse -Include *UiStateService.cs, *UiState*Service.cs -File |
  Where-Object { $_.FullName -notmatch "\\bin\\" -and $_.FullName -notmatch "\\obj\\" }
$appsettingsFiles = Get-ChildItem -Path $repoRoot -Recurse -Include appsettings*.json -File |
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

$copyHygieneFailures = New-Object System.Collections.Generic.List[string]

$connectionStringFiles = $docsFiles + $scriptFiles + $appsettingsFiles
$passwordAssignmentPattern = '(?i)Password\s*=\s*("?)([^;"\s`]+)\1'
$allowedPasswordPlaceholders = @(
  '<PASSWORD>',
  '<local-password>',
  'YOUR_LOCAL_PASSWORD',
  'LOCAL_PASSWORD',
  '<PASSWORD_PLACEHOLDER>'
)
$passwordAssignmentMatches = Select-String -Path ($connectionStringFiles | Select-Object -ExpandProperty FullName) -Pattern $passwordAssignmentPattern -Encoding UTF8
foreach ($match in @($passwordAssignmentMatches)) {
  foreach ($capture in [regex]::Matches($match.Line, $passwordAssignmentPattern)) {
    $passwordValue = $capture.Groups[2].Value
    if ($allowedPasswordPlaceholders -contains $passwordValue) {
      continue
    }

    $copyHygieneFailures.Add(("{0}:{1}: unsafe connection-string password example detected; use a safe placeholder such as Password=<PASSWORD>: {2}" -f $match.Path, $match.LineNumber, $match.Line.Trim()))
  }
}

$mojibakePatterns = @(
  (-join @([char]0x00C3, [char]0x00A1)),
  (-join @([char]0x00C3, [char]0x00A9)),
  (-join @([char]0x00C3, [char]0x00AD)),
  (-join @([char]0x00C3, [char]0x00B3)),
  (-join @([char]0x00C3, [char]0x00BA)),
  (-join @([char]0x00C3, [char]0x00B1)),
  (-join @([char]0x00C2, [char]0x00BF)),
  (-join @([char]0x00C2, [char]0x00A1)),
  (-join @([char]0x00E2, [char]0x20AC)),
  (-join @([char]0x00C3, [char]0x0192, [char]0x00C6, [char]0x2019)),
  (-join @([char]0x00C3, [char]0x0192, [char]0x00E2, [char]0x20AC, [char]0x0161)),
  (-join @([char]0x00C3, [char]0x2020, [char]0x00E2, [char]0x20AC, [char]0x2122))
)
$mojibakeFiles = $frontendFiles + $docsFiles + $scriptFiles
$mojibakeMatches = Select-String -Path ($mojibakeFiles | Select-Object -ExpandProperty FullName) -Pattern $mojibakePatterns -SimpleMatch -CaseSensitive:$true -Encoding UTF8
foreach ($match in @($mojibakeMatches)) {
  $copyHygieneFailures.Add(("{0}:{1}: corrupted encoding sequence detected: {2}" -f $match.Path, $match.LineNumber, $match.Line.Trim()))
}

$placeholderIdPattern = "<(CivilizationId|PlanetId|SystemId|UserId|PlayerProfileId)>"
$placeholderSearchFiles = $frontendFiles + $docsFiles
$placeholderMatches = Select-String -Path ($placeholderSearchFiles | Select-Object -ExpandProperty FullName) -Pattern $placeholderIdPattern -CaseSensitive:$true
foreach ($match in @($placeholderMatches)) {
  $copyHygieneFailures.Add(("{0}:{1}: replace angle-bracket id placeholder with a real seeded id, local-session guidance, or braces-only docs notation: {2}" -f $match.Path, $match.LineNumber, $match.Line.Trim()))
}

$englishFallbackPatterns = @(
  '"Loading\.\.\."',
  '"No data"',
  '"Unavailable"',
  '"Unknown system"',
  '"Unknown planet"',
  ">Loading\.\.\.<",
  ">No data<",
  ">Unavailable<",
  ">Unknown system<",
  ">Unknown planet<"
)
$primaryUiFiles = $frontendFiles |
  Where-Object { $_.FullName -match "\\(pages|components)\\" }
$englishFallbackMatches = Select-String -Path ($primaryUiFiles | Select-Object -ExpandProperty FullName) -Pattern $englishFallbackPatterns -CaseSensitive:$true
foreach ($match in @($englishFallbackMatches)) {
  $copyHygieneFailures.Add(("{0}:{1}: English fallback copy in primary UI should be Spanish-first: {2}" -f $match.Path, $match.LineNumber, $match.Line.Trim()))
}

$obsoleteVisibleCopyPatterns = @(
  "Prototipo jugable local",
  "Estado global del prototipo",
  "alineacion del prototipo",
  "Nivel de prototipo",
  "Acciones de mutacion de prototipo",
  "mutacion de prototipo",
  "Partida local",
  "partida local",
  "sesion local",
  "Nueva partida",
  "Continuar partida",
  "Olvidar partida",
  "Crear partida",
  "partida guardada",
  "Sesion local",
  "sesión local",
  "Sin sesion local",
  "local game",
  "local session",
  "new local game",
  "new game"
)
$obsoleteVisibleMatches = Select-String -Path ($primaryUiFiles | Select-Object -ExpandProperty FullName) -Pattern $obsoleteVisibleCopyPatterns -SimpleMatch -CaseSensitive:$false
foreach ($match in @($obsoleteVisibleMatches)) {
  $copyHygieneFailures.Add(("{0}:{1}: obsolete local-game wording in primary UI should use account/world-entry language: {2}" -f $match.Path, $match.LineNumber, $match.Line.Trim()))
}

$placeholderCopyPatterns = @(
  "placeholder vacio",
  "placeholder vacío",
  "placeholders futuros",
  "placeholders diplomaticos",
  "placeholders diplomáticos",
  "rellena con placeholders",
  "relleno temporal",
  "lorem ipsum",
  "coming soon",
  "under construction",
  "sample data",
  "mock data",
  "dummy data",
  "datos de ejemplo",
  "datos mock"
)
$placeholderCopyMatches = Select-String -Path ($primaryUiFiles | Select-Object -ExpandProperty FullName) -Pattern $placeholderCopyPatterns -SimpleMatch -CaseSensitive:$false
foreach ($match in @($placeholderCopyMatches)) {
  $copyHygieneFailures.Add(("{0}:{1}: placeholder or mock-copy wording must be replaced with explicit current/future/deferred scope: {2}" -f $match.Path, $match.LineNumber, $match.Line.Trim()))
}

$fakeResourceCopyPatterns = @(
  "fake resources",
  "fake stock",
  "mock resources",
  "dummy resources",
  "recursos falsos",
  "recurso falso",
  "reservas falsas",
  "reservas inventadas",
  "stock falso",
  "stock inventado",
  "cola falsa",
  "cola inventada"
)
$fakeResourceCopyMatches = Select-String -Path ($primaryUiFiles | Select-Object -ExpandProperty FullName) -Pattern $fakeResourceCopyPatterns -SimpleMatch -CaseSensitive:$false
foreach ($match in @($fakeResourceCopyMatches)) {
  $copyHygieneFailures.Add(("{0}:{1}: fake resource or queue copy is forbidden in primary UI: {2}" -f $match.Path, $match.LineNumber, $match.Line.Trim()))
}

$productionAuthOverclaimPatterns = @(
  "login de produccion habilitado",
  "autenticacion de produccion habilitada",
  "auth de produccion habilitada",
  "sesion autenticada activa",
  "permisos de produccion activos",
  "production auth ready",
  "production login ready"
)
$productionAuthOverclaimMatches = Select-String -Path ($primaryUiFiles | Select-Object -ExpandProperty FullName) -Pattern $productionAuthOverclaimPatterns -SimpleMatch -CaseSensitive:$false
foreach ($match in @($productionAuthOverclaimMatches)) {
  $copyHygieneFailures.Add(("{0}:{1}: production-auth overclaim detected in Development UI copy: {2}" -f $match.Path, $match.LineNumber, $match.Line.Trim()))
}

$globalShellFiles = $frontendFiles |
  Where-Object {
    $_.FullName -match "\\src\\VoidEmpires.Frontend\\src\\App\.tsx$" -or
    $_.FullName -match "\\src\\VoidEmpires.Frontend\\src\\components\\ui\\(AppShell|TopResourceBar)\.tsx$"
  }
$globalShellForbiddenPatterns = @(
  "Modo Development",
  "Suite jugable local",
  "VoidEmpires local",
  "Bucle jugable Development",
  "lecturas backend",
  "mutaciones Development",
  "login de produccion",
  "Sin recursos globales simulados",
  "URL base del backend",
  "Perfil esperado del backend",
  "Endpoints Development"
)
$globalShellForbiddenMatches = Select-String -Path ($globalShellFiles | Select-Object -ExpandProperty FullName) -Pattern $globalShellForbiddenPatterns -SimpleMatch -CaseSensitive:$false
foreach ($match in @($globalShellForbiddenMatches)) {
  $copyHygieneFailures.Add(("{0}:{1}: global shell must stay product-facing and hide development/backend copy: {2}" -f $match.Path, $match.LineNumber, $match.Line.Trim()))
}

$visibleEndpointDetailPatterns = @(
  "http://localhost",
  "https://localhost",
  "URL base del backend",
  "Perfil esperado del backend",
  "Endpoints Development",
  "/api/dev/",
  "POST /api/",
  "GET /api/",
  "payload bruto del endpoint"
)
$visibleEndpointDetailMatches = Select-String -Path ($primaryUiFiles | Select-Object -ExpandProperty FullName) -Pattern $visibleEndpointDetailPatterns -SimpleMatch -CaseSensitive:$false
foreach ($match in @($visibleEndpointDetailMatches)) {
  $copyHygieneFailures.Add(("{0}:{1}: endpoint URLs, localhost, backend profile, and raw endpoint details must not appear in normal UI: {2}" -f $match.Path, $match.LineNumber, $match.Line.Trim()))
}

$operatorOnlyComponentPattern = "\\src\\VoidEmpires.Frontend\\src\\components\\(ActionManifestPanel|DevDiagnosticsPanel|DevEndpointNotice|DevelopmentToolsPanel)\.tsx$"
$productSurfaceFiles = $primaryUiFiles |
  Where-Object { $_.FullName -notmatch $operatorOnlyComponentPattern }
$productSurfaceForbiddenPatterns = @(
  "(?i)\bDevelopment\b",
  "(?i)\bDev\b",
  "(?i)\bQA\b",
  "(?i)\bTest\b",
  "(?i)\bPrueba\b",
  "(?i)\bPrototipo\b",
  "(?i)Solo desarrollo",
  "(?i)Development-safe",
  "(?i)\bseed\b",
  "(?i)cockpit-validation",
  "(?i)minimal-validation",
  "(?i)\bendpoint(s)?\b",
  "(?i)localhost"
)

$productSurfaceAllowedLinePatterns = @(
  "^\s*import\b",
  "^\s*(export\s+)?(interface|type)\b",
  "^\s*(const|function)\s+.*(Development|Dev|QA|Test|endpoint|localhost)",
  "actionScope",
  "aria-",
  "className",
  "data-",
  "dev-diagnostics",
  "dev-meta",
  "developmentNote=",
  "DevelopmentToolsPanel",
  "DevDiagnosticsPanel",
  "DevEndpointNotice",
  "GameModalScope",
  "id=",
  "isOperatorMode",
  "operatorMode",
  "setTechnicalErrorDetail",
  "technicalDetail",
  "technical-disclosure"
)
$operatorOnlyVisibleFragments = @(
  "Materializaciones Development",
  "Las acciones QA mutan la base de datos Development",
  "Development QA",
  "Esta accion es solo Development",
  "Confirmar accion Development",
  "Esta accion muta la base de datos de Development."
)
$nonRenderedCockpitHeroFragments = @(
  "Mutaciones Development confirmadas"
)
$forbiddenLegacyModuleTermPattern = "(?i)\bca" + "bina(s)?\b"
$productSurfaceForbiddenPatterns = @($forbiddenLegacyModuleTermPattern) + $productSurfaceForbiddenPatterns

$productSurfaceForbiddenMatches = Select-String -Path ($productSurfaceFiles | Select-Object -ExpandProperty FullName) -Pattern $productSurfaceForbiddenPatterns -Encoding UTF8
foreach ($match in @($productSurfaceForbiddenMatches)) {
  $line = $match.Line.Trim()

  $isAllowed = $false
  foreach ($allowedPattern in $productSurfaceAllowedLinePatterns) {
    if ($line -match $allowedPattern) {
      $isAllowed = $true
      break
    }
  }

  if (-not $isAllowed) {
    foreach ($fragment in $operatorOnlyVisibleFragments) {
      if ($line -like "*$fragment*") {
        $isAllowed = $true
        break
      }
    }
  }

  if (-not $isAllowed) {
    foreach ($fragment in $nonRenderedCockpitHeroFragments) {
      if ($line -like "*$fragment*") {
        $isAllowed = $true
        break
      }
    }
  }

  if (-not $isAllowed) {
    $copyHygieneFailures.Add(("{0}:{1}: forbidden dev/test/prototype wording in product-surface UI; keep this copy operator-only or product-facing: {2}" -f $match.Path, $match.LineNumber, $line))
  }
}

if ($copyHygieneFailures.Count -gt 0) {
  throw "Frontend copy hygiene guard failed:`n$($copyHygieneFailures -join [Environment]::NewLine)"
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
  "auto complete",
  "Completar instantaneamente",
  "Completar instantáneamente",
  "Completa instantaneamente",
  "Completa instantáneamente",
  "Autocompletar al cargar",
  "Auto completar al cargar",
  "Auto-complete on page load",
  "Completar al cargar",
  "Materializar al cargar",
  "Materializa automaticamente al abrir",
  "Materializa automáticamente al abrir",
  "Materializacion como gameplay normal",
  "Materialización como gameplay normal"
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

$forbiddenCheatPatterns = @(
  "Instant completion",
  "Instant complete",
  "Complete instantly",
  "Cheat",
  "Cheats",
  "Truco",
  "Trucos",
  "Modo trampa",
  "Completar ahora sin esperar",
  "Completar sin esperar"
)
$forbiddenCheatMatches = Select-String -Path $frontendActionSurfacePaths -Pattern $forbiddenCheatPatterns -SimpleMatch -CaseSensitive:$false
if ($forbiddenCheatMatches) {
  Write-Host "Forbidden cheat or instant-completion copy detected in frontend page or component files:" -ForegroundColor Red
  $forbiddenCheatMatches | ForEach-Object {
    Write-Host ("{0}:{1}: {2}" -f $_.Path, $_.LineNumber, $_.Line.Trim())
  }

  throw "Frontend cheat-copy guard failed."
}

$playableSessionPath = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot "..\src\VoidEmpires.Frontend\src\utils\playableSession.ts"))
if (-not (Test-Path -LiteralPath $playableSessionPath)) {
  throw "Playable session helper was not found at '$playableSessionPath'."
}

$playableSessionViolations = New-Object System.Collections.Generic.List[string]
$credentialStorageViolations = New-Object System.Collections.Generic.List[string]
$credentialStoragePattern = '(?is)(localStorage|sessionStorage)\s*\.\s*(setItem|getItem|removeItem)\s*\([^)]*(password|token|credential|auth|cookie|bearer)'
foreach ($file in $frontendFiles) {
  $content = Get-Content -LiteralPath $file.FullName -Raw
  if ($content -match $credentialStoragePattern) {
    $credentialStorageViolations.Add("$($file.FullName): browser storage must not read or write password, token, auth, cookie, bearer, or credential fields.")
  }
}

if ($credentialStorageViolations.Count -gt 0) {
  throw "Frontend credential storage guard failed:`n$($credentialStorageViolations -join [Environment]::NewLine)"
}

$browserStorageMatches = Select-String -Path (
  $frontendFiles |
    Where-Object { $_.FullName -ne $playableSessionPath } |
    Select-Object -ExpandProperty FullName
) -Pattern @("localStorage", "sessionStorage") -SimpleMatch -CaseSensitive:$true
foreach ($match in @($browserStorageMatches)) {
  $playableSessionViolations.Add(("{0}:{1}: browser storage access must stay centralized in playableSession.ts: {2}" -f $match.Path, $match.LineNumber, $match.Line.Trim()))
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
      "La vista la mantiene visible sin completarla automaticamente.",
      "El stock orbital no equivale automaticamente a una escuadra visible en Flotas."
    )
  },
  @{
    Path = "src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx"
    Fragments = @(
      "Sin combate ni intercepcion",
      "Una orden visible confirma preparacion de construccion, no combate ni cierre automatico."
    )
  },
  @{
    Path = "src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx"
    Fragments = @(
      "La estimacion revisa una ruta sin reservar escuadras ni gastar recursos.",
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

$materializationGuardPath = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot "..\src\VoidEmpires.Frontend\src\pages\PlanetPage.tsx"))
if (-not (Test-Path -LiteralPath $materializationGuardPath)) {
  throw "Planet materialization guard file was not found at '$materializationGuardPath'."
}

$materializationContent = Get-Content -LiteralPath $materializationGuardPath -Raw
$requiredMaterializationFragments = @(
  "{operatorMode ? (",
  "Development QA",
  "Materializa solo ordenes vencidas en backend",
  "Las ordenes no vencidas se mantienen abiertas.",
  "onClick={() => void handleQueueMaterializationRefresh()}"
)
$missingMaterializationFragments = New-Object System.Collections.Generic.List[string]
foreach ($fragment in $requiredMaterializationFragments) {
  if ($materializationContent -notlike "*$fragment*") {
    $missingMaterializationFragments.Add("PlanetPage.tsx is missing materialization guard fragment: $fragment")
  }
}

if ($missingMaterializationFragments.Count -gt 0) {
  throw "Frontend materialization copy guard failed:`n$($missingMaterializationFragments -join [Environment]::NewLine)"
}

$operatorMaterializationPattern = '(?s)\{operatorMode \? \(\s*<div className=\{isConstructionRoute \? "construction-devtools-secondary" : undefined\}>\s*<DevelopmentToolsPanel'
if ($materializationContent -notmatch $operatorMaterializationPattern) {
  throw "Frontend materialization operator guard failed: DevelopmentToolsPanel must stay behind operatorMode."
}

$forbiddenProductMaterializationCopy = @(
  @{
    Path = "src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx"
    Fragments = @(
      "Usa la accion Development QA de Planeta",
      "materializar ordenes vencidas",
      "Tras materializar desde Planeta"
    )
  }
)

$visibleMaterializationLeaks = New-Object System.Collections.Generic.List[string]
foreach ($requirement in $forbiddenProductMaterializationCopy) {
  $path = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot "..\$($requirement.Path)"))
  if (-not (Test-Path -LiteralPath $path)) {
    $visibleMaterializationLeaks.Add("Missing product materialization guard file '$($requirement.Path)'.")
    continue
  }

  $content = Get-Content -LiteralPath $path -Raw
  foreach ($fragment in $requirement.Fragments) {
    if ($content -like "*$fragment*") {
      $visibleMaterializationLeaks.Add("$($requirement.Path) contains forbidden product materialization copy: $fragment")
    }
  }
}

if ($visibleMaterializationLeaks.Count -gt 0) {
  throw "Frontend visible materialization guard failed:`n$($visibleMaterializationLeaks -join [Environment]::NewLine)"
}

Write-Host "Frontend copy regression check passed." -ForegroundColor Green
