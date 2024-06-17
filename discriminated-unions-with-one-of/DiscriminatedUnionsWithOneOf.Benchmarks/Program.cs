// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Running;
using DiscriminatedUnionsWithOneOf.Benchmarks;

var summary = BenchmarkRunner.Run<ErrorHandlingBenchmark>();

Console.ReadKey();
