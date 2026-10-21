[CmdletBinding()]
param(
    [ValidateSet('Mono', 'Il2Cpp', 'Both')]
    [string]$Backend = 'Both',
    [ValidateRange(1024, 65535)]
    [int]$Port = 30421,
    [switch]$AddForce
)
$ErrorActionPreference = 'Stop'
$repoRoot = [IO.Path]::GetFullPath((Split-Path -Parent $PSScriptRoot))
$artifactPath = Join-Path $repoRoot '.artifacts'
New-Item -ItemType Directory -Path $artifactPath -Force | Out-Null
$profiles = if ($Backend -eq 'Both') { @('mono', 'il2cpp') } else { @($Backend.ToLowerInvariant()) }
foreach ($profile in $profiles) {
    $executable = Join-Path $artifactPath "build/windows-$profile/GalaxyExamples.exe"
    if (-not (Test-Path -LiteralPath $executable -PathType Leaf)) {
        throw "Build $profile first with build/unity.ps1. Missing: $executable"
    }
}
if (Get-NetUDPEndpoint -LocalPort $Port -ErrorAction SilentlyContinue) {
    throw "UDP $Port is already in use. Select a free local port."
}
$serverProject = Join-Path $repoRoot 'Templates/SimpleMmoServer/SimpleMmoServer/SimpleMmoServer.csproj'
& dotnet build $serverProject -c Release --nologo -v quiet
if ($LASTEXITCODE -ne 0) { throw 'Server build failed.' }
$serverDll = Join-Path $repoRoot 'Templates/SimpleMmoServer/SimpleMmoServer/bin/Release/net8.0/SimpleMmoServer.dll'
$server = $null
$player = $null
try {
    $server = Start-Process -FilePath 'dotnet' -ArgumentList @(('"' + $serverDll + '"'), '--port', $Port) `
        -WorkingDirectory $repoRoot -WindowStyle Hidden -PassThru `
        -RedirectStandardOutput (Join-Path $artifactPath 'player-smoke-server.log') `
        -RedirectStandardError (Join-Path $artifactPath 'player-smoke-server-errors.log')
    $deadline = [DateTime]::UtcNow.AddSeconds(15)
    do {
        if ($server.HasExited) { throw 'Smoke server exited. See .artifacts/player-smoke-server*.log.' }
        $endpoint = Get-NetUDPEndpoint -LocalPort $Port -ErrorAction SilentlyContinue |
            Where-Object OwningProcess -eq $server.Id
        if ($endpoint) { break }
        Start-Sleep -Milliseconds 100
    } while ([DateTime]::UtcNow -lt $deadline)
    if (-not $endpoint) { throw 'Smoke server did not bind its UDP port within 15 seconds.' }
    foreach ($profile in $profiles) {
        $executable = Join-Path $artifactPath "build/windows-$profile/GalaxyExamples.exe"
        $suffix = if ($AddForce) { '-add-force' } else { '' }
        $logPath = Join-Path $artifactPath "player-$profile$suffix-smoke.log"
        $playerArguments = @('-batchmode', '-nographics', "--galaxy-smoke=127.0.0.1:$Port", '-logFile', ('"' + $logPath + '"'))
        if ($AddForce) { $playerArguments += '--galaxy-smoke-add-force' }
        $player = Start-Process -FilePath $executable -WindowStyle Hidden -PassThru `
            -ArgumentList $playerArguments
        if (-not $player.WaitForExit(90000)) { throw "$profile player exceeded the 90 second timeout. See $logPath" }
        $player.Refresh()
        $passed = Select-String -LiteralPath $logPath -SimpleMatch 'GALAXY_PLAYER_SMOKE_OK:' -Quiet
        if ($player.ExitCode -ne 0 -or -not $passed) {
            throw "$profile player failed (exit $($player.ExitCode)). See $logPath"
        }
        Write-Host "$profile player smoke passed. Log: $logPath"
        $player.Dispose()
        $player = $null
    }
}
finally {
    # Stop only the processes created by this invocation.
    foreach ($ownedProcess in @($player, $server)) {
        if ($null -ne $ownedProcess) {
            if (-not $ownedProcess.HasExited) { $ownedProcess.Kill(); $ownedProcess.WaitForExit() }
            $ownedProcess.Dispose()
        }
    }
}
