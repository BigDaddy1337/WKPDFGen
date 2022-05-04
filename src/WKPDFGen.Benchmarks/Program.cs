using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using WKPDFGen;
using WKPDFGen.Settings;
using WKPDFGen.Settings.Units;

BenchmarkRunner.Run<Benchmarks>();

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class Benchmarks
{
    // |                      Method |     Mean |    Error |   StdDev |  Gen 0 | Allocated |
    // |---------------------------- |---------:|---------:|---------:|-------:|----------:|
    // | PDFConfigurationConstructor | 30.93 us | 0.234 us | 0.208 us | 1.6479 |      7 KB |
    
    [Benchmark]
    public WKPDFGenConfiguration PDFConfigurationConstructor()
    {
        return new(new PdfSettings
                   {
                       ColorMode = ColorMode.Color,
                       Orientation = Orientation.Portrait,
                       PaperSize = PaperKind.A4,
                       Copies = 1,
                       PagesCount = true,
                       HeaderSettings = new HeaderSettings
                       {
                           FontSize = 14,
                           Line = false
                       },
                       FooterSettings = new FooterSettings
                       {
                           FontSize = 14,
                           Line = false,
                           Spacing = 2.0
                       }
                   });
    }

}