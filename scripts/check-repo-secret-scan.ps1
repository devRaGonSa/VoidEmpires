Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$repoRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot ".."))
$docsRoot = Join-Path $repoRoot "docs"
$scriptsRoot = Join-Path $repoRoot "scripts"
$testsRoot = Join-Path $repoRoot "tests"
$migrationRoot = Join-Path $repoRoot "src\VoidEmpires.Infrastructure\Persistence\Migrations"
$sqlServerArtifactsRoot = Join-Path $repoRoot "artifacts\sqlserver"

$scanFiles = New-Object System.Collections.Generic.List[System.IO.FileInfo]
function Add-SecretScanFiles {
    param(
        [System.IO.FileInfo[]]$Files
    )

    if ($null -ne $Files) {
        $scanFiles.AddRange($Files)
    }
}

Add-SecretScanFiles ([System.IO.FileInfo[]](Get-ChildItem -Path $docsRoot -Recurse -Include *.md,*.sql -File))
Add-SecretScanFiles ([System.IO.FileInfo[]](Get-ChildItem -Path $scriptsRoot -Recurse -Include *.ps1,*.sql -File))
Add-SecretScanFiles ([System.IO.FileInfo[]](Get-ChildItem -Path $repoRoot -Recurse -Include appsettings*.json -File |
    Where-Object { $_.FullName -notmatch "\\bin\\" -and $_.FullName -notmatch "\\obj\\" }))
if (Test-Path -LiteralPath $migrationRoot) {
    Add-SecretScanFiles ([System.IO.FileInfo[]](Get-ChildItem -Path $migrationRoot -Recurse -Include *.cs -File))
}
if (Test-Path -LiteralPath $sqlServerArtifactsRoot) {
    Add-SecretScanFiles ([System.IO.FileInfo[]](Get-ChildItem -Path $sqlServerArtifactsRoot -Recurse -Include *.sql -File))
}

$authFixtureFiles = @()
if (Test-Path -LiteralPath $testsRoot) {
    $authFixtureFiles = Get-ChildItem -Path $testsRoot -Recurse -Include *Account*Tests.cs,*Auth*Tests.cs,*AuthenticatedPlayableLoopSmokeTests.cs -File |
        Where-Object { $_.FullName -notmatch "\\bin\\" -and $_.FullName -notmatch "\\obj\\" }
}

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

$allowedUserValues = @(
    '<USER>',
    'YOUR_USER',
    '${SQL_USER}',
    '%SQL_USER%'
)

$allowedAuthFixtureCredentialValues = @(
    '',
    'P@ssw0rd!23',
    'WrongP@ssw0rd!23',
    'OtherP@ssw0rd!23',
    'test-password',
    'weak',
    'different'
)

$violations = New-Object System.Collections.Generic.List[string]
$passwordAssignmentPattern = '(?i)Password\s*=\s*("?)([^;"\s`]+)\1'
$userAssignmentPattern = '(?i)(User\s+Id|UID)\s*=\s*("?)([^;"\s`]+)\2'
$authFixtureAssignmentPattern = '(?i)\b(password|confirmPassword)\s*=\s*"([^"]*)"'

function Test-IsAllowedPasswordValue {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Value
    )

    return $allowedPasswordValues -contains $Value
}

function Test-IsAllowedUserValue {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Value
    )

    return $allowedUserValues -contains $Value
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

        if ($line -match '(?i)(Server|Data\s+Source)\s*=' -and $line -match '(?i)Password\s*=') {
            foreach ($match in [regex]::Matches($line, $userAssignmentPattern)) {
                $value = $match.Groups[3].Value
                if (Test-IsAllowedUserValue -Value $value) {
                    continue
                }

                if ($value -match '(?i)^sa$') {
                    $violations.Add(("{0}:{1}: SQL Server sa login must not be committed in connection examples: {2}" -f $file.FullName, ($lineIndex + 1), $line.Trim()))
                    continue
                }

                $violations.Add(("{0}:{1}: hardcoded SQL Server user value detected in connection example: {2}" -f $file.FullName, ($lineIndex + 1), $line.Trim()))
            }
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

foreach ($file in @($authFixtureFiles)) {
    $lines = Get-Content -LiteralPath $file.FullName -Encoding UTF8
    for ($lineIndex = 0; $lineIndex -lt $lines.Count; $lineIndex++) {
        $line = $lines[$lineIndex]
        foreach ($match in [regex]::Matches($line, $authFixtureAssignmentPattern)) {
            $value = $match.Groups[2].Value
            if ($allowedAuthFixtureCredentialValues -contains $value) {
                continue
            }

            $violations.Add(("{0}:{1}: auth test credential assignment must use an approved fake/local value: {2}" -f $file.FullName, ($lineIndex + 1), $line.Trim()))
        }
    }
}

$qaRegistrationHelperPath = Join-Path $scriptsRoot "dev-qa-register-test-user.ps1"
if (Test-Path -LiteralPath $qaRegistrationHelperPath) {
    $qaRegistrationHelperContent = Get-Content -LiteralPath $qaRegistrationHelperPath -Raw -Encoding UTF8
    $requiredQaRegistrationFragments = @(
        'example.test',
        'Tmp!7$suffix',
        'Password: supplied by caller and intentionally not printed.',
        'It is not stored by this script.'
    )

    foreach ($fragment in $requiredQaRegistrationFragments) {
        if ($qaRegistrationHelperContent -notlike "*$fragment*") {
            $violations.Add(("{0}: local QA registration helper is missing fake/local credential safety fragment: {1}" -f $qaRegistrationHelperPath, $fragment))
        }
    }
}

if ($violations.Count -gt 0) {
    throw "Repository secret scan failed:`n$($violations -join [Environment]::NewLine)"
}

Write-Host "Repository secret scan passed."
