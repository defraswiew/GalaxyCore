[CmdletBinding()]
param()
$ErrorActionPreference = 'Stop'
$repoRoot = [IO.Path]::GetFullPath((Split-Path -Parent $PSScriptRoot))
$manifest = Get-Content -LiteralPath (Join-Path $repoRoot 'Core/SDK-MANIFEST.json') -Raw | ConvertFrom-Json
foreach ($entry in $manifest.files) {
    $file = [IO.Path]::GetFullPath((Join-Path $repoRoot $entry.path))
    if (-not $file.StartsWith((Join-Path $repoRoot 'Core') + [IO.Path]::DirectorySeparatorChar, [StringComparison]::OrdinalIgnoreCase)) {
        throw 'SDK manifest path is outside Core.'
    }
    if (-not (Test-Path -LiteralPath $file -PathType Leaf)) { throw "Missing SDK file: $($entry.path)" }
    if ((Get-FileHash -LiteralPath $file -Algorithm SHA256).Hash -ne $entry.sha256) { throw "SDK checksum mismatch: $($entry.path)" }
}
$plugins = Join-Path $repoRoot 'Templates/SimpleMmoClient/Assets/GalaxyNetwork/Core/Plugins/netstandard2.1'
foreach ($name in @('GalaxyCoreCommon.dll', 'GalaxyCoreLib.dll', 'protobuf-net.dll', 'YamlDotNet.dll')) {
    $expected = (Get-FileHash -LiteralPath (Join-Path $repoRoot "Core/Client/netstandard2.1/$name") -Algorithm SHA256).Hash
    $actual = (Get-FileHash -LiteralPath (Join-Path $plugins $name) -Algorithm SHA256).Hash
    if ($expected -ne $actual) { throw "Unity plugin does not match UnityRelease SDK: $name" }
    $duplicates = Get-ChildItem -LiteralPath (Join-Path $repoRoot 'Templates/SimpleMmoClient/Assets') -Filter $name -File -Recurse
    if (@($duplicates).Count -ne 1) { throw "Expected exactly one Unity copy of $name" }
}
if (Test-Path -LiteralPath (Join-Path $plugins 'Newtonsoft.Json.dll')) { throw 'Unity uses the official Newtonsoft.Json package; remove a duplicate plugin.' }
Write-Host "SDK verification passed: $(@($manifest.files).Count) hashes; Unity plugins match; source $($manifest.sourceCommit)."
