``` ini

BenchmarkDotNet=v0.12.0, OS=macOS 10.15.2 (19C57) [Darwin 19.2.0]
Intel Core i7-9750H CPU 2.60GHz, 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=3.0.100
  [Host]     : .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), X64 RyuJIT
  DefaultJob : .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), X64 RyuJIT


```
|             Method | _count |     Mean |    Error |   StdDev |    Gen 0 |   Gen 1 |   Gen 2 | Allocated |
|------------------- |------- |---------:|---------:|---------:|---------:|--------:|--------:|----------:|
|      ToListClasses |  10000 | 744.2 us |  9.01 us |  8.43 us | 117.1875 | 78.1250 | 39.0625 | 725.11 KB |
|  BuildArrayClasses |  10000 | 762.3 us | 14.09 us | 13.18 us | 117.1875 | 78.1250 | 39.0625 | 725.12 KB |
| BuildMyListClasses |  10000 | 761.0 us | 10.51 us |  9.83 us | 117.1875 | 78.1250 | 39.0625 | 725.12 KB |
