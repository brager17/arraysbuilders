``` ini

BenchmarkDotNet=v0.12.0, OS=macOS 10.15.2 (19C57) [Darwin 19.2.0]
Intel Core i7-9750H CPU 2.60GHz, 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=3.0.100
  [Host]     : .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), X64 RyuJIT
  DefaultJob : .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), X64 RyuJIT


```
|               Method |      Mean |     Error |    StdDev |    Median |
|--------------------- |----------:|----------:|----------:|----------:|
|         WriteInField | 0.0018 ns | 0.0030 ns | 0.0027 ns | 0.0009 ns |
| WriteInLocalVariable | 0.0072 ns | 0.0038 ns | 0.0034 ns | 0.0065 ns |
