# UNIGINE client template

This template demonstrates authentication against `SimpleMmoServer`, room discovery and entry,
world and room chat, and live network statistics. Start the server template first; the client
defaults to `127.0.0.1:30200`, application name `SimpleMmoServer`.

The client uses `GalaxyConnection` and its instance-scoped API/events. Its shared authentication
contracts are built from `../SimpleMmoServer/SimpleMmoCommon`; do not copy old template DLLs into
the project. Galaxy binaries and their dependencies come from
`../../Core/Client/Managed/netstandard2.1`.

## Engine requirements

The managed application targets **.NET 8**; Galaxy targets **.NET Standard 2.1**. Install the
.NET 8 SDK and runtime. The networking adapter has been compiled against the current Galaxy SDK
and shared authentication contracts on .NET 8, using compile-only UNIGINE surface stubs.
This checks Galaxy API compatibility; it does not validate the real engine API or native runtime.

The included worlds, components and project descriptor retain their original **UNIGINE 2.15.1**
asset format. Before running the example, select a UNIGINE C# SDK/native deployment compatible
with a .NET 8 host, migrate the project and assets using that SDK's SDK Browser tools, and validate
world loading, components and native interoperability. Compatibility of the original 2.15.1
engine binaries with .NET 8 has not been established.

Restore the matching engine binaries into `GalaxyNetworkTemplate/bin`, including
`UnigineSharp_x64[d].dll`, the native engine libraries, plugins and `cspropgen_x64[d].dll`.
They are not distributed here. The managed build generates its .NET 8 runtime configuration;
the launch scripts invoke that generated application through `dotnet`.

```powershell
dotnet build .\GalaxyNetworkTemplate\GalaxyNetworkTemplate.csproj -c Release
.\GalaxyNetworkTemplate\launch_release.bat
```

For managed compilation against an SDK in another directory, pass
`-p:UnigineBinPath="C:\path\to\engine\bin"`. Running the example still requires the full matching
engine deployment in the template's `bin` directory. `-p:SkipPostBuild=true` skips the engine
component-property generator for compilation-only checks.

`Shutdown` releases subscriptions, UI windows and the connection. Disconnecting during normal
operation restores the login window, so reconnecting does not reuse disposed widgets or duplicate
event subscriptions. Ping is displayed as round-trip milliseconds.

Full compilation, world loading and engine runtime checks require the matching UNIGINE SDK and
remain unverified without it. A .NET build without the SDK stops with a diagnostic naming the
missing SDK DLL and the required engine migration.
