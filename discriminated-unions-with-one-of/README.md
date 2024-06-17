# Discriminated Unions With OneOf

This example is a demo of discriminated unions using the [OneOf](https://github.com/mcintyre321/OneOf) Nuget package.

The project setup is a dotnet minimal API with two endpoints. They both have been implemented with the same behavior, but achieve it in different ways: 

- `/weatherforecast`:
  - On Success: returns the same type of dependency service.
  - On Failure: throws a `Exception` if a location is not supported and relies on setup of a exception handler `app.UseExceptionHandler` to map the appropriate bad request response.
- `/weatherforecast-usingoneof`
  - On Success: on result of OneOf maps to ` Results.Ok`  
  - On Failure: on result maps to `ProblemDetails` with `Results.BadRequest`.


# Benchmarks

A benchmark project has been added to illustrate the impact using discriminated unions vs exception handling middleware.

To run from the root of this folder, run the following command:

```powershell
dotnet run --project .\DiscriminatedUnionsWithOneOf.Benchmarks\DiscriminatedUnionsWithOneOf.Benchmarks.csproj -c Release
```

A previous result from my own laptop resulted with the following:

```
BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3593/23H2/2023Update/SunValley3)
AMD Ryzen 9 5900HS with Radeon Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK 8.0.106
  [Host]     : .NET 8.0.6 (8.0.624.26715), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.6 (8.0.624.26715), X64 RyuJIT AVX2


```
| Method                       | Mean      | Error    | StdDev   | Min       | Max       | Median    | Gen0   | Gen1   | Allocated |
|----------------------------- |----------:|---------:|---------:|----------:|----------:|----------:|-------:|-------:|----------:|
| GetWeatherForecast           | 702.41 μs | 8.063 μs | 7.148 μs | 693.46 μs | 721.50 μs | 701.21 μs | 4.3945 | 0.9766 |  38.79 KB |
| GetWeatherForecastUsingOneOf |  34.91 μs | 1.260 μs | 3.714 μs |  26.83 μs |  41.74 μs |  34.92 μs | 1.4648 | 0.4883 |  13.14 KB |

This highlights the significant impact of using exception handling middleware and should be avoided if possible if performance is a concern. OneOf provides a alternative approach. At time of this writing there has been more inquiries of having C# Discriminated Unions, which has shows signs of this being introduced, but there has been no official announcement of this feature coming yet.