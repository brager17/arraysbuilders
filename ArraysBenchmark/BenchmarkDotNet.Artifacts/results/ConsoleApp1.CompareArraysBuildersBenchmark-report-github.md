``` ini

BenchmarkDotNet=v0.12.0, OS=macOS 10.15.2 (19C57) [Darwin 19.2.0]
Intel Core i7-9750H CPU 2.60GHz, 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=3.0.100
  [Host]     : .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), X64 RyuJIT
  DefaultJob : .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), X64 RyuJIT


```
| Method | _count |     Mean |    Error |   StdDev |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|------- |------- |---------:|---------:|---------:|-------:|------:|------:|----------:|
| AsList |   1000 | 17.41 us | 0.101 us | 0.090 us | 5.2490 |     - |     - |  32.23 KB |
