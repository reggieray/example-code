// See https://aka.ms/new-console-template for more information


using NBomber.CSharp;
using NBomber.Http.CSharp;
using System.Net;

Console.WriteLine("Please ensure docker is running");
Console.WriteLine("Enter choice: 1. Without OneOf. 2. With OneOf (Defaults to 1).");
var choice = Console.ReadLine();
var withOneOf = choice == "2";
var url = withOneOf ? "http://localhost:8888/weatherforecast-usingoneof?location=londis" : "http://localhost:8888/weatherforecast?location=londis";

using var httpClient = new HttpClient();

var scenario = Scenario.Create(withOneOf ? "http_with_discriminated_unions_scenario" : "http_without_discriminated_unions_scenario", async context =>
{
    var request =
        Http.CreateRequest("GET", url)
            .WithHeader("Content-Type", "application/json");

    var response = await Http.Send(httpClient, request);

    return response.StatusCode == "BadRequest" ? Response.Ok() : Response.Fail();
})
    .WithoutWarmUp()
    .WithLoadSimulations(
        Simulation.Inject(rate: 50, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromMinutes(1))
    );

NBomberRunner
    .RegisterScenarios(scenario)
    .Run();
