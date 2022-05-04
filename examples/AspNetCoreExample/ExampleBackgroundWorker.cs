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

        /// <summary>
        /// Details https://wkhtmltopdf.org/libwkhtmltox/pagesettings.html
        /// Declare settings once.
        /// </summary>
        private readonly WKPDFGenConfiguration pdfConfig = new(
            new PdfSettings
            {
                DocumentTitle = "Example PDF",
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Copies = 1,
                PagesCount = true,
                LoadSettings = new LoadSettings
                {
                    BlockLocalFileAccess = false
                },
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
            }
        );

        public ExampleBackgroundWorker(IWkHtmlToPdfConverter pdfConverter, ILogger<ExampleBackgroundWorker> logger)
        {
            this.pdfConverter = pdfConverter;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var html = await File.ReadAllTextAsync(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Simple.html"), stoppingToken);

            await CreatePdf(html, stoppingToken);
            await CreatePdfOnDisk(html, stoppingToken);
        }

        private async Task CreatePdf(string html, CancellationToken stoppingToken)
        {
            var fileStream = await pdfConverter.CreateAsync(html, pdfConfig, stoppingToken);
                    
            await using var writeFileStream = File.OpenWrite("example.pdf");
                    
            await fileStream.CopyToAsync(writeFileStream, stoppingToken);

            logger.LogInformation("Created PDF file with size {Length}", writeFileStream.Length);
        }
        
        private async Task CreatePdfOnDisk(string html, CancellationToken stoppingToken)
        {
            const string path = "example-on-disk.pdf";
            
            await pdfConverter.CreateOnDiskAsync(html, path, pdfConfig, stoppingToken);
                
            logger.LogInformation("Created PDF at {Path}", path);
        }
    }
}