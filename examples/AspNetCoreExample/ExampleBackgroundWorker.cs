using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WKPDFGen;
using WKPDFGen.Converters;
using WKPDFGen.Settings;
using WKPDFGen.Settings.Units;

namespace AspNetCoreExample
{
    public class ExampleBackgroundWorker: BackgroundService
    {
        private readonly ILogger logger;
        
        private readonly IWkHtmlToPdfConverter pdfConverter;

        private readonly PDFConfiguration pdfConfig = new(new PdfSettings(), new GlobalPdfSettings
        {
            ColorMode = ColorMode.Color,
            Orientation = Orientation.Portrait,
            PaperSize = PaperKind.A4,
        });

        public ExampleBackgroundWorker(IWkHtmlToPdfConverter pdfConverter, ILogger<ExampleBackgroundWorker> logger)
        {
            this.pdfConverter = pdfConverter;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var html = await File.ReadAllTextAsync("Simple.html", stoppingToken);
            
            // ReSharper disable once MethodHasAsyncOverloadWithCancellation
            var result1 = pdfConverter.Convert(html, pdfConfig);
            await WriteToFileAsync("bytes", result1);
            
            var result2 = await pdfConverter.ConvertAsync(html, pdfConfig, stoppingToken);
            await WriteToFileAsync("bytes-task", result2);

            var result3 = pdfConverter.ConvertAsStream(html, pdfConfig);
            await WriteToFileAsync("stream", result3.ToArray());
            
            async Task WriteToFileAsync(string type, byte[] file)
            {
                await File.WriteAllBytesAsync($"example-{type}.pdf", file, stoppingToken);
                
                logger.LogInformation("Created PDF file with size {Length} via {Type}", file.Length, type);
            }
        }
    }
}