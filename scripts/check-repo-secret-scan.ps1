Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$repoRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot ".."))
$docsRoot = Join-Path $repoRoot "docs"
$scriptsRoot = Join-Path $repoRoot "scripts"

$scanFiles = New-Object System.Collections.Generic.List[System.IO.FileInfo]
$scanFiles.AddRange([System.IO.FileInfo[]](Get-ChildItem -Path $docsRoot -Recurse -Include *.md -File))
$scanFiles.AddRange([System.IO.FileInfo[]](Get-ChildItem -Path $scriptsRoot -Recurse -Include *.ps1 -File))
$scanFiles.AddRange([System.IO.FileInfo[]](Get-ChildItem -Path $repoRoot -Recurse -Include appsettings*.json -File |
    Where-Object { $_.FullName -notmatch "\\bin\\" -and $_.FullName -notmatch "\\obj\\" }))

$allowedPasswordValues = @(
    '<PASSWORD>',
    '<local-password>',
    'YOUR_LOCAL_PASSWORD',
    'YOUR_PASSWORD',
    'LOCAL_PASSWORD',
    '<PASSWORD_PLACEHOLDER>',
    '${SQL_PASSWORD}',
    '%SQL_PASSWORD%'
)

$allowedSecretValues = @(
    '""',
    "''",
    '<API_KEY>',
    '<SECRET>',
    '<TOKEN>',
    'YOUR_LOCAL_PASSWORD',
    'LOCAL_PASSWORD'
)

$violations = New-Object System.Collections.Generic.List[string]
$passwordAssignmentPattern = '(?i)Password\s*=\s*("?)([^;"\s`]+)\1'

function Test-IsAllowedPasswordValue {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Value
    )

    return $allowedPasswordValues -contains $Value
}

foreach ($file in $scanFiles) {
    $lines = Get-Content -LiteralPath $file.FullName -Encoding UTF8
    for ($lineIndex = 0; $lineIndex -lt $lines.Count; $lineIndex++) {
        $line = $lines[$lineIndex]

        foreach ($match in [regex]::Matches($line, $passwordAssignmentPattern)) {
            $value = $match.Groups[2].Value
            if (Test-IsAllowedPasswordValue -Value $value) {
                continue
            }
            if ($line -like '*unsafe connection-string password example detected*') {
                continue
            }

            if ($line -match '192\.168\.178\.28') {
                $violations.Add(("{0}:{1}: real-looking password near known SQL Server target detected: {2}" -f $file.FullName, ($lineIndex + 1), $line.Trim()))
                continue
            }

            if ($line -match '(?i)User\s+Id\s*=\s*"?sa"?') {
                $violations.Add(("{0}:{1}: real-looking password for SQL Server sa login detected: {2}" -f $file.FullName, ($lineIndex + 1), $line.Trim()))
                continue
            }

            $violations.Add(("{0}:{1}: unsafe password assignment detected: {2}" -f $file.FullName, ($lineIndex + 1), $line.Trim()))
        }

        foreach ($match in [regex]::Matches($line, '(?i)(ApiKey|Secret|Token)\s*[:=]\s*("?)([^",;\s`]+)\2')) {
            $value = $match.Groups[3].Value
            if ($allowedSecretValues -contains $value) {
                continue
            }

            $violations.Add(("{0}:{1}: possible committed secret value detected: {2}" -f $file.FullName, ($lineIndex + 1), $line.Trim()))
        }
    }
}

if ($violations.Count -gt 0) {
    throw "Repository secret scan failed:`n$($violations -join [Environment]::NewLine)"
}

Write-Host "Repository secret scan passed."
