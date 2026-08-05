[CmdletBinding()]
param(
    [string]$UnityPath
)

$ErrorActionPreference = 'Stop'
$ProjectRoot = Split-Path -Parent $PSScriptRoot
$VersionLine = Get-Content -LiteralPath (Join-Path $ProjectRoot 'ProjectSettings\ProjectVersion.txt') | Select-Object -First 1
$UnityVersion = ($VersionLine -split ':', 2)[1].Trim()

if (-not $UnityPath) {
    $UnityPath = Join-Path $env:ProgramFiles "Unity\Hub\Editor\$UnityVersion\Editor\Unity.exe"
}
if (-not (Test-Path -LiteralPath $UnityPath)) {
    throw "Unity Editor bulunamadı: $UnityPath. MANUAL_STEPS.md dosyasını izleyin veya -UnityPath verin."
}

$ResultRoot = Join-Path $ProjectRoot 'TestResults'
New-Item -ItemType Directory -Force -Path $ResultRoot | Out-Null

function Invoke-UnityTests([string]$Platform) {
    $result = Join-Path $ResultRoot "$Platform-results.xml"
    $log = Join-Path $ResultRoot "$Platform.log"
    $assemblyName = "DogaRun.Tests.$Platform"
    $arguments = "-batchmode -nographics -projectPath `"$ProjectRoot`" -runTests -testPlatform $Platform -assemblyNames $assemblyName -testResults `"$result`" -logFile `"$log`""
    $process = Start-Process -FilePath $UnityPath -ArgumentList $arguments -WindowStyle Hidden -PassThru
    $process.WaitForExit()
    if ($process.ExitCode -ne 0) { throw "$Platform testleri başarısız. ExitCode=$($process.ExitCode), log=$log" }
    Write-Host "[OK] $Platform -> $result"
}

Invoke-UnityTests 'EditMode'
Invoke-UnityTests 'PlayMode'
