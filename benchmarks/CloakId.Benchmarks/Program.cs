using BenchmarkDotNet.Running;

namespace CloakId.Benchmarks;

public class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<SqidsCodecBenchmarks>();
        BenchmarkRunner.Run<JsonSerializationBenchmarks>();
    }
}
