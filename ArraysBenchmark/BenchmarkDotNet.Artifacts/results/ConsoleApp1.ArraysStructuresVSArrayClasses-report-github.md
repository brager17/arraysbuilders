``` ini

BenchmarkDotNet=v0.12.0, OS=macOS 10.15.2 (19C57) [Darwin 19.2.0]
Intel Core i7-9750H CPU 2.60GHz, 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=3.0.100
  [Host]     : .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), X64 RyuJIT
  DefaultJob : .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), X64 RyuJIT


```
|          Method |      Mean |     Error |    StdDev |
|---------------- |----------:|----------:|----------:|
| ArrayStructures | 16.330 ms | 0.1428 ms | 0.1335 ms |
|    ArrayClasses |  4.130 ms | 0.0637 ms | 0.0596 ms |
