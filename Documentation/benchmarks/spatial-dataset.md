# Spatial strategy dataset

The collector lives in the BenchmarkDotNet executable but uses a paired in-process runner so every
backend receives exactly the same generated world, moves, sphere queries and rays.

```powershell
dotnet run --project .\GalaxyCore.SpatialIndex.Benchmarks\GalaxyCore.SpatialIndex.Benchmarks.csproj -c Release -- SpatialDatasetCollector 100 output.csv
```

Each scenario emits one row per compatible backend. Current labels include separate build, update,
sphere-query and ray nanoseconds, managed allocation, results, visited nodes or cells, candidates,
reinsertion count, changed grid levels, tree height and selected-level occupancy statistics.

The old PointOctree is retained as a comparison label only for point objects and at most 1000 moves,
because its recursive remove makes larger update batches unsuitable for mass dataset generation.

## Training split and metric

- split by whole seeds and distribution families, never by individual backend rows;
- keep temporal sequences intact in one split;
- compare lookup or linear baselines, small trees and a tiny recurrent model;
- optimize mean and p95 regret relative to the measured oracle, not classification accuracy;
- include rebuild cost and memory policy after predicted per-backend runtime cost.

The independent-scenario collector records three replicates with a different deterministic backend
order. `SpatialSequenceDatasetCollector` additionally keeps all candidate indices alive while a
world moves through calm, coherent-motion, teleport, radius-burst and ray-burst phases. Backend
measurement order rotates each tick, and complete sequences (not rows) are assigned to train,
validation and test splits.

`SpatialModelTrainer` is a dependency-free C# trainer. It implements normalization, a ridge-linear
baseline, an eight-hidden-unit feed-forward ablation, and an eight-hidden-unit Elman RNN trained by
full BPTT and Adam. The model predicts log runtime for each backend; selection is evaluated by
runtime regret against the measured per-tick oracle.

The first 80-sequence/40-tick experiment produced 12,800 backend rows. On 600 held-out ticks the
linear baseline reached 64.5% accuracy and 9.96% mean regret; the feed-forward network reached
71.7% and 7.46%; the recurrent network reached 67.8% and 7.67%. Recurrence therefore is not yet
justified by this feature set, although it reduced the worst observed regret slightly. Its hot
inference cost was about 153 ns per tick on the collection machine. These are preliminary results:
the machine was under load, and a larger clean dataset plus cheap occupancy telemetry is required
before freezing a runtime policy.

## Hidden-width, pruning and quantization experiment

Five deterministic initializations were trained for every width on the same sequence-level split.
The held-out mean results were:

| Model | Hidden | Accuracy | Mean regret | P95 regret | Hot inference |
| --- | ---: | ---: | ---: | ---: | ---: |
| Feed-forward | 6 | 65.87% | 9.09% | 46.31% | not measured |
| Elman RNN | 6 | 65.97% | 8.10% | 43.78% | 100 ns |
| Feed-forward | 10 | 70.27% | 7.32% | 44.98% | not measured |
| Elman RNN | 10 | 69.60% | 7.24% | 43.80% | 197 ns |
| Feed-forward | 12 | 69.77% | 7.51% | 44.58% | not measured |
| Elman RNN | 12 | 69.23% | 7.09% | 40.99% | 301 ns |

RNN-12 has the best regret tail, while RNN-10 is the better quality/cost compromise. Plain
post-training magnitude pruning of RNN-12 was destructive: pruning 25%, 50% and 75% of matrix
weights increased mean regret to 10.70%, 27.99% and 57.48%. A future pruning experiment should
fine-tune while keeping the zero mask fixed.

Symmetric per-matrix int8 quantization retained nearly all quality (7.17% mean regret), but scalar
C# inference slowed from about 301 ns to 330 ns. It reduces storage substantially, but is not a
runtime optimization without a packed SIMD kernel. Timing was collected on a loaded machine and
is useful only for relative comparisons.

Converting the trained models to `float` did not change a single held-out backend decision. It
reduced RNN-10 inference from about 197 ns to 185 ns and RNN-12 from about 292 ns to 273 ns in the
repeat run.

Freezing a magnitude mask and fine-tuning for 100 additional epochs changed the pruning result.
At 50% matrix sparsity RNN-12 improved to 69.77% accuracy and 6.81% mean regret (P95 42.26%). A
compact row-oriented sparse-float inference preserved those decisions and ran in about 244 ns,
faster than both dense-float and dense-double. This is the current best experimental model, though
the clean production dataset may select a different width or sparsity.


This is a preserved research note from the GalaxyCore source workspace. The SDK distribution contains the compiled runtime models; the original training harness is not included here.
