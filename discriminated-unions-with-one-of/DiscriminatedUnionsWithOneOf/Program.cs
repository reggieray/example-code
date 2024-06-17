using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OneOf;

namespace DiscriminatedUnionsWithOneOf;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddSingleton<IWeatherService, WeatherService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseExceptionHandler(handler =>
        {
            handler.Run(async context =>
            {
                var handledException = context.Features.Get<IExceptionHandlerPathFeature>();

                if (handledException?.Error is NotSupportedException)
                {
                    var problemDetails = new ProblemDetails
                    {
                        Title = handledException.Error.Message,
                        Status = StatusCodes.Status400BadRequest,
                        Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1"
                    };
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsJsonAsync(problemDetails);
                    return;
                }

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync("Unhandled exception.");
            });
        });

        app.UseHttpsRedirection();



        app.MapGet("/weatherforecast", (IWeatherService weatherService, [FromQuery(Name = "location")] string location) => weatherService.GetWeatherForecast(location))
        .WithName("GetWeatherForecast")
        .WithOpenApi();

        app.MapGet("/weatherforecast-usingoneof", (IWeatherService weatherService, [FromQuery(Name = "location")] string location) =>
        {
            var result = weatherService.GetWeatherForecastUsingOneOf(location);

            return result.Match(
                forecast => Results.Ok(forecast),
                notSupported => Results.BadRequest(new ProblemDetails { Title = notSupported.Message }));
        })
        .WithName("GetWeatherForecastUsingOneOf")
        .WithOpenApi();

        app.Run();
    }
}

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

record NotSupportedResult(string Message);

interface IWeatherService 
{
    IEnumerable<WeatherForecast> GetWeatherForecast(string location);
    OneOf<IEnumerable<WeatherForecast>, NotSupportedResult> GetWeatherForecastUsingOneOf(string location);    
}

class WeatherService : IWeatherService
{
    private static readonly string[] SupportedLocations = ["London", "Paris", "Madrid"];

    private static readonly string[] Summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    public IEnumerable<WeatherForecast> GetWeatherForecast(string location)
    {
        if (SupportedLocations.Any(x => x.Equals(location, StringComparison.InvariantCultureIgnoreCase)))
        {
            return GetWeatherForcast();
        }

        throw new NotSupportedException($"{location} is not a supported location!");
    }

    public OneOf<IEnumerable<WeatherForecast>, NotSupportedResult> GetWeatherForecastUsingOneOf(string location)
    {
        if (SupportedLocations.Any(x => x.Equals(location, StringComparison.InvariantCultureIgnoreCase)))
        {
            return GetWeatherForcast();
        }

        return new NotSupportedResult($"{location} is not a supported location!");
    }

    private static WeatherForecast[] GetWeatherForcast()
    {
        return Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            Summaries[Random.Shared.Next(Summaries.Length)]
        ))
        .ToArray();
    }
}
