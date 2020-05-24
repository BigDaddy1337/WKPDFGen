using BenchmarkDotNet.Running;

namespace WKPDFGen.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<BenchmarkBasicConverter>();
        }
    }
}