param(
    [string]$ScriptPath = ".\artifacts\sqlserver\VoidEmpires_Dev_SqlServerInitialBaseline.sql"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$repoRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot ".."))
$resolvedScriptPath = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($ScriptPath)
if (-not [System.IO.Path]::IsPathRooted($ScriptPath)) {
    $resolvedScriptPath = [System.IO.Path]::GetFullPath((Join-Path $repoRoot $ScriptPath))
}

if (-not (Test-Path -LiteralPath $resolvedScriptPath)) {
    throw "SQL Server generated script was not found at '$resolvedScriptPath'."
}

if ([System.IO.Path]::GetExtension($resolvedScriptPath) -ne ".sql") {
    throw "SQL Server generated script safety check only accepts .sql files."
}

$content = Get-Content -LiteralPath $resolvedScriptPath -Raw -Encoding UTF8
$lines = Get-Content -LiteralPath $resolvedScriptPath -Encoding UTF8

$requiredFragments = @(
    "Generated for manual review only",
    "Do not execute without operator approval",
    "__EFMigrationsHistory",
    "IF NOT EXISTS",
    "20260706131610_SqlServerInitialBaseline"
)

foreach ($fragment in $requiredFragments) {
    if ($content -notlike "*$fragment*") {
        throw "SQL Server generated script safety check expected fragment '$fragment'."
    }
}

$unsafePatterns = @(
    [pscustomobject]@{ Name = "DROP DATABASE"; Pattern = "(?i)^\s*DROP\s+DATABASE\b" },
    [pscustomobject]@{ Name = "DROP LOGIN"; Pattern = "(?i)^\s*DROP\s+LOGIN\b" },
    [pscustomobject]@{ Name = "DROP USER"; Pattern = "(?i)^\s*DROP\s+USER\b" },
    [pscustomobject]@{ Name = "TRUNCATE TABLE"; Pattern = "(?i)^\s*TRUNCATE\s+TABLE\b" },
    [pscustomobject]@{ Name = "CREATE or ALTER LOGIN"; Pattern = "(?i)^\s*(CREATE|ALTER)\s+LOGIN\b" },
    [pscustomobject]@{ Name = "CREATE or ALTER USER"; Pattern = "(?i)^\s*(CREATE|ALTER)\s+USER\b" },
    [pscustomobject]@{ Name = "PASSWORD connection-string or login literal"; Pattern = "(?i)\bPASSWORD\s*=" },
    [pscustomobject]@{ Name = "USER ID connection-string literal"; Pattern = "(?i)\bUSER\s+ID\s*=" },
    [pscustomobject]@{ Name = "UID connection-string literal"; Pattern = "(?i)\bUID\s*=" },
    [pscustomobject]@{ Name = "SERVER connection-string literal"; Pattern = "(?i)\bSERVER\s*=" },
    [pscustomobject]@{ Name = "DATA SOURCE connection-string literal"; Pattern = "(?i)\bDATA\s+SOURCE\s*=" },
    [pscustomobject]@{ Name = "ADDRESS connection-string literal"; Pattern = "(?i)\bADDR(ESS)?\s*=" }
)

$violations = New-Object System.Collections.Generic.List[string]

for ($lineIndex = 0; $lineIndex -lt $lines.Count; $lineIndex++) {
    $line = $lines[$lineIndex]
    foreach ($unsafePattern in $unsafePatterns) {
        if ($line -match $unsafePattern.Pattern) {
            $violations.Add(("{0}:{1}: unsafe SQL script fragment detected ({2}): {3}" -f $resolvedScriptPath, ($lineIndex + 1), $unsafePattern.Name, $line.Trim()))
        }
    }
}

if ($violations.Count -gt 0) {
    throw "SQL Server generated script safety check failed:`n$($violations -join [Environment]::NewLine)"
}

Write-Host "SQL Server generated script safety check passed."
