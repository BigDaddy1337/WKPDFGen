using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WKPDFGen.Converters;
using WKPDFGen.Settings;
using WKPDFGen.Settings.Settings;
using WKPDFGen.Settings.Units;

namespace AspNetCoreExample
{
    public class ExampleBackgroundWorker: BackgroundService
    {
        private readonly ILogger logger;
        
        private readonly IConverter basicConverter;

        private readonly PdfConfig pdfConfig = new PdfConfig
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
                    HtmlContent = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. In consectetur mauris eget ultrices iaculis.",
                }
            }
        };

        public ExampleBackgroundWorker(IConverter basicConverter, ILogger<ExampleBackgroundWorker> logger)
        {
            this.basicConverter = basicConverter;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (stoppingToken.IsCancellationRequested == false)
            {
                await Task.Delay(3000, stoppingToken);

                var result = basicConverter.Convert(pdfConfig);
                
                await File.WriteAllBytesAsync("example.pdf", result, stoppingToken);
                
                logger.LogInformation($"Creted PDF file with size {result.Length} at {DateTime.Now}");
            }
        }
    }
}