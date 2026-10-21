```

BenchmarkDotNet v0.15.8, Windows 10 (10.0.19045.6456/22H2/2022Update)
11th Gen Intel Core i9-11900 2.50GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.101
  [Host]     : .NET 8.0.11 (8.0.11, 8.0.1124.51707), X64 RyuJIT x86-64-v4
  Job-FGEKWY : .NET 8.0.11 (8.0.11, 8.0.1124.51707), X64 RyuJIT x86-64-v4

IterationCount=3  LaunchCount=1  WarmupCount=1

```
| Method              | Count  | Mean     | Error     | StdDev   | Ratio | RatioSD | Gen0      | Gen1      | Gen2      | Allocated | Alloc Ratio |
|-------------------- |------- |---------:|----------:|---------:|------:|--------:|----------:|----------:|----------:|----------:|------------:|
| Gradual256ToTarget  | 100000 | 26.17 ms | 12.395 ms | 0.679 ms |  1.00 |    0.03 | 2125.0000 | 2125.0000 | 1875.0000 |  71.01 MB |        1.00 |
| Reserved256ToTarget | 100000 | 20.48 ms |  9.454 ms | 0.518 ms |  0.78 |    0.02 |  656.2500 |  656.2500 |  656.2500 |  26.17 MB |        0.37 |
| ConstructedAtTarget | 100000 | 19.29 ms |  3.878 ms | 0.213 ms |  0.74 |    0.02 |  656.2500 |  656.2500 |  656.2500 |   26.1 MB |        0.37 |
