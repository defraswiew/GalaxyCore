# Jitter2 vendored source

- Upstream: https://github.com/notgiven688/jitterphysics2
- Version: `2.8.9`
- Commit: `c15bc6abfdda90a936975979a42f7a54a211084e`
- License: MIT, see `LICENSE`

The upstream C# sources are compiled directly into `GalaxyCore.Physics.Jitter2.dll`.
They are intentionally kept inside the optional server adapter: GalaxyCoreServer,
GalaxyCoreCommon and the Unity client do not compile or reference Jitter2.

To upgrade the engine, replace this directory from a reviewed upstream tag, retain
the license and third-party notices, then run the complete adapter conformance suite.
