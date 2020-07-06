using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using WKPDFGen.Converters;
using WKPDFGen.Library;
using WKPDFGen.Settings;
using WKPDFGen.Settings.Settings;
using WKPDFGen.Settings.Units;

namespace WKPDFGen.Tests
{
    public class Tests
    {
        private static string GetAsset(string name) => File.ReadAllText(Path.Combine("Assets", name));

        private string html;
        
        [SetUp]
        public void Setup()
        {
            html = GetAsset("Simple.html");
        }

        [Test]
        public void Convert()
        {
            var pathToLibFolder = Helper.GetLibFolder();
            
            var option =  Options.Create(new WkPdfOptions
            {
                LinuxLibPath = Path.Combine(pathToLibFolder, "libwkhtmltox.so"),
                WindowsLibPath = Path.Combine(pathToLibFolder, "libwkhtmltox.dll"),
                OsxLibPath = Path.Combine(pathToLibFolder, "libwkhtmltox.dylib")
            });
            
            var service = new WkHtmlToXService(option);
            var converter = new BasicConverter(service);

            var version = service.GetLibraryVersion();
            
            Assert.AreEqual("0.12.5", version);

            var bytes = converter.Convert(new PdfConfig
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
                        HtmlContent = html
                    }
                }
            });
            
            Assert.IsNotNull(bytes);
            
            File.WriteAllBytes("test.pdf", bytes);
        }
    }
}