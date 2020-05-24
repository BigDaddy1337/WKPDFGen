using System.Collections.Generic;
using System.IO;
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
            var service = new WkHtmlToXService();
            var converter = new BasicConverter(service);

            var version = service.GetLibraryVersion();
            
            Assert.AreEqual(WkHtmlToXService.LibVersion, version);

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