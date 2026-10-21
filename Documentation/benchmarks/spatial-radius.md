```

BenchmarkDotNet v0.15.8, Windows 10 (10.0.19045.6456/22H2/2022Update)
11th Gen Intel Core i9-11900 2.50GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.101
  [Host]   : .NET 8.0.11 (8.0.11, 8.0.1124.51707), X64 RyuJIT x86-64-v4
  ShortRun : .NET 8.0.11 (8.0.11, 8.0.1124.51707), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1
WarmupCount=3

```
| Method            | Distribution | Mean     | Error     | StdDev    | Ratio | RatioSD | Gen0     | Gen1    | Allocated | Alloc Ratio |
|------------------ |------------- |---------:|----------:|----------:|------:|--------:|---------:|--------:|----------:|------------:|
| **LegacyPointOctree** | **Uniform**      | **1.576 ms** | **2.2456 ms** | **0.1231 ms** |  **1.00** |    **0.09** |  **91.7969** |       **-** |  **783664 B** |        **1.00** |
| GridX2            | Uniform      | 1.769 ms | 0.6811 ms | 0.0373 ms |  1.13 |    0.08 |        - |       - |         - |        0.00 |
| GridX4            | Uniform      | 4.396 ms | 3.8469 ms | 0.2109 ms |  2.80 |    0.22 |        - |       - |         - |        0.00 |
|                   |              |          |           |           |       |         |          |         |           |             |
| **LegacyPointOctree** | **Clustered**    | **6.482 ms** | **1.0737 ms** | **0.0589 ms** |  **1.00** |    **0.01** | **750.0000** | **62.5000** | **6501856 B** |        **1.00** |
| GridX2            | Clustered    | 5.213 ms | 0.4088 ms | 0.0224 ms |  0.80 |    0.01 |        - |       - |         - |        0.00 |
| GridX4            | Clustered    | 7.280 ms | 3.0097 ms | 0.1650 ms |  1.12 |    0.02 |        - |       - |         - |        0.00 |
