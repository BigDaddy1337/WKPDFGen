# WKPDFGen

C# wrapper for wkhtmltopdf library for cross-platform generating PDF files from HTML 

## Usage

Download package for your OS architecture from https://wkhtmltopdf.org/downloads.html

Extract files and find OS specific library file and get path of it.

Alternatively you can check examples folder with already extracted libraries for different OS's.

```cs
// Register wrapper with path to native library file for your OS

public void ConfigureServices(IServiceCollection services)
{
    services.AddWkPdfGenerator(options =>
    {
        var nativeLibsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "0.12.6");
        
        options.LinuxLibPath = Path.Combine(nativeLibsPath, "libwkhtmltox.so.0.12.6");
        options.WindowsLibPath = Path.Combine(nativeLibsPath, "libwkhtmltox.dll");
        options.OsxLibPath = Path.Combine(nativeLibsPath, "libwkhtmltox.0.12.6.dylib");
    });
}
```

```cs
// Create configuration once before using converter

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

// Inject converter form DI

public ExampleClass(IWkHtmlToPdfConverter pdfConverter)
{
    this.pdfConverter = pdfConverter;
}

// Use the most convenient API 

private async Task CreatePdf(string html, CancellationToken stoppingToken)
{
    var fileStream = await pdfConverter.CreateAsync(html, pdfConfig, stoppingToken);
            
    await using var writeFileStream = File.OpenWrite("example.pdf");
            
    await fileStream.CopyToAsync(writeFileStream, stoppingToken);
}

private async Task CreatePdfOnDisk(string html, CancellationToken stoppingToken)
{
    const string path = "example-on-disk.pdf";
    
    await pdfConverter.CreateOnDiskAsync(html, path, pdfConfig, stoppingToken);
}
```

Check examples folder for more details.