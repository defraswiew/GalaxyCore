# GalaxyCore.Physics.Jitter2

Optional server-side physics for Galaxy Network. The core server does not reference this
assembly, so a server which does not install/register it creates no physics engine objects.

Jitter2 2.8.9 source is vendored and compiled directly into this adapter assembly. Consumers
need only `GalaxyCore.Physics.Jitter2.dll`; there is no Jitter2 NuGet/runtime dependency.

Register the backend before starting GalaxyCore:

```csharp
PhysicsBackendId jitter2 = GalaxyPhysics.RegisterBackend(
    "jitter2",
    Jitter2PhysicsBackend.Create());

GalaxyCore.Start(config);
```

Select physics independently for each `Instance`:

```csharp
public override void Start()
{
    Physics.Start(jitter2, PhysicsWorldMode.InstanceThread);
}
```

Available modes:

- `InstanceThread`: the cheapest mode; Jitter steps inside the room frame.
- `DedicatedThread`: one stable coordinator thread per physical room, single-threaded solver.
- `Parallel`: one stable coordinator thread and Jitter's process-wide solver workers.

Create a body after the entity has been initialized:

```csharp
entity.Physics.Create(new PhysicsBodyOptions
{
    MotionType = PhysicsBodyMotionType.Dynamic,
    Shape = PhysicsShape.Sphere(0.5f),
    Mass = 1f
});

entity.Physics.ApplyImpulse(new GalaxyVector3(0f, 5f, 0f));
```

Collision transitions are marshalled back to the owning Instance thread:

```csharp
entity.Physics.CollisionEntered += collision =>
    Console.WriteLine($"Contact with body {collision.OtherBodyId}");
```

Use `Physics.TryRaycast(...)` for allocation-free queries in `InstanceThread`. Dedicated and
parallel worlds expose `RaycastAsync(...)`; use `RaycastBatchAsync(...)` for groups of AI,
weapon, suspension, or ground-check rays so the group crosses the thread boundary as one command
and uses one Task instead of one per ray. Never block the Instance thread with `.Wait()`/`.Result`.

Dynamic body snapshots update the server `NetEntity.transform`, so the normal Galaxy transform
frame transports the result to clients. Kinematic bodies work in the opposite direction: change
the entity transform and Galaxy forwards the changed pose to Jitter at the next physics boundary.

The adapter requires .NET 8 or newer and a physics tick rate of at least 60 Hz. Its parallel
worker pool is process-wide; multiple `Parallel` worlds may therefore serialize inside Jitter.
