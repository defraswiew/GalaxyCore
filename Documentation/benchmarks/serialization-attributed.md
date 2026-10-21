```

BenchmarkDotNet v0.15.8, Windows 10 (10.0.19045.6456/22H2/2022Update)
11th Gen Intel Core i9-11900 2.50GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 7.0.306
  [Host]     : .NET 7.0.9 (7.0.9, 7.0.923.32018), X64 RyuJIT x86-64-v3
  Job-HHSXOG : .NET 7.0.9 (7.0.9, 7.0.923.32018), X64 RyuJIT x86-64-v3

IterationCount=12  WarmupCount=5

```
| Method                 | Categories | Mean       | Error    | StdDev   | Gen0   | Allocated |
|----------------------- |----------- |-----------:|---------:|---------:|-------:|----------:|
| GalaxyAttributedDecode | Decode     |   660.5 ns | 20.54 ns | 14.85 ns | 0.0515 |     432 B |
| ProtobufNetDecode      | Decode     | 1,049.4 ns | 25.71 ns | 15.30 ns | 0.0858 |     728 B |
|                        |            |            |          |          |        |           |
| GalaxyAttributedEncode | Encode     |   716.4 ns | 20.52 ns | 14.84 ns | 0.0772 |     648 B |
| ProtobufNetEncode      | Encode     |   742.5 ns | 23.28 ns | 18.18 ns | 0.0944 |     792 B |
