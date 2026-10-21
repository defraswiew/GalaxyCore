# Simple MMO server

This .NET 8 template uses the SDK in `Core/Server/netstandard2.1` and the optional Jitter2 backend in `Core/Server/Physics/net8.0`.
The shared `SimpleMmoCommon` project targets .NET Standard 2.1 so the same authentication, approval and loot contracts work in Unity 6 and managed clients.

From the repository root:

```powershell
dotnet build Templates/SimpleMmoServer/SimpleMmoServer.sln -c Release
dotnet run --project Templates/SimpleMmoServer/SimpleMmoServer -c Release -- --port 30200
```

Use `exit` or Ctrl+C to stop the server. The server also works with closed stdin in a container or service.
The example accepts logins containing `test`; its registration handler accepts nonempty credentials. Replace these demo handlers with account storage and credential validation for your game.

## Examples

The client creates rooms by type: `1` is the force/physics swarm, `2` is movement and GalaxyVars, `4` is the physics/video room, and `5` is navigation. Other types use the empty-room example.
The source also includes invocation, building persistence and RPG examples for custom server-created locations.

The Jitter2 backend is registered explicitly before `GalaxyCore.Start`. A room enables its world with `Physics.Start("jitter2")`; entities create bodies with `Physics.Create(...)`.
The core applies physics snapshots to transforms automatically. The physics world uses a fixed 60 Hz simulation step while room/network frames can run more slowly.

`Content/navigation/NavigationExample.gnm` is the navigation bake shipped with the Unity scene. It is copied beside the server executable on build and publish.
To use Unity scene collision geometry, export a GPHB bake with **Galaxy Network → Physics** and place it at `Content/physics/ExamplePhys2.gphb` before building.
The physics room supplies a floor when no bake is installed. The importer supports primitive colliders, convex hulls, triangle meshes, height fields and compound poses, with materials, triggers and collision layers.
It expects Unity's `LeftHandedYUp` coordinates and converts bake units to metres.

Application messages now use `GalaxyMessage`/`GalaxyField`; legacy protobuf authentication and old `.phys` files are incompatible and should be regenerated.
The navigation `.gnm` format remains supported. The building example writes its own new GalaxySerializer map files under `Maps`; old protobuf map saves must be converted or regenerated.
Building-map persistence is deliberately separate from baked physics geometry.

## Local verification

```powershell
dotnet run --project Templates/SimpleMmoServer/SimpleMmoServer -c Release -- --port 30420 --smoke-test
```

The bounded smoke check starts all eight example room types, initializes dynamic physics and GalaxyVars, loads every supported GPHB shape, verifies shared message serialization, then stops cleanly.
It does not require an external server. Use a free local UDP port.

Building `SimpleMmoCommon` has no side effects in the Unity `Assets` folder. After changing shared contracts, run `build/update-sdk.ps1` to refresh the Unity copy of its DLL along with the SDK.
