using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using WKPDFGen.Converters;
using WKPDFGen.Library;
using WKPDFGen.Settings;
using WKPDFGen.Settings.Settings;
using WKPDFGen.Settings.Units;

namespace WKPDFGen.Benchmarks
{
    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.Method)]
    [SimpleJob(launchCount: 3, warmupCount: 3, targetCount: 5)]
    public class BenchmarkBasicConverter
    {
        private readonly IConverter converter = new BasicConverter(new WkHtmlToXService());
        private readonly IConverter syncConverter = new SynchronizedConverter(new WkHtmlToXService());

        private readonly PdfConfig config = new PdfConfig
        {
            GlobalPdfSettings =
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
            },
            PdfSettings = new List<PdfSettings>
            {
                new PdfSettings
                {
                    HtmlContent =
                        @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. In consectetur mauris eget ultrices iaculis.",
                }
            }
        };
        
        // |       Method |     Mean |   Error |  StdDev | Gen 0 | Gen 1 | Gen 2 | Allocated |
        // |------------- |---------:|--------:|--------:|------:|------:|------:|----------:|
        // | BasicConvert | 248.2 ms | 2.77 ms | 2.45 ms |     - |     - |     - | 111.62 KB |
        // |  SyncConvert | 249.4 ms | 1.74 ms | 1.54 ms |     - |     - |     - | 112.14 KB |
        
        [Benchmark]
        public byte[] BasicConvert() => converter.Convert(config);
        
        [Benchmark]
        public byte[] SyncConvert() => syncConverter.Convert(config);
    }
}