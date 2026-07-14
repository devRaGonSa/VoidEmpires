Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$appPath = Join-Path $PSScriptRoot "..\src\VoidEmpires.Frontend\src\App.tsx"
$resolvedAppPath = [System.IO.Path]::GetFullPath($appPath)
$publicAuthLayoutPath = Join-Path $PSScriptRoot "..\src\VoidEmpires.Frontend\src\components\PublicAuthLayout.tsx"
$resolvedPublicAuthLayoutPath = [System.IO.Path]::GetFullPath($publicAuthLayoutPath)
$appShellPath = Join-Path $PSScriptRoot "..\src\VoidEmpires.Frontend\src\components\ui\AppShell.tsx"
$resolvedAppShellPath = [System.IO.Path]::GetFullPath($appShellPath)

if (-not (Test-Path -LiteralPath $resolvedAppPath)) {
    throw "App route file was not found at '$resolvedAppPath'."
}

if (-not (Test-Path -LiteralPath $resolvedPublicAuthLayoutPath)) {
    throw "Public auth layout file was not found at '$resolvedPublicAuthLayoutPath'."
}

if (-not (Test-Path -LiteralPath $resolvedAppShellPath)) {
    throw "Game shell file was not found at '$resolvedAppShellPath'."
}

$appContent = Get-Content -LiteralPath $resolvedAppPath -Raw
$publicAuthLayoutContent = Get-Content -LiteralPath $resolvedPublicAuthLayoutPath -Raw
$appShellContent = Get-Content -LiteralPath $resolvedAppShellPath -Raw

$protectedPages = @(
    "StrategicMapPage",
    "HomePage",
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
    "RegisterPage",
    "OnboardingPage",
    "LoginPage",
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
    "HomePage",
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
    "RegisterPage",
    "OnboardingPage",
    "LoginPage",
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

$authShellViolations = New-Object System.Collections.Generic.List[string]
$requiredPublicAuthPathFragments = @(
    '"/login"',
    '"/register"',
    '"/registro"',
    '"/onboarding"'
)

foreach ($pathFragment in $requiredPublicAuthPathFragments) {
    if ($appContent -notmatch [Regex]::Escape($pathFragment)) {
        $authShellViolations.Add("App.tsx must keep public auth route $pathFragment registered outside the game shell.")
    }
}

if ($appContent -notmatch 'const\s+publicAuthPathnames\s*=\s*new\s+Set') {
    $authShellViolations.Add("App.tsx must keep an explicit publicAuthPathnames set for standalone auth pages.")
}

if ($appContent -notmatch 'function\s+GameLayout\s*\(') {
    $authShellViolations.Add("App.tsx must keep an explicit GameLayout wrapper for authenticated game pages.")
}

if ($appContent -notmatch '<AppShell\s+sidebarItems=\{sidebarItems\}\s+statusItems=\{statusItems\}') {
    $authShellViolations.Add("GameLayout must render AppShell with sidebarItems and statusItems.")
}

if ($appContent -notmatch 'if\s*\(\s*isPublicAuthRoute\s*\|\|\s*shouldShowPublicAccountPrompt\s*\)\s*\{\s*return\s+<PublicAuthLayout>\{routeContent\}</PublicAuthLayout>;\s*\}') {
    $authShellViolations.Add("Public auth routes and signed-out account prompts must render through PublicAuthLayout.")
}

if ($appContent -notmatch 'return\s*\(\s*<GameLayout\s+sidebarItems=\{sidebarItems\}\s+statusItems=\{shellStatusItems\}') {
    $authShellViolations.Add("Authenticated game routes must render routeContent through GameLayout with sidebar and status items.")
}

if ($publicAuthLayoutContent -match '\b(AppShell|SidebarNav|TopStatusBar)\b|app-sidebar|top-status-bar|data-layout="game"') {
    $authShellViolations.Add("PublicAuthLayout.tsx must stay standalone and must not import or render game shell/sidebar/resource bar elements.")
}

if ($appShellContent -notmatch 'data-layout="game"' -or $appShellContent -notmatch '<aside\s+className="app-sidebar"' -or $appShellContent -notmatch '<TopStatusBar') {
    $authShellViolations.Add("AppShell.tsx must keep the game layout marker, sidebar, and top resource/status bar.")
}

if ($authShellViolations.Count -gt 0) {
    throw "Frontend auth shell guard failed:`n$($authShellViolations -join [Environment]::NewLine)"
}

$routeGuardPages = @(
    @{
        Path = "src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx"
        RequiredHelpers = @("buildPlanetUrl", "buildConstructionUrl", "buildGalaxyUrl", "buildFleetsUrl")
    },
    @{
        Path = "src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx"
        RequiredHelpers = @("buildShipyardUrl")
    },
    @{
        Path = "src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx"
        RequiredHelpers = @("buildPlanetUrl", "buildResearchUrl")
    },
    @{
        Path = "src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx"
        RequiredHelpers = @("buildDefensesUrl")
    },
    @{
        Path = "src/VoidEmpires.Frontend/src/pages/GroundArmyPage.tsx"
        RequiredHelpers = @()
    },
    @{
        Path = "src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx"
        RequiredHelpers = @()
    },
    @{
        Path = "src/VoidEmpires.Frontend/src/pages/MarketPage.tsx"
        RequiredHelpers = @("buildPlanetUrl", "buildConstructionUrl", "buildShipyardUrl", "buildGalaxyUrl", "buildFleetsUrl")
    },
    @{
        Path = "src/VoidEmpires.Frontend/src/pages/EspionagePage.tsx"
        RequiredHelpers = @("buildPlanetUrl", "buildResearchUrl", "buildGalaxyUrl", "buildFleetsUrl")
    },
    @{
        Path = "src/VoidEmpires.Frontend/src/pages/AlliancePage.tsx"
        RequiredHelpers = @("buildMarketUrl", "buildEspionageUrl", "buildGalaxyUrl", "buildRankingUrl")
    },
    @{
        Path = "src/VoidEmpires.Frontend/src/pages/RankingPage.tsx"
        RequiredHelpers = @("buildMarketUrl", "buildEspionageUrl", "buildGalaxyUrl", "buildAllianceUrl")
    },
    @{
        Path = "src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx"
        RequiredHelpers = @("buildPlanetUrl", "buildConstructionUrl", "buildGalaxyUrl", "buildFleetsUrl")
    },
    @{
        Path = "src/VoidEmpires.Frontend/src/pages/ModuleCabinPage.tsx"
        RequiredHelpers = @("buildPlanetUrl", "buildConstructionUrl", "buildFleetsUrl")
    }
)

$hardCodedRoutePattern = '(?m)(to|href)=["'']/(planet|construction|research|shipyard|defenses|fleets|galaxy)\b'
$routeGuardViolations = New-Object System.Collections.Generic.List[string]

foreach ($page in $routeGuardPages) {
    $pagePath = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot "..\$($page.Path)"))
    if (-not (Test-Path -LiteralPath $pagePath)) {
        $routeGuardViolations.Add("Missing protected cockpit page '$($page.Path)'.")
        continue
    }

    $pageContent = Get-Content -LiteralPath $pagePath -Raw
    if ($pageContent -match $hardCodedRoutePattern) {
        $routeGuardViolations.Add("$($page.Path) contains a hard-coded cockpit route link. Use route URL helpers to preserve query params.")
    }

    foreach ($helperName in $page.RequiredHelpers) {
        if ($pageContent -notmatch "\b$helperName\s*\(") {
            $routeGuardViolations.Add("$($page.Path) is expected to keep using $helperName(...) for context-preserving navigation.")
        }
    }
}

$frontendRouteSurfaceRoots = @(
    "src/VoidEmpires.Frontend/src/pages",
    "src/VoidEmpires.Frontend/src/components"
)
$frontendRouteSurfaceFiles = foreach ($surfaceRoot in $frontendRouteSurfaceRoots) {
    $surfacePath = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot "..\$surfaceRoot"))
    if (Test-Path -LiteralPath $surfacePath) {
        Get-ChildItem -LiteralPath $surfacePath -Recurse -Include *.ts, *.tsx -File
    }
}

$globalHardCodedRoutePattern = '(?m)(to|href)=\{?["'']/(planet|construction|research|shipyard|defenses|ground-army|fleets|galaxy|market|espionage|alliance|ranking)\b'
$globalHardCodedRouteMatches = Select-String -Path ($frontendRouteSurfaceFiles | Select-Object -ExpandProperty FullName) -Pattern $globalHardCodedRoutePattern
foreach ($match in @($globalHardCodedRouteMatches)) {
    $routeGuardViolations.Add("$($match.Path):$($match.LineNumber) contains a hard-coded cockpit route link. Use route URL helpers to preserve query params.")
}

if ($routeGuardViolations.Count -gt 0) {
    throw "Frontend cockpit route helper guard failed:`n$($routeGuardViolations -join [Environment]::NewLine)"
}

$routeUrlsPath = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot "..\src\VoidEmpires.Frontend\src\utils\routeUrls.ts"))
if (-not (Test-Path -LiteralPath $routeUrlsPath)) {
    throw "Route URL helper file was not found at '$routeUrlsPath'."
}

$routeUrlsContent = Get-Content -LiteralPath $routeUrlsPath -Raw
$requiredRouteHelperFragments = @(
    @{ Name = "buildPlanetUrl"; Pattern = 'return buildUrl\("/planet", \{ civilizationId, planetId \}\);' },
    @{ Name = "buildConstructionUrl"; Pattern = 'return buildUrl\("/construction", \{ civilizationId, planetId \}\);' },
    @{ Name = "buildResearchUrl"; Pattern = 'return buildUrl\("/research", \{ civilizationId, planetId \}\);' },
    @{ Name = "buildGroundArmyUrl"; Pattern = 'return buildUrl\("/ground-army", \{ civilizationId, planetId \}\);' },
    @{ Name = "buildShipyardUrl"; Pattern = 'return buildUrl\("/shipyard", \{ civilizationId, planetId \}\);' },
    @{ Name = "buildMarketUrl"; Pattern = 'return buildUrl\("/market", \{ civilizationId, planetId \}\);' },
    @{ Name = "buildAllianceUrl"; Pattern = 'return buildUrl\("/alliance", \{ civilizationId \}\);' },
    @{ Name = "buildDefensesUrl"; Pattern = 'return buildUrl\("/defenses", \{ civilizationId, planetId \}\);' },
    @{ Name = "buildGalaxyUrl"; Pattern = 'return buildUrl\("/galaxy", \{ civilizationId, systemId, planetId \}\);' },
    @{ Name = "buildFleetsUrl"; Pattern = 'return buildUrl\("/fleets", \{ civilizationId, planetId \}\);' },
    @{ Name = "buildEspionageUrl"; Pattern = 'return buildUrl\("/espionage", \{ civilizationId, systemId, planetId \}\);' },
    @{ Name = "buildRankingUrl"; Pattern = 'return buildUrl\("/ranking", \{ civilizationId \}\);' }
)

$routeUrlHelperViolations = New-Object System.Collections.Generic.List[string]
foreach ($helper in $requiredRouteHelperFragments) {
    if ($routeUrlsContent -notmatch $helper.Pattern) {
        $routeUrlHelperViolations.Add("$($helper.Name) must preserve civilizationId and planetId through buildUrl(...).")
    }
}

if ($routeUrlsContent -notmatch "new URLSearchParams\(\)" -or $routeUrlsContent -notmatch "\.trim\(\)") {
    $routeUrlHelperViolations.Add("buildUrl(...) must continue trimming values and using URLSearchParams instead of manual query concatenation.")
}

if ($routeUrlHelperViolations.Count -gt 0) {
    throw "Frontend route URL helper guard failed:`n$($routeUrlHelperViolations -join [Environment]::NewLine)"
}

$distAssetsPath = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot "..\src\VoidEmpires.Frontend\dist\assets"))
if (Test-Path -LiteralPath $distAssetsPath) {
    $entryChunks = @(Get-ChildItem -LiteralPath $distAssetsPath -File -Filter "index-*.js")
    $bundleViolations = New-Object System.Collections.Generic.List[string]

    if ($entryChunks.Count -ne 1) {
        $bundleViolations.Add("Expected exactly one built Vite entry chunk matching dist/assets/index-*.js after npm run build; found $($entryChunks.Count).")
    }
    else {
        $entryChunk = $entryChunks[0]
        $entryBytes = [System.IO.File]::ReadAllBytes($entryChunk.FullName)
        $entrySizeKilobytes = [Math]::Round($entryChunk.Length / 1KB, 2)

        $gzipStream = New-Object System.IO.MemoryStream
        try {
            $compressor = New-Object System.IO.Compression.GZipStream -ArgumentList $gzipStream, ([System.IO.Compression.CompressionLevel]::Optimal), $true
            try {
                $compressor.Write($entryBytes, 0, $entryBytes.Length)
            }
            finally {
                $compressor.Dispose()
            }

            $entryGzipKilobytes = [Math]::Round($gzipStream.Length / 1KB, 2)
        }
        finally {
            $gzipStream.Dispose()
        }

        $maxEntrySizeKilobytes = 210
        $maxEntryGzipKilobytes = 70
        if ($entrySizeKilobytes -gt $maxEntrySizeKilobytes) {
            $bundleViolations.Add("Built entry chunk '$($entryChunk.Name)' is $entrySizeKilobytes kB, above the $maxEntrySizeKilobytes kB guard budget.")
        }

        if ($entryGzipKilobytes -gt $maxEntryGzipKilobytes) {
            $bundleViolations.Add("Built entry chunk '$($entryChunk.Name)' is $entryGzipKilobytes kB gzip, above the $maxEntryGzipKilobytes kB gzip guard budget.")
        }
    }

    if ($bundleViolations.Count -gt 0) {
        throw "Frontend entry bundle guard failed:`n$($bundleViolations -join [Environment]::NewLine)"
    }
}

Write-Host "Frontend route lazy-import guard passed."
