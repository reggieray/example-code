using BenchmarkDotNet.Attributes;
using System.Net;

namespace DiscriminatedUnionsWithOneOf.Benchmarks
{
    [MemoryDiagnoser, MinColumn, MaxColumn, MeanColumn, MedianColumn]
    public class ErrorHandlingBenchmark
    {
        private readonly WeatherApi weatherApi = new();

        [Benchmark]
        public async Task<bool> GetWeatherForecast()
        {
            var api = weatherApi.CreateClient();
            var response = await api.GetAsync("weatherforecast?location=londis");
            return response.StatusCode == HttpStatusCode.BadRequest;
        }

        [Benchmark]
        public async Task<bool> GetWeatherForecastUsingOneOf()
        {
            var api = weatherApi.CreateClient();
            var response = await api.GetAsync("weatherforecast-usingoneof?location=londis");
            return response.StatusCode == HttpStatusCode.BadRequest;
        }
    }
}
