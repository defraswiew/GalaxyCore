# SerializationBenchmarks — preserved summary

Source: `GalaxyCore.Serialization.Benchmarks.SerializationBenchmarks-20260807-235856.log`.

The block below is copied from the saved BenchmarkDotNet output. The report compares codec APIs on a synthetic snapshot; see [methodology](../../PERFORMANCE.md).

// * Summary *

BenchmarkDotNet v0.15.8, Windows 10 (10.0.19045.6456/22H2/2022Update)
11th Gen Intel Core i9-11900 2.50GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 7.0.306
  [Host]     : .NET 7.0.9 (7.0.9, 7.0.923.32018), X64 RyuJIT x86-64-v3
  Job-HHSXOG : .NET 7.0.9 (7.0.9, 7.0.923.32018), X64 RyuJIT x86-64-v3

IterationCount=12  WarmupCount=5

| Method                    | Categories  | Mean        | Error     | StdDev    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|-------------------------- |------------ |------------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| NetBufferColdDecode       | Cold decode |    53.18 ns |  1.862 ns |  1.454 ns |  0.71 |    0.15 | 0.0124 |     104 B |        1.44 |
| GalaxyBitBufferColdDecode | Cold decode |    76.56 ns | 13.698 ns | 10.694 ns |  1.03 |    0.25 | 0.0086 |      72 B |        1.00 |
| ProtobufNetColdDecode     | Cold decode | 1,045.13 ns | 21.552 ns | 14.255 ns | 14.03 |    2.87 | 0.0935 |     792 B |       11.00 |
|                           |             |             |           |           |       |         |        |           |             |
| GalaxyBitBufferColdEncode | Cold encode |    61.58 ns |  2.745 ns |  2.143 ns |  1.00 |    0.05 | 0.0334 |     280 B |        1.00 |
| NetBufferColdEncode       | Cold encode |   202.46 ns |  5.107 ns |  3.987 ns |  3.29 |    0.13 | 0.0458 |     384 B |        1.37 |
| ProtobufNetColdEncode     | Cold encode |   785.93 ns | 40.170 ns | 31.362 ns | 12.78 |    0.65 | 0.1354 |    1136 B |        4.06 |
|                           |             |             |           |           |       |         |        |           |             |
| GalaxyBitBufferDecode     | Decode      |    33.16 ns |  0.337 ns |  0.201 ns |  1.00 |    0.01 |      - |         - |          NA |
| NetBufferDecode           | Decode      |    42.91 ns |  1.467 ns |  1.061 ns |  1.29 |    0.03 |      - |         - |          NA |
| ProtobufNetDecode         | Decode      | 1,065.85 ns | 19.192 ns | 14.984 ns | 32.15 |    0.47 | 0.0858 |     728 B |          NA |
|                           |             |             |           |           |       |         |        |           |             |
| GalaxyBitBufferEncode     | Encode      |    40.34 ns |  1.540 ns |  1.203 ns |  1.00 |    0.04 |      - |         - |          NA |
| NetBufferEncode           | Encode      |   144.55 ns |  3.609 ns |  2.818 ns |  3.59 |    0.12 |      - |         - |          NA |
| ProtobufNetEncode         | Encode      |   736.84 ns | 22.599 ns | 14.948 ns | 18.28 |    0.63 | 0.0944 |     792 B |          NA |
