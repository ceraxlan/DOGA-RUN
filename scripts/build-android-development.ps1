[CmdletBinding()]
param([string]$UnityPath)

$ErrorActionPreference = 'Stop'
$ProjectRoot = Split-Path -Parent $PSScriptRoot
$UnityVersion = ((Get-Content -LiteralPath (Join-Path $ProjectRoot 'ProjectSettings\ProjectVersion.txt') | Select-Object -First 1) -split ':', 2)[1].Trim()
if (-not $UnityPath) { $UnityPath = Join-Path $env:ProgramFiles "Unity\Hub\Editor\$UnityVersion\Editor\Unity.exe" }
if (-not (Test-Path -LiteralPath $UnityPath)) { throw "Unity Editor bulunamadı: $UnityPath" }

$LogPath = Join-Path $ProjectRoot 'Logs\android-development.log'
New-Item -ItemType Directory -Force -Path (Split-Path -Parent $LogPath) | Out-Null
$arguments = "-batchmode -quit -projectPath `"$ProjectRoot`" -executeMethod DogaRun.Editor.AndroidBuildCommand.BuildDevelopment -logFile `"$LogPath`""
$process = Start-Process -FilePath $UnityPath -ArgumentList $arguments -WindowStyle Hidden -PassThru
$process.WaitForExit()
if ($process.ExitCode -ne 0) { throw "Development build başarısız. ExitCode=$($process.ExitCode), log=$LogPath" }
Write-Host '[OK] Builds/Android/DogaRun-development.apk'
