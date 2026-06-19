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
    'LOCAL_PASSWORD',
    '<PASSWORD_PLACEHOLDER>'
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

foreach ($file in $scanFiles) {
    $lines = Get-Content -LiteralPath $file.FullName -Encoding UTF8
    for ($lineIndex = 0; $lineIndex -lt $lines.Count; $lineIndex++) {
        $line = $lines[$lineIndex]

        foreach ($match in [regex]::Matches($line, '(?i)Password\s*=\s*("?)([^;"\s`]+)\1')) {
            $value = $match.Groups[2].Value
            if ($allowedPasswordValues -contains $value) {
                continue
            }
            if ($line -like '*unsafe connection-string password example detected*') {
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
