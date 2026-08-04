[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
$RequiredUnityVersion = '6000.3.21f1'
$ProjectRoot = Split-Path -Parent $PSScriptRoot

function Find-UnityEditor {
    $candidate = Join-Path $env:ProgramFiles "Unity\Hub\Editor\$RequiredUnityVersion\Editor\Unity.exe"
    if (Test-Path -LiteralPath $candidate) { return $candidate }

    $found = Get-ChildItem -LiteralPath (Join-Path $env:ProgramFiles 'Unity\Hub\Editor') -Filter Unity.exe -Recurse -ErrorAction SilentlyContinue |
        Select-Object -First 1 -ExpandProperty FullName
    return $found
}

Write-Host "Doğa Koşusu Windows ortam kontrolü"
Write-Host "Proje: $ProjectRoot"

$commands = @('git', 'gh', 'git-lfs')
foreach ($commandName in $commands) {
    $command = Get-Command $commandName -ErrorAction SilentlyContinue
    if ($command) { Write-Host "[OK] $commandName -> $($command.Source)" }
    else { Write-Warning "[EKSİK] $commandName" }
}

$hubCandidates = @(
    (Join-Path $env:ProgramFiles 'Unity Hub\Unity Hub.exe'),
    (Join-Path $env:LOCALAPPDATA 'Programs\Unity Hub\Unity Hub.exe')
)
$hub = $hubCandidates | Where-Object { Test-Path -LiteralPath $_ } | Select-Object -First 1
if ($hub) { Write-Host "[OK] Unity Hub -> $hub" }
else { Write-Warning '[EKSİK] Unity Hub. MANUAL_STEPS.md dosyasını izleyin.' }

$unity = Find-UnityEditor
if ($unity) {
    Write-Host "[OK] Unity Editor -> $unity"
    $editorRoot = Split-Path -Parent (Split-Path -Parent $unity)
    $androidRoot = Join-Path $editorRoot 'Editor\Data\PlaybackEngines\AndroidPlayer'
    if (Test-Path -LiteralPath $androidRoot) { Write-Host '[OK] Android Build Support' }
    else { Write-Warning '[EKSİK] Android Build Support/SDK/NDK/OpenJDK modüllerini Hub ile ekleyin.' }
}
else {
    Write-Warning "[EKSİK] Unity $RequiredUnityVersion"
}

if (Get-Command git -ErrorAction SilentlyContinue) {
    git -C $ProjectRoot lfs install --local
    Write-Host '[OK] Git LFS repository hook yapılandırıldı.'
}

Write-Host 'Kontrol tamamlandı. Bu script ağır yazılım yüklemez ve tekrar çalıştırılabilir.'
