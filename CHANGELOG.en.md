# GalaxyCore — major update

[Русский](CHANGELOG.md) · [English](CHANGELOG.en.md)

GalaxyCore now has its own serialization system, expanded game-state synchronization, RPC, server-side physics with interchangeable backends, and a server administration application. Network frames, connection lifecycles and large-world processing have been reworked, with new replay recording and connection diagnostics tools. Compact AI models embedded in the server select spatial algorithms at runtime.

These patch notes bring together the accumulated changes: what is new, which tasks are now easier, and what to consider when upgrading an existing project.

## Updated public distribution

The public repository now includes current DLLs, adapters and examples with matching client and server components.

- **Unity 6.3 LTS (`6000.3.24f1`).** Updated the project, dependencies, scenes, UI and gameplay examples. A separate AOT SDK distribution is provided for Mono and IL2CPP.
- **.NET 8 server.** Shared messages and client libraries use .NET Standard 2.1; a separate managed distribution is provided for standard .NET clients.
- **Current networking APIs.** Reworked connection and disconnection, event subscriptions, authentication and shared message contracts. The Unity factory preserves the original incoming SDK entity, its owner and its initial GalaxyVars.
- **Fixed entering Add Force.** The temporary factory object is no longer destroyed during scene changes. Spawned game objects remain in their own scene and are removed on exit; entering again no longer leaves old spheres behind.
- **Consistent variable permissions.** In the GalaxyVars example, the server sends the name and time, and the client receives them. The initial name is now included in the state snapshot.
- **Physics examples use Jitter2 and GPHB.** Scene geometry is exported to the new format, and the server loads the prepared bake.
- **Portable navigation.** Maps have been rebaked from scenes, and references to developer drives have been replaced with data inside the project. The large authoring bake is stored in a local cache and generated when needed.
- **Removed unused resources.** Removed old DLLs, unused assets and UI, experimental scaffolding, duplicate backups and IDE files. Required resources were moved out of temporary folders while preserving their GUIDs.
- **Repeatable distribution checks.** Added SDK updates from source, SHA-256 verification, Unity asset audits, Player builds and network smoke tests. Mono and IL2CPP were checked on Windows x64: authentication, rooms, world sync, GalaxyVars and two Add Force entry/exit cycles.

The UNIGINE adapter has been updated for the current Galaxy API and .NET 8. Full engine validation requires a compatible UNIGINE SDK and migration of the original assets in the 2.15.1 format; running the actual engine was outside this validation pass.

[Running the examples](README.md) · [Distribution contents and validation results](Documentation/Validation.md)

## Performance: before / after

The main gains are in frequently executed operations: writing network data, updating spatial structures and calculating visibility. The results below come from saved measurements on an Intel Core i9-11900.

| Scenario | Before | After | Result |
|---|---|---|---|
| Writing a game snapshot into a reusable buffer: protobuf-net → hand-written Galaxy bit format | 736.84 ns | 40.34 ns | **≈18.3× faster**, managed allocations 792 → 0 bytes |
| Reading a snapshot with object creation: protobuf-net → attribute-based GalaxySerializer | 1049.4 ns | 660.5 ns | **≈1.59× faster**, allocations 728 → 432 bytes |
| Visibility calculation in a test replay: fixed PerAnchor → MLP plan selection | Baseline time — 100% | ≈55% of baseline time | **1.82×** on the measured pass, including feature preparation and model execution |
| 256 radius queries over 100,000 clustered objects: octree → multilevel grid | 6.482 ms | 5.213 ms | **≈20% less time**, allocations 6,501,856 → 0 bytes per query batch |
| Building a grid for 100,000 objects: incremental growth → capacity reservation | 26.17 ms | 20.48 ms | **≈22% less time**, managed allocations 71.01 → 26.17 MB (**−63%**) |

**The difference is even greater for bulk position updates.** In preliminary smoke runs, a batch of 1000 moves in a world containing 100,000 objects took approximately **0.93–1.02 seconds** with the previous PointOctree and **1.40–4.64 milliseconds** with the new Hybrid. This is a change from almost a second to a few milliseconds in this specific scenario; these values are retained as preliminary ranges.

The figures apply to the listed operations and come from different test runs. A hand-written codec, attribute-based serialization and a complete game server have different costs. Spatial gains depend on movement, object distribution and queries, which is why the server selects strategies at runtime.

For details, see [comparison conditions, original reports and measurement scope](PERFORMANCE.md).

### Less data on the wire

Optional quantization reduces the size of individual values:

| Value | Full representation | Compact representation | Savings |
|---|---:|---:|---:|
| Quaternion rotation | 16 bytes | 4 bytes | **75%** |
| Normalized Vector3 direction | 12 bytes | 4 bytes | **≈67%** |
| Float | 4 bytes | 2 bytes | **50%** |

These are the sizes of the values themselves, excluding packet headers. Quantization is explicitly enabled and allows a loss of precision; the compact direction representation preserves the vector's orientation without its length.

## New features

### Serialization and network messages

Added **GalaxySerializer**, a dedicated game-packet serialization system with attribute-based contracts. Message structures can be defined in C#, while direct bit reading and writing are available for code with specific format requirements.

- The `GalaxySerializable` and `GalaxyField` attributes define fields and their order in the network contract.
- The `GalaxyMessage` base class simplifies creating messages for the existing send API.
- `GalaxyBitWriter` and `GalaxyBitReader` provide access to individual bits, primitives and custom packets.
- Added overloads for writing into a supplied buffer and reading into an existing object, allowing memory reuse in frequently executed code.
- Supported types include primitives, enums, strings, byte arrays, nullable values, nested objects, one-dimensional arrays, `List<T>` and Galaxy math types.
- Custom codecs support application-specific types and data formats.
- Compression options include half-float, normalized directions and compact rotations. Direction compression preserves a vector's orientation but not its original length.
- Reading validates lengths, nesting depth and collection limits; `TryDeserialize` methods allow callers to handle malformed data without exceptions.
- Execution paths are provided for JIT and AOT, including Unity IL2CPP.

Transform frames, network-entity messages and GalaxyVars use specialized compact formats. Packing accounts for protocol overhead and splits accumulated messages across packets at item boundaries.

### GalaxyVars: entity and instance state

The network-variable system has been expanded for composed gameplay models. It can synchronize character and vehicle state, object components and the game room itself.

- **Scopes** let independent components use their own field-ID namespaces.
- Synchronization directions and ownership checks determine who can send and apply changes.
- Added scope-aware change events, value reading and forced synchronization.
- `GalaxyVarArray<T>` tracks collection changes, including insertion, removal, resizing and element replacement. When changed, the array is transmitted as a complete snapshot.
- Added support for more math types and compact directions/rotations.
- Variable updates for entities and the current instance are combined into network frames.
- Full synchronization sends initial values, while a repeated `SyncWorld` updates GalaxyVars on existing client entities.
- The server captures final values after game logic, delayed calls and the physics step. A change late in a frame can reach the client in that same frame.

An optional **`ClientInstance`** model has been added for client-side room logic, with a factory selected by instance type, its own GalaxyVars, and creation/exit events. It can be adopted gradually while continuing to use `InstanceInfo` and standard events.

### Transform hierarchy and server-controlled movement

`GalaxyTransform` now has a complete local/world coordinate model. Related network objects can form hierarchies, such as a passenger inside a vehicle, a weapon attached to a character, or equipment on a moving platform.

- Supports parent relationships and local/world position, rotation and scale.
- Added transformation matrices, operations on points, vectors and directions, and reparenting that preserves the world pose.
- Network synchronization transmits the parent and the child's local state.
- The client defers a relationship if the parent has not yet been created, then applies topology and transform consistently.
- Validates cyclic relationships, self-parenting and membership in the same game session.
- When a parent is removed, the child is detached while preserving its world pose.
- Hierarchy changes reset previous interpolation so that objects are not smoothed between different coordinate systems.

The **side controlling the transform** can now be selected. Server control suits server simulations and objects whose movement is driven by server game logic. The standard mode, controlled by the owning client, is also retained.

### RPC and network time

Added **network-entity RPC**: gameplay actions can be defined as methods and invoked through `entity.Rpc` with typed arguments.

- Handlers are registered through `GalaxyRpc`; additional components use scopes and stable network names.
- Calls can be sent immediately or included in a network frame.
- The server can target the owner or a selected client; calls included in frames use the shared broadcast.
- Access policies specify the allowed source: the owner, any client or the server.
- Call context includes the sender, network time, estimated RTT, uncertainty and delay before execution.
- Validates signatures, call direction and ownership; the server limits how much it trusts a supplied timestamp.

The new **network-time** system uses monotonic clocks and estimates the server offset, RTT and uncertainty. It provides a time base for RPC and network-state processing that does not depend on adjustments to the system clock.

### Server-side physics with interchangeable backends

Physics has been moved into a separate pluggable layer. The application registers a backend, and each instance decides whether it needs a physics world and how to update it.

**Three execution modes are available:**

| Mode | Purpose |
|---|---|
| `InstanceThread` | The physics step runs on the instance thread. Suitable for small worlds and synchronous queries. |
| `DedicatedThread` | The world has a dedicated coordinator thread and processes commands independently of the game frame. |
| `Parallel` | A dedicated coordinator uses the selected backend's parallel solver capabilities. |

Added an optional **Jitter2** adapter. Its engine source is compiled into the adapter assembly, so no separate engine library is required at runtime.

- Dynamic, kinematic and static bodies; forces, impulses and movement control.
- Primitives, cylinders, convex hulls, triangle meshes, height fields and compound colliders with local poses.
- Collision layers and masks, triggers, and standalone trigger objects that do not require a network entity.
- Contact enter/exit events deliver available collision data to the instance thread.
- Fixed, distance, hinge, slider and spring joints with limits and connection parameters.
- Synchronous raycasts in instance-thread mode, and asynchronous queries and batched raycasts for dedicated physics threads.
- Coordinated world shutdown, sleep and wake-up alongside the instance. A backend error puts the affected world into a failed state while other rooms continue running.
- Protection against applying delayed commands to a new object that has received a reused ID.

Dynamic bodies publish results to the network transform. Kinematic bodies receive their pose from game code at the physics-step boundary.

Added the **GPHB** format for prepared geometry: materials, shapes, compounds and geometry blocks, with integrity and limit checks during reading. The application determines how to create its world objects from these data.

### Spatial queries and visibility

The server's spatial-indexing layer has been reworked for worlds with large object counts.

- Added dynamic, multilevel, hybrid and adaptive indexes.
- Supports entity spatial radii, distance searches, raycasts against points/spheres and index capacity reservation.
- Client visibility queries are processed in batches.
- Adaptive index and query-plan selection account for workload characteristics; results pass through exact geometric filtering.
- Added `ExcludeSyncWorld` for objects the application wants to omit from full world synchronization.

A correctness and performance harness for spatial structures was added during core development, covering index construction, object movement, radius queries, raycasts and scenarios with one hundred thousand objects. It helps compare indexes under different workloads; saved results are included in the public distribution's documentation.

### Embedded AI for runtime optimization

Two trained neural-network models are integrated into the server assembly. They run locally and select algorithms based on the world's current workload.

- **The RNN selects the spatial index.** It considers object counts, movement, search radii and raycast frequency, choosing between tree, grid and hybrid variants.
- **The MLP selects the visibility calculation plan.** Depending on the layout of client-owned objects, it selects individual queries, an enclosing sphere, cluster grouping or a full scan of a small set.
- **Gameplay rules remain exact.** The selected plan performs exact distance checks and deduplication. Across 5000 test executions of the strategies, results matched a full scan.
- **Switching is constrained.** The RNN requires a sustained recommendation and a sufficient predicted gain; large indexes migrate in batches at frame boundaries. Expensive full scans have a hard limit on the number of checks.
- **Weights are embedded in the DLL and shared across the process.** RNN recurrent state is kept separately for each index so that histories from different worlds do not mix.

In a replay benchmark on a held-out test set, the MLP made visibility calculation **1.82 times faster** than a fixed plan of individual queries. The measurement includes strategy selection, queries, filtering and deduplication; index construction is excluded.

### Virtual connections and replays

The client API now supports **virtual connections without a UDP socket**. Messages, events and requests use the normal client processing path, while a local implementation supplies server behavior.

- Connection, approval, message exchange and disconnection can be simulated.
- Delivery delay can be configured in both directions.
- The provided `VirtualGalaxyServer` supplies a local model of rooms, entry/exit and requests for a single client.

The replay system now supports **server-side instance traffic recording**. An invisible observer receives broadcasts for recording without occupying a player slot or participating in room-owner selection.

- Recordings are stored in ZIP packages containing a message stream, metadata and additional files.
- Applications can attach text and binary data.
- Playback supports pausing, speed changes, restarting and state resets.
- Recording duration, current time and the checkpoint list are available.
- Starting from a checkpoint applies a prepared full snapshot and continues playback with subsequent messages.
- Loading validates package structure, timestamps and checkpoint data.
- Connection-health markers are excluded from recordings, and recorded time-synchronization responses do not interfere with a live connection.

The application creates checkpoints. Playback can seek to the start of a recording or an existing checkpoint; restoring application-specific game objects and data remains part of the project's integration.

### Connection diagnostics

Added **`GalaxyConnectionAnalyzer`**, a separate, brief diagnostic session before game authentication or at the request of support.

- Checks server reachability and connection establishment.
- Measures RTT, loss, retransmissions and data integrity.
- Finds a verified UDP payload size that does not require fragmentation.
- Produces a final report with quality metrics, failure reasons and diagnostic completion data.
- Limits duration, traffic volume and concurrent server sessions.
- Authenticates diagnostic requests with replay protection.
- Corrects diagnostic request time when client and server clocks differ: a signed response permits one connection retry with a new nonce.

The verified UDP payload size helps select network settings but is not the full IP MTU. Diagnostics are enabled separately and generate probe traffic during analysis.

### Connection health during gameplay

Added an optional **instance traffic-delay detector**. It tracks the progress of reliable markers and acknowledgements during a session, including periods with no game-state changes.

- Detects missing incoming frames and missing acknowledgements of outgoing frames separately.
- An adaptive threshold accounts for frame rate, RTT and RTT variation.
- Recognizes late markers and bursts of delivery from an accumulated queue.
- Exposes current state, alarm/recovery events and details of completed episodes.
- The server produces a final report for each client's stay in an instance: state durations, RTT statistics, episodes and delivery anomalies.
- Client and server can enable their own assessment independently; client thresholds are configurable.
- Instance changes, re-entry and delayed messages are separated by session identifier.
- Room sleep pauses observation; waking restarts the assessment warm-up.
- A local stall of the client's update thread is accounted for separately, while preserving observation of actual incoming pauses and previously detected network issues.

These events can drive a connection indicator, a match report or application-specific handling. An alarm indicates a network anomaly; the game decides how to interpret and respond to it.

### GalaxyAdmin: server administration

Added an **administration HTTP API** and the **GalaxyAdmin Windows application** for monitoring a single server or a fleet.

**Monitoring and diagnostics**

- Server overview, process health, uptime and traffic statistics.
- Client and instance lists with access to details, network entities and their hierarchy.
- Metrics for network workers, game frames, physics, queues and errors.
- CPU, memory and runtime information with measurement availability and scope.
- Separate views of active issues and accumulated observations.
- Displays data-retrieval times and retains the last successful snapshot during temporary endpoint failures.
- Bounded request concurrency, retries with increasing intervals, and cancellation of unnecessary requests when the selected object changes.

**Operator actions**

- Disconnect clients; put instances to sleep, wake them or close them.
- Check write permissions and verify that the selected target is still current before execution.
- Protection against duplicate execution of a request, result receipts and a bounded audit log in server memory.
- Custom administrative methods with argument descriptions and a calling interface.

**Project data and server connections**

- Declarative Custom Data for servers, clients and instances: fields, tables, tags and search.
- Selected project fields are integrated into standard administration tables.
- Global search uses bounded indexes; detailed object data are loaded on demand.
- Persistent server profiles with credentials stored through Windows facilities.
- Import servers from JSON for the current session, with a preview and entry validation.
- The application's native layer handles network requests and secrets; the web interface receives results through a restricted bridge.
- The API describes available capabilities and operator permissions; an OpenAPI specification has been added.

### Math and game-engine integration

- Added `GalaxyPose`, `GalaxyRay`, `GalaxyPlane`, `GalaxyAabb` and their serialization.
- `GalaxyCoordinateBasis` converts coordinates and rotations between selected axis conventions.
- Expanded vector, quaternion, matrix and transform operations: safe normalization, reflection, length clamping, inverse transformations and pose operations.
- Added an engine-independent Animator state packet. The core validates packet format, ownership and size, while the game-engine adapter defines the contents and animation-parameter schema.

## Network and client behavior changes

- **Client entity messages can be sent within a frame.** `SendMessageInFrame` and `frameDrop` allow grouping messages and selecting their frequency; a forced send flushes the accumulated buffer without waiting for the next frame.
- **Initial synchronization of large worlds is split across messages.** Batch sizes and network buffers can be configured for the project.
- **Incoming server messages are distributed across workers.** Each connection is assigned to one worker, preserving its message order while different clients are processed in parallel.
- **Client network-frame frequency accounts for `FrameMultiplier`.** Fractional elapsed time is retained, and missed frames are not sent as a catch-up burst after a stall.
- **Async requests support timeouts and cancellation.** The default wait is limited to 30 seconds; disconnection completes pending requests.
- **Expanded connection statistics.** Packets, messages, traffic, retransmissions by cause, queue lengths and time since the last activity are available.
- **The default server MTU has been reduced to 1200 bytes.**
- **Client compatibility is checked on connection.** The server can reject an unsupported client before application authentication and provide rejection details to the application.
- **Instance sleep state can be read through `IsSleep`.** Transitions use lifecycle methods.
- **Chat post-processing can replace a message or cancel sending by returning `null`.** Server code can access channel membership.

## Fixes

### Connections and shutdown

- Fixed removal of a new client session caused by a delayed disconnect of its old transport.
- Timeout events are tied to the specific connection: a problem with one client no longer triggers callbacks for other clients sharing the peer.
- Fixed network-loop termination and message-waiting paths during application shutdown.
- Disconnection clears entity state, the current instance, time synchronization and pending requests.
- Exceptions in application disconnect and entity-destruction handlers no longer interrupt the remaining cleanup.
- Unity `OnDestroy` and `OnApplicationQuit` stop the backend.
- Completed network threads are unregistered, releasing references held by the internal collection.
- Fixed races during startup, shutdown, sleep and wake-up of the frame loop. Frame timing uses a monotonic clock; a sleep pause does not become a queue of catch-up updates.
- The instance watchdog accounts for the actual heartbeat, normal shutdown and room startup; repeated notifications are suppressed until recovery.

### Delivery and synchronization

- Fixed handling of stale, duplicate and invalid ACKs; acknowledgement queues are cleared when the connection resets.
- Added validation of malformed lengths, channel numbers and sequence numbers before accessing internal structures.
- Fixed multithreaded-queue errors and coordinated access to frame buffers and network variables.
- Sending uses the actual message length rather than the allocated array size.
- Fixed mixing of incoming and outgoing counters for newest entity messages.
- Removed accesses to missing data when assembling and reading network variables.
- After reconnection, new transform frames are no longer rejected because sequence numbers from the previous session were retained. This fixes a case where moving objects could freeze.
- Fixed client API readiness when the connection event is raised.
- Refined handling of initial state, parent relationships and updates to existing entities during `SyncWorld`.

### Gameplay logic, tools and math

- Leaving a chat removes the correct participant's subscription; repeated add/remove operations no longer create extra subscriptions or notifications.
- Closed chats release history, handlers, private members and disconnect subscriptions.
- Removing an old instance checks its identity and does not affect a new room with a reused ID.
- Moving an object within an octree preserves the correct item count.
- Fixed exact vector equality, interpolation of opposite directions and rotations, and degenerate `LookRotation` cases.
- `TimeConnected` returns the full connection duration in seconds.
- Restarting GalaxyAdmin isolates requests and resources from the previous run; audit-entry ordering has been corrected.
- Removed excessive debug messages from certain network paths.

## Removed and replaced

- Built-in physics based on the old Jitter engine and the previous `PhysWorld`, `PhysEntity` and collider/material wrappers have been replaced by the pluggable physics API.
- The old `PhysBake` has been replaced by the GPHB format.
- `BitGalaxy` is marked obsolete; `GalaxyBitWriter`, `GalaxyBitReader` and `GalaxySerializer` are available for new code.
- Reduced the low-level `NetQueue` API and removed batched reading through `NetPeer.ReadMessages`.
- The previous projection of arbitrary JSON in GalaxyAdmin has been replaced by the declarative Custom Data model.
- Removed the old `lic.galaxy` file loader.

## Before upgrading your project

This update changes network formats and several public APIs. For an existing integration, review the following:

| Area | What to check |
|---|---|
| Client, server and Common | Update them as a matching set. Formats have changed for frames, entity messages, GalaxyVars and initial synchronization. The legacy protobuf API remains available for individual messages. |
| Serialization and GalaxyVars | Match network IDs, scopes, fields and codecs on both sides. For reflection paths in IL2CPP, protect the members in use from stripping with Preserve/link.xml. |
| Transform | Check local/world coordinates and parent relationships. Server control is selected before entity initialization and operates independently of GalaxyVars permissions. |
| RPC | For commands requiring strict ordering, use immediate sending with `reliableOrdered`. Targeted `CallTo`/`CallOwner` calls are not intended for frame broadcasts. |
| Physics | Migrate old Phys types to the new API. Register the backend before starting the server, and start a world for instances that need it. The Jitter2 adapter requires .NET 8+ and a physics-step frequency of at least 60 Hz. |
| Physics queries | Do not block the instance thread with `.Wait()`/`.Result` while waiting for an asynchronous raycast. |
| Spatial index | The current implementation requires the float configuration. `OctreeThreshold` is retained as a legacy setting and no longer sets the previous octree-update threshold. |
| Replays | Timestamps and the recording container have changed. The loader expects a ZIP package; corrupted data and invalid checkpoints throw exceptions. |
| Async requests | Handle timeouts and cancellation. Specify an infinite wait explicitly if needed. |
| `CompressedQuaternion` | Normalize the input quaternion yourself: automatic normalization has been removed. |
| Low-level API | Adapt direct calls to removed NetQueue/NetPeer methods and the old `GalaxyVersion` constructor. `FrameMultiplier` is now a property; rebuild SDK consumers. |
| GalaxyAdmin | Update custom data to the declarative contract and use matching server and desktop-client versions. |

## Documentation and validation

Russian guides and English documentation cover client connection and lifecycle, world synchronization, GalaxyVars, serialization, RPC, network time, physics, diagnostics and the administration API. Practical Unity integration examples have been added.

Automated checks have been expanded to cover serialization and packet sizes, malformed data, multi-client broadcasts, reconnect and shutdown, transform hierarchies, RPC, replays, network latency, physics backends and the GalaxyAdmin contract. Separate benchmark scenarios are available for spatial indexes.

The expanded tests and benchmark-harness sources belong to the core development repository. The public distribution contains prebuilt SDKs, template source code, saved benchmark reports and distribution-validation commands. The GalaxyAdmin desktop application is distributed separately; its server-side HTTP API is documented here.

- [Galaxy documentation](Documentation/README.md)
- [Serialization](Documentation/Serialization.md)
- [Jitter2 physics setup](Core/Server/Physics/README.md)
- [GalaxyAdmin HTTP API](Documentation/GalaxyAdmin.md)
