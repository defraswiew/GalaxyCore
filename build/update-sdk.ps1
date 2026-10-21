[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$SourcePath,
    [switch]$SkipExamples
)

$ErrorActionPreference = 'Stop'
$repoRoot = [IO.Path]::GetFullPath((Split-Path -Parent $PSScriptRoot))
$sourceRoot = (Resolve-Path -LiteralPath $SourcePath).Path
$stageRoot = Join-Path $repoRoot '.artifacts/sdk-build'
$profiles = @(
    @{ Name = 'Client'; Project = 'GalaxyCoreLib/GalaxyCoreLib.csproj'; Configuration = 'UnityRelease'; Destination = 'Core/Client/netstandard2.1' },
    @{ Name = 'Managed'; Project = 'GalaxyCoreLib/GalaxyCoreLib.csproj'; Configuration = 'Release'; Destination = 'Core/Client/Managed/netstandard2.1' },
    @{ Name = 'Server'; Project = 'GalaxyCore/GalaxyCoreServer.csproj'; Configuration = 'Release'; Destination = 'Core/Server/netstandard2.1' },
    @{ Name = 'Physics'; Project = 'GalaxyCore.Physics.Jitter2/GalaxyCore.Physics.Jitter2.csproj'; Configuration = 'Release'; Destination = 'Core/Server/Physics/net8.0' }
)

foreach ($profile in $profiles) {
    $projectPath = Join-Path $sourceRoot $profile.Project
    if (-not (Test-Path -LiteralPath $projectPath -PathType Leaf)) {
        throw "Galaxy source project not found: $projectPath"
    }
    $output = Join-Path $stageRoot $profile.Name
    & dotnet build $projectPath -c $profile.Configuration '-p:SkipGalaxyLocalDeploy=true' `
        '-p:CopyLocalLockFileAssemblies=true' -o $output '-clp:ErrorsOnly;Summary'
    if ($LASTEXITCODE -ne 0) { throw "SDK build failed: $($profile.Name)" }
}

foreach ($profile in $profiles) {
    $output = Join-Path $stageRoot $profile.Name
    $destination = Join-Path $repoRoot $profile.Destination
    New-Item -ItemType Directory -Path $destination -Force | Out-Null
    if ($profile.Name -eq 'Physics') {
        $files = Get-ChildItem -LiteralPath $output -File | Where-Object { $_.Name -in @('GalaxyCore.Physics.Jitter2.dll', 'GalaxyCore.Physics.Jitter2.xml') }
    } else {
        $files = Get-ChildItem -LiteralPath $output -File | Where-Object { $_.Extension -in @('.dll', '.xml') }
    }
    foreach ($file in $files) { Copy-Item -LiteralPath $file.FullName -Destination (Join-Path $destination $file.Name) -Force }
}

$pluginRoot = Join-Path $repoRoot 'Templates/SimpleMmoClient/Assets/GalaxyNetwork/Core/Plugins/netstandard2.1'
New-Item -ItemType Directory -Path $pluginRoot -Force | Out-Null
# Unity supplies Newtonsoft.Json through its official package to avoid duplicate assemblies.
foreach ($fileName in @('GalaxyCoreCommon.dll', 'GalaxyCoreCommon.xml', 'GalaxyCoreLib.dll', 'GalaxyCoreLib.xml', 'protobuf-net.dll', 'protobuf-net.xml', 'YamlDotNet.dll')) {
    Copy-Item -LiteralPath (Join-Path $repoRoot "Core/Client/netstandard2.1/$fileName") -Destination (Join-Path $pluginRoot $fileName) -Force
}

if (-not $SkipExamples) {
    $commonProject = Join-Path $repoRoot 'Templates/SimpleMmoServer/SimpleMmoCommon/SimpleMmoCommon.csproj'
    $commonOutput = Join-Path $stageRoot 'ExampleCommon'
    & dotnet build $commonProject -c Release -o $commonOutput '-clp:ErrorsOnly;Summary'
    if ($LASTEXITCODE -ne 0) { throw 'Shared example model build failed.' }
    foreach ($fileName in @('SimpleMmoCommon.dll', 'SimpleMmoCommon.xml')) {
        $commonFile = Join-Path $commonOutput $fileName
        if (Test-Path -LiteralPath $commonFile) { Copy-Item -LiteralPath $commonFile -Destination (Join-Path $pluginRoot $fileName) -Force }
    }
}

$licenseRoot = Join-Path $repoRoot 'Core/ThirdParty/Jitter2'
New-Item -ItemType Directory -Path $licenseRoot -Force | Out-Null
foreach ($fileName in @('LICENSE', 'THIRD-PARTY-NOTICES.txt', 'SOURCE.md')) {
    Copy-Item -LiteralPath (Join-Path $sourceRoot "GalaxyCore.Physics.Jitter2/ThirdParty/Jitter2/$fileName") -Destination (Join-Path $licenseRoot $fileName) -Force
}

$restoreAssets = Get-Content -LiteralPath (Join-Path $sourceRoot 'GalaxyCoreCommon/obj/project.assets.json') -Raw | ConvertFrom-Json
$packageRoot = @($restoreAssets.packageFolders.PSObject.Properties.Name)[0]
foreach ($dependency in @(@{ Name = 'Newtonsoft.Json'; License = 'LICENSE.md' }, @{ Name = 'YamlDotNet'; License = 'LICENSE.txt' })) {
    $libraryKey = @($restoreAssets.libraries.PSObject.Properties.Name | Where-Object { $_ -like ($dependency.Name + '/*') })[0]
    if (-not $libraryKey) { throw "Restored dependency not found: $($dependency.Name)" }
    $packageFolder = Join-Path $packageRoot $libraryKey.ToLowerInvariant()
    $dependencyDestination = Join-Path $repoRoot ('Core/ThirdParty/' + $dependency.Name)
    New-Item -ItemType Directory -Path $dependencyDestination -Force | Out-Null
    Copy-Item -LiteralPath (Join-Path $packageFolder $dependency.License) -Destination (Join-Path $dependencyDestination $dependency.License) -Force
}

$sourceCommit = (& git -C $sourceRoot rev-parse HEAD).Trim()
if ($LASTEXITCODE -ne 0) { throw 'Unable to read SDK source revision.' }
$fileHashes = foreach ($profile in $profiles) {
    $destination = Join-Path $repoRoot $profile.Destination
    foreach ($file in (Get-ChildItem -LiteralPath $destination -File | Sort-Object Name)) {
        [ordered]@{ path = ($profile.Destination + '/' + $file.Name); sha256 = (Get-FileHash -LiteralPath $file.FullName -Algorithm SHA256).Hash.ToLowerInvariant() }
    }
}
$manifest = [ordered]@{
    sourceCommit = $sourceCommit
    numericModel = 'float'
    unityProfile = 'UnityRelease (GALAXY_AOT)'
    managedProfile = 'Release'
    serverProfile = 'Release'
    physicsTarget = 'net8.0'
    files = @($fileHashes)
}
[IO.File]::WriteAllText((Join-Path $repoRoot 'Core/SDK-MANIFEST.json'), ($manifest | ConvertTo-Json -Depth 6) + [Environment]::NewLine, [Text.UTF8Encoding]::new($false))
Write-Host "Updated SDK from source commit $sourceCommit"
