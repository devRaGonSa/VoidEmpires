Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$appPath = Join-Path $PSScriptRoot "..\src\VoidEmpires.Frontend\src\App.tsx"
$resolvedAppPath = [System.IO.Path]::GetFullPath($appPath)

if (-not (Test-Path -LiteralPath $resolvedAppPath)) {
    throw "App route file was not found at '$resolvedAppPath'."
}

$appContent = Get-Content -LiteralPath $resolvedAppPath -Raw

$protectedPages = @(
    "StrategicMapPage",
    "PlanetPage",
    "ConstructionPage",
    "ResearchPage",
    "ShipyardPage",
    "FleetsPage",
    "DefensesPage",
    "GroundArmyPage",
    "EspionagePage",
    "AlliancePage",
    "MarketPage",
    "RankingPage",
    "ModuleCabinPage"
)

$staticImportViolations = New-Object System.Collections.Generic.List[string]

foreach ($pageName in $protectedPages) {
    $pattern = "(?m)^\s*import\s+.+\b$pageName\b.+from\s+['""]\./pages/$pageName['""]\s*;?\s*$"
    if ($appContent -match $pattern) {
        $staticImportViolations.Add($pageName)
    }
}

if ($staticImportViolations.Count -gt 0) {
    throw "Protected cockpit pages must stay lazy-loaded in App.tsx. Direct imports found for: $($staticImportViolations -join ', ')."
}

$requiredLazyPages = @(
    "StrategicMapPage",
    "PlanetPage",
    "ConstructionPage",
    "ResearchPage",
    "ShipyardPage",
    "FleetsPage",
    "DefensesPage",
    "GroundArmyPage",
    "EspionagePage",
    "AlliancePage",
    "MarketPage",
    "RankingPage",
    "ModuleCabinPage"
)

$missingLazyDefinitions = New-Object System.Collections.Generic.List[string]

foreach ($pageName in $requiredLazyPages) {
    $lazyPattern = "(?s)\bconst\s+$pageName\s*=\s*lazy\s*\(\s*async\s*\(\)\s*=>\s*\{\s*const\s+module\s*=\s*await\s+import\(['""]\./pages/$pageName['""]\)"
    if ($appContent -notmatch $lazyPattern) {
        $missingLazyDefinitions.Add($pageName)
    }
}

if ($missingLazyDefinitions.Count -gt 0) {
    throw "Expected lazy route definitions were not found in App.tsx for: $($missingLazyDefinitions -join ', ')."
}

Write-Host "Frontend route lazy-import guard passed."
