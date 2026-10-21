# Visibility planning experiment

This experiment asks a selector to choose an exact visibility search plan. The selector never decides whether an entity is visible; every plan applies the same distance predicate and is checked against brute force.

## Exact plans

- PerAnchor: one radius query per owned entity, matching the current Galaxy visibility shape.
- CoverSphere: one covering sphere followed by exact filtering against every owned sphere.
- Cluster4 and Cluster16: dominant-axis spatial bins, a covering query per non-empty bin, exact filtering, and ID de-duplication.
- FullScan: direct brute-force scan of the world.

The measured path includes planning, spatial queries, candidate filtering, and de-duplication. Replay additionally puts feature extraction and selector inference inside the same timer. Persistent index construction is excluded.

## Dataset

The checked-in generator varies 1k to 100k world entities, 1 to 256 owned entities, five world distributions, five owner layouts, and radius profiles from 10 to 2000 metres plus mixed radii. The 200-scenario run contains 5000 timed strategy executions. Every execution matched the brute-force visible ID set and allocated zero bytes after warm-up.

Fastest-plan distribution after spatial clustering: PerAnchor 73, CoverSphere 57, Cluster4 25, Cluster16 40, FullScan 5. This confirms that a selector has a real job; no static plan dominates.

## Held-out results

Train/validation/test are separated by a deterministic hash of scenario ID. Selector accuracy means choosing the fastest plan, not visibility correctness. Visibility correctness is always 100 percent.

| Selector | Fastest-plan accuracy | Estimated E2E speedup | Actual replay speedup |
|---|---:|---:|---:|
| Current PerAnchor | 48.9% | 1.00x | 1.00x |
| Hand heuristic | 44.4% | 0.38x | not replayed |
| Decision tree depth 5 | 66.7% | 2.00x | 1.71x |
| Linear cost model | 51.1% | 1.54x | not replayed |
| MLP 6 | 64.4% | 1.96x | 1.82x |
| MLP 10 | 60.0% | 1.54x | 1.44x |
| MLP 12 | 66.7% | 1.96x | 1.77x |
Accuracy alone is insufficient: the selected MLP-6 has slightly lower fastest-plan accuracy than MLP-12, but wins the measured replay because its mistakes are cheaper and inference is smaller. MLP-6 is active in production with exact-result filtering and a hard fallback that rejects FullScan when `worldCount * ownerCount > 1,000,000`. Production traces remain the next source for retraining.

## Seed sweep and recurrence

The single-seed MLP-10 anomaly was not architectural. A five-seed sweep showed good and bad local minima for every width. On the balanced static split all five MLP-10 runs accelerated the aggregate workload; their estimated speedups ranged from 1.49x to 1.60x. MLP-6 remained the most attractive width because its inference was cheaper and its validation-selected full replay reached 1.75x in that run. Absolute replay ratios remain noisy on the loaded workstation.

A separate dataset contains 60 independent sequences of 24 ticks: coherent owner motion, gradual radius-regime changes, and occasional teleports. It has 21,600 measured rows. Sequence IDs, never individual ticks, are assigned to train, validation, or test, preventing temporal leakage.

The Elman RNN uses full 24-step backpropagation through time and keeps its hidden state per search instance. It did not beat the feed-forward model. On the 312 held-out ticks, validation-selected results were:

| Width | Feed-forward accuracy / mean regret | RNN accuracy / mean regret | FF/RNN step cost |
|---|---:|---:|---:|
| 6 | 69.2% / 1.216 | 59.0% / 1.315 | about 102 / 113 ns |
| 10 | 65.1% / 1.236 | 63.8% / 1.303 | about 177 / 216 ns |
| 12 | 67.9% / 1.257 | 65.7% / 1.304 | about 205 / 273 ns |

Some individual RNN seeds reached higher classification accuracy, but their expensive mistakes produced worse regret. With the current twelve complete snapshot features, recurrence adds state and cost without useful hidden information. A future RNN test is justified only after adding causal runtime telemetry such as previous selected plan, candidate count, exact-test count, and elapsed-time bucket.

The selected static MLP-6 (model version 2, seed 9127) is embedded in `GalaxyCoreServer.dll` as `VisibilityDirectorMlp`. It contains 137
float values (normalizer, biases, and weights), or 548 raw weight bytes. One internal static copy is
shared process-wide. `ModelVersion`, `FeatureSchemaVersion`, the training-set tag, and a SHA-256
fingerprint travel with the weights. The earlier spatial-backend RNN follows the same metadata
contract in `SpatialDirectorRnn` and was already compiled into the same DLL.


This is a preserved research note from the GalaxyCore source workspace. The SDK distribution contains the compiled runtime models; the original training harness is not included here.
