[CmdletBinding()]
param(
    [ValidateSet('Audit', 'Dependencies', 'Mono', 'Il2Cpp', 'Navigation')]
    [string]$Action = 'Audit',
    [string]$EditorPath = 'C:\Program Files\Unity\Hub\Editor\6000.3.24f1\Editor\Unity.exe'
)
$ErrorActionPreference = 'Stop'
if (-not (Test-Path -LiteralPath $EditorPath -PathType Leaf)) {
    throw 'Unity 6000.3.24f1 is required. Pass -EditorPath when installed elsewhere.'
}
$repoRoot = [IO.Path]::GetFullPath((Split-Path -Parent $PSScriptRoot))
$projectPath = Join-Path $repoRoot 'Templates/SimpleMmoClient'
$artifactPath = Join-Path $repoRoot '.artifacts'
New-Item -ItemType Directory -Path $artifactPath -Force | Out-Null
$methods = @{
    Audit = 'GalaxyMigrationChecks.Audit'
    Dependencies = 'GalaxyMigrationChecks.ExportDependencies'
    Mono = 'GalaxyMigrationChecks.BuildWindowsMono'
    Il2Cpp = 'GalaxyMigrationChecks.BuildWindowsIl2Cpp'
    Navigation = 'GalaxyMigrationChecks.BakeNavigationAssets'
}
$logPath = Join-Path $artifactPath ('unity-' + $Action.ToLowerInvariant() + '.log')
$arguments = @('-batchmode', '-nographics', '-quit', '-accept-apiupdate',
    '-projectPath', ('"' + $projectPath + '"'), '-executeMethod', $methods[$Action],
    '-logFile', ('"' + $logPath + '"'))
$run = Start-Process -FilePath $EditorPath -ArgumentList $arguments -WindowStyle Hidden -PassThru -Wait
if ($run.ExitCode -ne 0) { throw "Unity $Action failed ($($run.ExitCode)). See $logPath" }
Write-Host "Unity $Action passed. Log: $logPath"
exit 0
